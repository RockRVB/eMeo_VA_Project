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
using RemoteTellerServiceProtocol;
using VTMBusinessActivity;
using VTMModelLibrary;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{981EB323-6281-429E-8826-204A0B128129}",
                 NodeNameOfConfiguration = "FingerprintScanFB",
                 Name = "FingerprintScanFB",
                 Author = "")]
    public class FingerprintScanFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerprintScanFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerprintScanFB()
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

        private string ImageBase64 = string.Empty;
        private string FeatureData = string.Empty;

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
                    Log.Action.LogError("FingerScan Can not get CIF No.");
                    return emBusActivityResult_t.Success;
                }
            }

            if (DeviceBySP != 1)
            {
                FingerprintDevice fingerprintDev = new FingerprintDevice();
                int emResult = fingerprintDev.GetFingerprintFeature(out FeatureData, out ImageBase64);
                if (emResult != 0)
                {
                    //Log.Action.LogDebugFormat("GetFingerprintFeature emResult = {0}", emResult);
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("GetFingerprintFeature Failed!");
                    return emBusActivityResult_t.Success;
                }

                if (string.IsNullOrEmpty(FeatureData))
                {
                    Log.Action.LogError("HanlderGetFingerprintFeature() featureData is null or empty");
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("GetFingerprintFeature Failed!");
                    return emBusActivityResult_t.Success;
                }
                else
                {
                    //Log.Action.LogDebug("befor Fingerprint Feature =" + featureData);
                    Log.Action.LogDebug("start Decrypt Fingerprint Feature");
                    byte[] decryptedFeature;
                    var retCode = FingerprintDevice.AratekAUFDecrypt.Decrypt(FingerprintDevice.AratekAUFDecrypt.FeatureFormat.Base64String, Encoding.ASCII.GetBytes(FeatureData), FingerprintDevice.AratekAUFDecrypt.FeatureFormat.Base64String, out decryptedFeature);
                    FeatureData = Encoding.ASCII.GetString(decryptedFeature);
                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                }
            }
            else
            {
                if (null != VTMContext.FingerService)
                {
                    SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
                    VTM_Finger.FingerImg = ImgProc.BuildPath("bmp");

                    GrgCmdScanFingerInArg objInArg = new GrgCmdScanFingerInArg()
                    {
                        UserState = Thread.CurrentThread.ManagedThreadId,
                        ImgPath = VTM_Finger.FingerImg,
                    };

                    emDevCmdResult emResult;
                    emResult = VTMContext.FingerService.HostedDevice.ExecuteCommandAsyn(FingerDeviceProtocol.FingerCmds.s_ScanFingerCmd, objInArg);
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

                    VTMContext.FingerService.CancelScanFinger();
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
            VTMContext.TransactionDataCache.Set("FB_FingerprintFeature", FeatureData, GetType());
            VTMContext.TransactionDataCache.Set("FB_FingerprintImg", ImageBase64, GetType());

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService iDev, DeviceEventArg objArg)
        {
            if (null != VTMContext.FingerService && objArg.Source == VTMContext.FingerService.HostedDevice)
            {
                if (objArg.Event == DeviceEventArg.s_CmdCompletedEvt)
                {
                    if (objArg.CommandCompleted && objArg.ResultOfCommandCompleted != null)
                    {
                        if (objArg.ResultOfCommandCompleted.IsSuccess)
                        {
                            if (FingerCmds.s_ScanFingerCmd == objArg.ResultOfCommandCompleted.CommandString)
                            {
                                GrgCmdScanFingerOutArg outObj = (GrgCmdScanFingerOutArg)objArg.ResultOfCommandCompleted;
                                if (null != outObj && !string.IsNullOrEmpty(outObj.FeatureValue))
                                {
                                    FeatureData = outObj.FeatureValue;
                                    ImageBase64 = ImgProc.ImgToBase64String(VTM_Finger.FingerImg);
                                    SendSignal(Signal.Success);
                                }
                                else
                                {
                                    Log.Action.LogDebug("Scan Finger failed,outObj  is null");
                                    SendSignal(Signal.Failure);
                                }
                            }
                            else
                            {
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
                }
                return emBusiCallbackResult_t.Swallowd;
            }

            return base.InnerOnDevEvtHandle(iDev, objArg);
        }


    }
}
