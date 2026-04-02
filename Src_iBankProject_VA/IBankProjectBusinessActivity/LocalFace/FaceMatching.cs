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

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{148B8AA5-DC2E-4845-BE65-147130E1393E}",
             NodeNameOfConfiguration = "FaceMatching",
             Name = "FaceMatching")]
    public class FaceMatching : BusinessActivityVTMBase
    {
        #region properties

        IBankProjectBusinessServiceContext m_context = null;

        private double m_currentRate = 0.5;
        [GrgBindTarget("rate", Access = AccessRight.ReadAndWrite, Type = TargetType.Double)]
        public double Rate
        {
            get
            {
                return m_currentRate;
            }
            set
            {
                m_currentRate = value;
                OnPropertyChanged("Rate");
            }
        }

        private string m_imageDataPool = "passport_image_path";
        [GrgBindTarget("ImageDataPool", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string ImageDataPool
        {
            get
            {
                return m_imageDataPool;
            }
            set
            {
                m_imageDataPool = value;
                OnPropertyChanged("ImageDataPool");
            }
        }
        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceMatching() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceMatching()
        {
        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            string outImage = string.Empty;
            //获取护照相片路径
            object _imagePath = string.Empty;
            //获取相片特征码
            byte[] _imageFeature = null;
            //捕捉人脸特征码
            object _outFeature = null;
            //比对精度
            double _matchRate = Rate;
            int _extractFeature;

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            m_objContext.TransactionDataCache.Get(ImageDataPool, out _imagePath, GetType());

            if (_imagePath != null)
            {
                _extractFeature = m_context.FaceRecognitionService4SmartKiosk.ExtractFeature(_imagePath.ToString(), out _imageFeature);

                if (_extractFeature.Equals(0))
                {
                    m_objContext.TransactionDataCache.Get("scan_face_feature", out _outFeature, GetType());
                    if (_outFeature != null)
                    {
                        //比对
                        double outMtchRate;
                        int _result = m_context.FaceRecognitionService4SmartKiosk.Compare(_imageFeature, _outFeature as byte[], out outMtchRate);
                        if (_result.Equals(0))
                        {
                            if (outMtchRate > _matchRate)
                            {
                                if (m_objContext.UIState.ContainsKey("ScanSuccessed"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "ScanSuccessed", 3000);
                                    WaitSignal();
                                }
                                Log.Action.LogInfo("The face recognition match successed!");
                                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                                VTMContext.ActionResult = emBusActivityResult_t.Success;
                            }
                            else
                            {
                                if (m_objContext.UIState.ContainsKey("ScanFailed"))
                                {
                                    SwitchUIState(m_objContext.MainUI, "ScanFailed", 3000);
                                    WaitSignal();
                                }
                                Log.Action.LogError("The face recognition match failed.");
                                VTMContext.NextCondition = EventDictionary.s_EventFail;
                                VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                            }
                        }
                        else
                        {
                            if (m_objContext.UIState.ContainsKey("HardwareError"))
                            {
                                SwitchUIState(m_objContext.MainUI, "HardwareError", 5000);
                            }
                            Log.Action.LogError("Face recognition failed to match passport photo.");
                            VTMContext.NextCondition = EventDictionary.s_EventFail;
                            VTMContext.ActionResult = emBusActivityResult_t.HardwareError;
                        }
                    }
                }
                else
                {
                    /// <returns>-1: Unknown Error. 0: Success. 1: Quality Too Bad. 2: Face Not Found. 3: Open Image Failed.</returns>

                    if (_extractFeature.Equals(-1))
                    {
                        Log.Action.LogError("-1: Unknown Error.");
                        if (m_objContext.UIState.ContainsKey("CameraError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
                            WaitSignal();
                        }
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }
                    else if (_extractFeature.Equals(1))
                    {
                        Log.Action.LogError("1: Quality Too Bad.");
                        if (m_objContext.UIState.ContainsKey("CameraError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
                            WaitSignal();
                        }
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }
                    else if (_extractFeature.Equals(2))
                    {
                        Log.Action.LogError("2: Face Not Found.");
                        if (m_objContext.UIState.ContainsKey("CameraError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
                            WaitSignal();
                        }
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }
                    else if (_extractFeature.Equals(3))
                    {
                        Log.Action.LogError("3: Open Image Failed.");
                        if (m_objContext.UIState.ContainsKey("CameraError"))
                        {
                            SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
                            WaitSignal();
                        }
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                    }
                }
            }
            else
            {
                Log.Action.LogError("Failed to get possport image path or the path is null.");
                if (m_objContext.UIState.ContainsKey("CameraError"))
                {
                    SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
                    WaitSignal();
                }
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusActivityResult_t InnerPreRun(BusinessContext objContext)
        {
            m_context = (IBankProjectBusinessServiceContext)objContext;
            return base.InnerPreRun(objContext);
        }

        private void ShowDevError()
        {
            SwitchUIState(m_objContext.MainUI, "CameraError", 3000);
            WaitSignal();
            VTMContext.LogJournalKey("IDS_ScanCameraFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
            m_objContext.NextCondition = EventDictionary.s_EventFail;
        }

        protected override void InnerExit()
        {
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
                m_objContext.NextCondition = objArg.Key.ToString();
                SignalCancel();
            }
            return emBusiCallbackResult_t.Swallowd;
        }
        #endregion

    }
}
