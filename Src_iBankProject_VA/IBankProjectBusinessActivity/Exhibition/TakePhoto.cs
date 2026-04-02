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

namespace VTMBusinessActivity
{
    [GrgActivity("{148B8AA5-DC2E-4845-BE65-147130E1393E}",
             NodeNameOfConfiguration = "TakePhoto",
             Name = "TakePhoto",
             Author = "clyun")]
    public class TakePhoto : BusinessActivityVTMBase
    {
        #region properties

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
        private short m_WindowHeight = 300;
        [GrgBindTarget("h", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowHeight
        {
            get
            {
                return m_WindowHeight;
            }
            set
            {
                m_WindowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }
        private short m_WindowWidth = 300;
        [GrgBindTarget("w", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short WindowWidth
        {
            get
            {
                return m_WindowWidth;
            }
            set
            {
                m_WindowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }
        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new TakePhoto() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected TakePhoto()
        {
        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            /*gxbao add for delete temp file*/
            DeleteTempFile();

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            if (VTMContext.USBCameraServiceByDriver != null)
            {
                CameraConfig cfg = VTMContext.USBCameraServiceByDriver.GetCameraConfig();
                cfg.SonWindow.X = WindowX;
                cfg.SonWindow.Y = WindowY;
                cfg.SonWindow.Width = WindowWidth;
                cfg.SonWindow.Height = WindowHeight;
                if (VTMContext.USBCameraServiceByDriver.OpenVideo() == CameraServiceErrorCode.s_success)
                {
                    emWaitSignalResult_t emWaitResult = emWaitSignalResult_t.Cancel;
                    if (WaitPopu == 1)
                    {
                        emWaitResult = VTMWaitSignal();
                    }
                    else
                    {
                        emWaitResult = WaitSignal();
                    }

                    if (emWaitSignalResult_t.Timeout == emWaitResult)
                    {
                        m_objContext.NextCondition = EventDictionary.s_EventTimeout;
                    }
                }
                else
                {
                    ShowDevError();
                }
            }
            else
            {
                ShowDevError();
            }

            //WaitSignal();

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
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
            VTMContext.USBCameraServiceByDriver.CloseCamera();

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
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    if (key.Equals("OnTakePhoto", StringComparison.OrdinalIgnoreCase))
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory + "Temp";
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }

                        string fileName = string.Format("{0}\\{1}.png", basePath, "Photo");
                        if (VTMContext.USBCameraServiceByDriver.CaptureImage(fileName) == CameraServiceErrorCode.s_success)
                        {
                            m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreTakePhotoPath, fileName, GetType());
                            //add by gxbao,将拍摄照片保存到ScanIDCardInfo
                            object obj = null;
                            ScanIDCardInfo currentIDCard = new ScanIDCardInfo();
                            m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out obj, GetType());
                            if (obj != null && obj is ScanIDCardInfo)
                            {
                                currentIDCard = (ScanIDCardInfo)obj;
                                currentIDCard.IdCard_TakePhoto = fileName;
                                m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_IDCARD, currentIDCard, GetType());
                            }

                            TakePhotoButtonVisible = false;
                            ReTakePhotoButtonVisible = true;
                            m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                            m_objContext.MainUI.ExecuteScriptCommand("photoGraph", fileName.Replace("\\", "/"));
                        }
                    }
                    else if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                    {
                        object objPhotoPath = null;
                        string strPhotoPath = string.Empty;
                        m_objContext.TransactionDataCache.Get(DataCacheKey.s_coreTakePhotoPath, out objPhotoPath, GetType());
                        if (objPhotoPath != null)
                        {
                            strPhotoPath = objPhotoPath.ToString();
                        }

                        if (!string.IsNullOrWhiteSpace(strPhotoPath) && File.Exists(strPhotoPath))
                        {
                            m_objContext.NextCondition = EventDictionary.s_EventConfirm;
                            SignalCancel();
                        }
                    }
                    else if (key.Equals(EventDictionary.s_EventLast, StringComparison.OrdinalIgnoreCase))
                    {
                        m_objContext.NextCondition = EventDictionary.s_EventLast;
                        SignalCancel();
                    }
                    else
                    {
                        m_objContext.NextCondition = key;
                        SignalCancel();
                    }
                }
            }
            else if (objArg.EventName.Equals(UIEventNames.s_PreShowEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (VTMContext.USBCameraServiceByDriver != null)
                {
                    VTMContext.USBCameraServiceByDriver.HideVideoPanel();
                }
            }
            else if (objArg.EventName.Equals(UIEventNames.s_popupEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    if (key.Equals("Cancel", StringComparison.OrdinalIgnoreCase) || key.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
                    {
                        if (VTMContext.USBCameraServiceByDriver != null)
                        {
                            VTMContext.USBCameraServiceByDriver.ShowVideoPanel();
                        }
                    }
                }
            }

            return emBusiCallbackResult_t.Swallowd;
        }



        #endregion

        #region 删除之前拍照文件
        /// <summary>
        /// 增加进入该action时删除之前拍摄照片，防止返回上一步重新进来时，不需要点击拍照就可以进入下一步---gxbao
        /// </summary>
        private void DeleteTempFile()
        {
            object objPhotoPath = null;
            string strPhotoPath = string.Empty;
            m_objContext.TransactionDataCache.Get(DataCacheKey.s_coreTakePhotoPath, out objPhotoPath, GetType());
            if (objPhotoPath != null)
            {
                strPhotoPath = objPhotoPath.ToString();
            }

            if (!string.IsNullOrWhiteSpace(strPhotoPath) && File.Exists(strPhotoPath))
            {
                try
                {
                    m_objContext.TransactionDataCache.Set(DataCacheKey.s_coreTakePhotoPath, null, GetType());
                    File.Delete(strPhotoPath);
                }
                catch (Exception)
                {
                    Log.Action.LogDebug("Delete Temp File fail!");
                }
            }
        }
        #endregion
    }
}
