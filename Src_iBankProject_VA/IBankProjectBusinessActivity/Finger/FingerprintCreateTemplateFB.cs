using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using BusinessServiceProtocol;
using LogProcessorService;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using ResourceManagerProtocol;
using System.Threading;
using UIServiceProtocol;
using CardReaderDeviceProtocol;
using FingerDeviceProtocol;
using System.Windows;
using System.Drawing;
using eCATBusinessActivityBase;
using System.ComponentModel;
using IBankProjectBusinessActivityBase;
using FingerServerRequestService;
using System.IO;
using VTMModelLibrary;
using RemoteTellerServiceProtocol;
using VTMBusinessActivity;

namespace IBankProjectBusinessActivity
{

    [GrgActivity("{21F78DB0-2A8F-4B8B-9DD5-E8C537A10302}",
                 NodeNameOfConfiguration = "FingerprintCreateTemplateFB",
                 Name = "FingerprintCreateTemplateFB",
                 Author = "")]
    public class FingerprintCreateTemplateFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerprintCreateTemplateFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerprintCreateTemplateFB()
        {

        }
        #endregion       

        private FingerInfo currentFinger = new FingerInfo(); //指纹信息
        public FingerInfo VTM_Finger
        {
            get { return currentFinger; }
            set
            {
                currentFinger = value;
                OnPropertyChanged("VTM_Finger");
            }
        }

