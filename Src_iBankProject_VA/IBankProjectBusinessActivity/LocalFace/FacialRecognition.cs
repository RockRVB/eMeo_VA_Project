using Attribute4ECAT;
using BusinessServiceProtocol;
using CameraServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using ResourceManagerProtocol;
using System;
using System.IO;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMModelLibrary;
using IBankProjectBusinessServiceProtocol;
using DevServiceProtocol;
using RemoteTellerServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{148B8AA5-DC2E-4845-BE65-147130E1393E}",
             NodeNameOfConfiguration = "FacialRecognition",
             Name = "FacialRecognition")]
    public class FacialRecognition : BusinessActivityVTMBase
    {
        #region properties

        IBankProjectBusinessServiceContext m_context = null;
        //获取护照相片路径
        private object _imagePath = string.Empty;
        //获取相片特征码
        private byte[] _imageFeature = null;
        //捕捉人脸特征码
        private byte[] _outFeature = null;
        //比对精度
        private double _matchRate = 0;
        //拍照灯
        public const string FaceCamera = "FaceCamera";

        private int m_takePhotoTime = 3000;
        [GrgBindTarget("TakePhotoTime", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int TakePhotoTime
        {
            get
            {
                return m_takePhotoTime;
            }
            set
            {
                m_takePhotoTime = value;
                OnPropertyChanged("TakePhotoTime");
            }
        }

        private int m_TimeOut = 60000;
        [GrgBindTarget("timeout", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int Timeout
        {
            get
            {
                return m_TimeOut;
            }
            set
            {
                m_TimeOut = value;
                OnPropertyChanged("Timeout");
            }
        }

        private bool m_TakePhotoButtonVisible = true;
        [GrgBindTarget("TakePhotoButtonVisible", Type = TargetType.Bool, Access = AccessRight.ReadAndWrite)]
        public bool TakePhotoButtonVisible
        {
            get
            {
                return m_TakePhotoButtonVisible;
            }
            set
            {
                m_TakePhotoButtonVisible = value;
                OnPropertyChanged("TakePhotoButtonVisible");
            }
        }

        private bool m_ReTakePhotoButtonVisible = false;
        [GrgBindTarget("ReTakePhotoButtonVisible", Type = TargetType.Bool, Access = AccessRight.ReadAndWrite)]
        public bool ReTakePhotoButtonVisible
        {
            get
            {
                return m_ReTakePhotoButtonVisible;
            }
            set
            {
                m_ReTakePhotoButtonVisible = value;
                OnPropertyChanged("ReTakePhotoButtonVisible");
            }
        }

        private short m_WindowX = 0;
        [GrgBindTarget("x", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowX
        {
            get
            {
                return m_WindowX;
            }
            set
            {
                m_WindowX = value;
                OnPropertyChanged("WindowX");
            }
        }

        private short m_WindowY = 0;
        [GrgBindTarget("y", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowY
        {
            get
            {
                return m_WindowY;
            }
            set
            {
                m_WindowY = value;
                OnPropertyChanged("WindowY");
            }
        }
        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FacialRecognition() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FacialRecognition()
        {
        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(objContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            string outImage = string.Empty;

            m_context = (IBankProjectBusinessServiceContext)objContext;

            if (m_context.FaceRecognitionService4SmartKiosk != null)
            {
                SwitchUIState(m_context.MainUI, DataDictionary.s_DefaultUIState);
                if (m_context.FaceRecognitionService4SmartKiosk.ServiceAvailability == true)
                {
                    int retResult;
                    DevResult devResult;
                    FacialRecognize();
                    retResult = m_context.FaceRecognitionService4SmartKiosk.CaptureFace(int.Parse(m_WindowX.ToString()), int.Parse(m_WindowY.ToString()), out _outFeature, out outImage, TakePhotoTime);
                    //_imagePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "testface\\test.png");
                    //retResult = m_context.FaceRecognitionService4SmartKiosk.ExtractFeature(_imagePath.ToString(), out _imageFeature);
                    //int _result = m_context.FaceRecognitionService4SmartKiosk.Compare(_imageFeature, _outFeature, out _matchRate);
                    if (retResult.Equals(0))
                    {
                        if (outImage != string.Empty)
                        {
                            Log.Action.LogInfo($"All Path Is {AppDomain.CurrentDomain.BaseDirectory + outImage}");
                            m_objContext.TransactionDataCache.Set("scan_face_image", AppDomain.CurrentDomain.BaseDirectory + outImage, GetType());
                        }
                        Log.Action.LogInfo("Success!");
                        if (m_objContext.UIState.ContainsKey("ScanSuccessed"))
                        {
                            SwitchUIState(m_objContext.MainUI, "ScanSuccessed", 1000);
                            //WaitSignal();
                        }
                        m_objContext.TransactionDataCache.Set("scan_face_feature", _outFeature, GetType());
                        VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                        VTMContext.ActionResult = emBusActivityResult_t.Success;
                    }
                    else
                    {
                        //-1: Unknown Error. 0: Success. 1: Quality Too Bad. 2: Face Not Found. 3: Open Image Failed.</returns>
                        if (retResult.Equals(-1))
                        {
                            Log.Action.LogInfo("-1: Unknown Error.");
                            if (m_objContext.UIState.ContainsKey("CameraError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "CameraError", 5000);
                                //WaitSignal();
                            }
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                        }
                        else if (retResult.Equals(1))
                        {
                            Log.Action.LogInfo("1: Quality Too Bad.");
                            if (m_objContext.UIState.ContainsKey("CameraError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "CameraError", 5000);
                            }
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                        }
                        else if (retResult.Equals(2))
                        {
                            Log.Action.LogInfo("2: Face Not Found.");
                            if (m_objContext.UIState.ContainsKey("CameraError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "CameraError", 5000);
                            }
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                        }
                        else if (retResult.Equals(3))
                        {
                            Log.Action.LogInfo("3: Open Image Failed.");
                            if (m_objContext.UIState.ContainsKey("CameraError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "CameraError", 5000);
                            }
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                        }
                        VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                    }
                }
                else
                {
                    Log.Action.LogError("The service is not available.");
                    if (m_objContext.UIState.ContainsKey("ScanFailed"))
                    {
                        SwitchUIState(m_objContext.MainUI, "ScanFailed", 5000);
                    }
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                }
            }
            else
            {
                Log.Action.LogError("The service is not enabled.");
                if (m_objContext.UIState.ContainsKey("ScanFailed"))
                {
                    SwitchUIState(m_objContext.MainUI, "ScanFailed", 5000);
                }
                VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusActivityResult_t InnerPreRun(BusinessContext objContext)
        {
            m_context = (IBankProjectBusinessServiceContext)objContext;
            m_context.FaceRecognitionService4SmartKiosk.FaceDetected += FacialRecognize;
            ResetTimeOut(Timeout);
            return base.InnerPreRun(objContext);
        }

        private void GetPassportInfo()
        {
            m_objContext.TransactionDataCache.Get("passport_image_path", out _imagePath, GetType());
            m_context.FaceRecognitionService4SmartKiosk.ExtractFeature(_imagePath.ToString(), out _imageFeature);
        }

        public const string Off = "Off";
        public const string Slow = "Slow";
        public const string Medium = "Medium";
        public const string Quick = "Quick";
        public const string Continuous = "Continuous";
        private void FacialRecognize()
        {
            var state = Continuous;
            DevResult setLightResult = null;
            //var lightState = LightState.Off;
            //switch (state)
            //{
            //    case Off:
            //        lightState = LightState.Off;
            //        break;
            //    case Slow:
            //        lightState = LightState.Slow;
            //        break;
            //    case Medium:
            //        lightState = LightState.Medium;
            //        break;
            //    case Quick:
            //        lightState = LightState.Quick;
            //        break;
            //    case Continuous:
            //        lightState = LightState.Continuous;
            //        break;
            //}
            //setLightResult = m_context.IOBoard4SmartKiosk.ControlFaceFillLight(lightState);


//             if (setLightResult == null || setLightResult.IsFailure)
//             {
//                 VTMContext.NextCondition = EventDictionary.s_EventFail;
//                 Log.Action.LogError("Failed to set light!");
//                 //return emBusActivityResult_t.Success;
//             }
        }

        private void ShowDevError()
        {
            SwitchUIState(m_objContext.MainUI, "CameraError", 5000);
            WaitSignal();
            VTMContext.LogJournalKey("IDS_ScanCameraFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
            m_objContext.NextCondition = EventDictionary.s_EventFail;
        }

        protected override void InnerExit()
        {
            //VTMContext.USBCameraServiceByDriver.CloseCamera();
            DevResult setLightResult = null;
            //var lightState = LightState.Off;
            //setLightResult = m_context.IOBoard4SmartKiosk.ControlFaceFillLight(lightState);
//             if (setLightResult == null || setLightResult.IsFailure)
//             {
//                 VTMContext.NextCondition = EventDictionary.s_EventFail;
//                 Log.Action.LogError("Failed to set light off!");
//             }
            m_context.FaceRecognitionService4SmartKiosk.FaceDetected -= FacialRecognize;
            base.InnerExit();
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                m_context.FaceRecognitionService4SmartKiosk.CancelCaptureFace();
                m_objContext.NextCondition = objArg.Key.ToString();
                SignalCancel();
            }
            return emBusiCallbackResult_t.Swallowd;
        }
        #endregion

    }
}