        private short deviceBySP = 0;//0：驱动， 1：SP
        [GrgBindTarget("DeviceBySP", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public short DeviceBySP
        {
            get { return deviceBySP; }
            set
            {
                deviceBySP = value;
                OnPropertyChanged("DeviceBySP");
            }
        }

        private short _retryTimes;
        [GrgBindTarget("RetryTimes", Access = AccessRight.OnlyRead, Type = TargetType.Short)]
        public short RetryTimes
        {
            get { return _retryTimes; }
            set
            {
                if (value != _retryTimes)
                {
                    _retryTimes = value;
                    OnPropertyChanged(nameof(RetryTimes));
                }
            }
        }

        private string ImageBase64 = string.Empty;
        private string TemplateData = string.Empty;

        private const string ErrorState = "FingerError";
        private const string PutFingerState = "FingerPutFinger";
        private const string RemoveFingerState = "FingerRemoveFinger";
        private const string ScanSuccessState = "FingerScanSuccess";

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            object objFingerDataInfo = null;
            //object objFeature = null;
            string Cif = string.Empty;

            VTMContext.CardHolderDataCache.Get("FingerDataInfo", out objFingerDataInfo, this.GetType());

            FingerDataInfo Fingerdata = null;
            if (objFingerDataInfo is FingerDataInfo)
            {
                Log.Action.LogDebug("Finger CIF get from CIF query.");
                Fingerdata = objFingerDataInfo as FingerDataInfo;
                Cif = Fingerdata.Cif;
            }
            else
            {
                object customer_id = null;
                VTMContext.TransactionDataCache.Get("FB_CustomerId", out customer_id, GetType());
                if (customer_id != null)
                {
                    Cif = customer_id.ToString();
                }
                else
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("FingerCreateTemplate Can not get CIF No.");
                    return emBusActivityResult_t.Success;
                }
            }
            

            if (DeviceBySP != 1)
            {
                FingerprintDevice fingerprintDev = new FingerprintDevice();

                int emResult = fingerprintDev.CreateFingerprintTemplate(out ImageBase64, out TemplateData);
                if (emResult != 0)
                {
                    //Log.Action.LogDebugFormat("CreateFingerprintTemplate emResult = {0}", emResult);
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("CreateFingerprintTemplate Failed!");
                    return emBusActivityResult_t.Success;
                }

                if (string.IsNullOrEmpty(TemplateData))
                {
                    Log.Action.LogDebug("fingerprintDev.TemplateData is null or empty");
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("CreateFingerprintTemplate Failed!");
                    return emBusActivityResult_t.Success;
                }
                else
                {
                    Log.Action.LogDebug("start Decrypt Fingerprint TemplateData");
                    byte[] decryptedFeature;
                    var retCode = FingerprintDevice.AratekAUFDecrypt.Decrypt(FingerprintDevice.AratekAUFDecrypt.FeatureFormat.Base64String, Encoding.ASCII.GetBytes(TemplateData), FingerprintDevice.AratekAUFDecrypt.FeatureFormat.Base64String, out decryptedFeature);
                    TemplateData = Encoding.ASCII.GetString(decryptedFeature);
                    m_objContext.NextCondition = EventDictionary.s_EventContinue;
                }

                if (string.IsNullOrEmpty(ImageBase64))
                {
                    Log.Action.LogDebug("fingerprintDev.imageBase64 is null or empty");
                }
            }
            else
            {
                if (null != VTMContext.FingerService)
                {
                    SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
                    VTM_Finger.FingerImg = ImgProc.BuildPath("bmp");
                    VTM_Finger.FingerTemplatePath = ImgProc.BuildPath("txt");
                    
                    GrgCmdScanFingerInArg objInArg = new GrgCmdScanFingerInArg()
                    {
                        UserState = Thread.CurrentThread.ManagedThreadId,
                        ImgPath = VTM_Finger.FingerImg,
                        TemplateImgPath = VTM_Finger.FingerTemplatePath
                    };

                    emDevCmdResult emResult;
                    emResult = VTMContext.FingerService.HostedDevice.ExecuteCommandAsyn(FingerDeviceProtocol.FingerCmds.s_CreateTemplateCmd, objInArg);
                    if (emResult != emDevCmdResult.Pending && emResult != emDevCmdResult.Success)
                    {
                        Log.Action.LogError("execute Finger failed");

                        VTMContext.LogJournalKey("IDS_ScanFingerFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        if (m_objContext.UIState.ContainsKey(ErrorState))
                        {
                            SwitchUIState(m_objContext.MainUI, ErrorState, 5000);
                            WaitSignal();
                        }
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        VTMContext.NextCondition = EventDictionary.s_EventFail;
                        return emBusActivityResult_t.Success;
                    }
                    string strCurSignalName;
                    emWaitSignalResult_t emWaitResult;
                    string strSignalNames = string.Format("{0},{1},{2}", Signal.Cancel, Signal.Failure, Signal.Success);
                    AddSignal(strSignalNames);
                    if (WaitPopu == 1)
                    {
                        emWaitResult = VTMWaitSignal(strSignalNames, out strCurSignalName, false);
                    }
                    else
                    {
                        emWaitResult = WaitSignal(strSignalNames, out strCurSignalName, false);
                    }
                    if (emWaitResult == emWaitSignalResult_t.Timeout)
                    {
                        VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                        VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                        VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                    }
                    else if (strCurSignalName.Equals(Signal.Success, StringComparison.OrdinalIgnoreCase))
                    {
                        SwitchUIState(m_objContext.MainUI, ScanSuccessState, 5000);
                        WaitSignal();
                        VTMContext.NextCondition = EventDictionary.s_EventContinue;
                    }
                    else if (strCurSignalName.Equals(Signal.Failure, StringComparison.OrdinalIgnoreCase))
                    {
                        if (m_objContext.UIState.ContainsKey(ErrorState))
                        {
                            SwitchUIState(m_objContext.MainUI, ErrorState, 5000);
                            WaitSignal();
                        }
                        VTMContext.LogJournalKey("IDS_ScanFingerFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                    }
                    else
                    {
                        VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                        VTMContext.NextCondition = EventDictionary.s_EventCancel;
                    }

                    VTMContext.FingerService.CancelCreateTemplate();
                }
                else
                {
                    if (m_objContext.UIState.ContainsKey(ErrorState))
                    {
                        SwitchUIState(m_objContext.MainUI, ErrorState, 5000);
                        WaitSignal();
                    }
                    VTMContext.LogJournalKey("IDS_FingerScanerNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    VTMContext.NextCondition = EventDictionary.s_EventHardwareError;
                }

            }

            VTMContext.TransactionDataCache.Set("FB_FingerprintId", Cif, GetType());
            
            VTMContext.TransactionDataCache.Set("FB_FingerprintFeature", TemplateData, GetType());

            VTMContext.TransactionDataCache.Set("FB_FingerprintImg", ImageBase64, GetType());
          
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
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
                    m_objContext.NextCondition = key;
                    VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                    SendSignal(Signal.Cancel);
                    return emBusiCallbackResult_t.Swallowd;
                }
            }

            return emBusiCallbackResult_t.Bypass;
        }

        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService iDev, DeviceEventArg objArg)
        {
            if (null != VTMContext.FingerService && objArg.Source == VTMContext.FingerService.HostedDevice)
            {
                switch (objArg.Event)
                {
                    case FingerEvent.s_MediaInserted:
                        Log.Action.LogInfo("Device event: Finger media inserted");
                        RetryTimes++;
                        if (RetryTimes < 3)
                        {
                            if (m_objContext.UIState.ContainsKey(RemoveFingerState))
                            {
                                SwitchUIState(m_objContext.MainUI, RemoveFingerState);
                            }
                        }
                        break;
                    // card take event
                    case FingerEvent.s_MediaMoved:
                        Log.Action.LogInfo("Device event: Finger media moved");
                        if (RetryTimes < 3)
                        {
                            if (m_objContext.UIState.ContainsKey(PutFingerState))
                            {
                                SwitchUIState(m_objContext.MainUI, PutFingerState);
                            }
                        }
                        break;
                    case DeviceEventArg.s_CmdCompletedEvt:
                        if (objArg.CommandCompleted && objArg.ResultOfCommandCompleted != null)
                        {
                            if (objArg.ResultOfCommandCompleted.IsSuccess)
                            {
                                if (FingerCmds.s_CreateTemplateCmd == objArg.ResultOfCommandCompleted.CommandString)
                                {
                                    ImageBase64 = ImgProc.ImgToBase64String(VTM_Finger.FingerImg);
                                    TemplateData = ImgProc.GetStringFromFile(VTM_Finger.FingerTemplatePath);
                                    SendSignal(Signal.Success);
                                }
                                else
                                {
                                    Log.Action.LogInfo("objArg.ResultOfCommandCompleted.CommandString is {0}", objArg.ResultOfCommandCompleted.CommandString);
                                    SendSignal(Signal.Success);
                                }
                            }
                            else if (objArg.ResultOfCommandCompleted.IsFailure)
                            {
                                SendSignal(Signal.Failure);
                            }
                            else if (objArg.ResultOfCommandCompleted.Cancelled)
                            {
                                SendSignal(Signal.Cancel);
                            }
                            //VTMContext.FingerService.CancelScanFinger();
                        }
                        break;
                }
                return emBusiCallbackResult_t.Swallowd;
            }

            return base.InnerOnDevEvtHandle(iDev, objArg);
        }

    }
}
