using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using IDCardReaderProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using VTMBusinessServiceProtocol;
using UIServiceProtocol;
using System.Threading;
using ResourceManagerProtocol;
using System.Collections.ObjectModel;
using RemoteTellerServiceProtocol;
using CardReaderDeviceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{5A911BB6-007B-433D-B919-1A8094CC7448}",
                 Name = "VTMSelectFunction",
                 NodeNameOfConfiguration = "VTMSelectFunction",
                 Author = "wychao")]
    public class VTMSelectFunction : BusinessActivityVTMBase
    {

        #region constructor
        protected VTMSelectFunction()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VTMSelectFunction();
        }
        #endregion
        private string m_CheckDevice = "false";
        [GrgBindTarget("IsCheckDevice", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IsCheckDevice
        {
            get
            {
                return m_CheckDevice;
            }
            set
            {
                m_CheckDevice = value;
                OnPropertyChanged("IsCheckDevice");
            }
        }
        #region methods

        private string currentTrans = string.Empty;
        public override bool IsTransAvailable(string argTrans)
        {

            currentTrans = argTrans;
            return base.IsTransAvailable(argTrans);
        }

        public override bool IsDevAvailable(string argDev)
        {
            bool rlt = base.IsDevAvailable(argDev);
            if (!rlt)
            {
                VTMContext.LogJournal(string.Format("{0} not ok disable {1}", argDev, currentTrans));
                return false;
            }

            switch (argDev)
            {
                case "IDReader":
                    GrgCmdIDCStatusInfo idCardReaderStatus;
                    VTMContext.IDReader.GetStatusInfo(out idCardReaderStatus);
                    Log.Action.LogDebugFormat("ID car statuts {0},retainBin {1}", idCardReaderStatus.DeviceState, idCardReaderStatus.RetainBin);
                    if (!"OK".Equals(idCardReaderStatus.RetainBin, StringComparison.InvariantCultureIgnoreCase))
                    {
                        rlt = false;
                        VTMContext.LogJournal(string.Format("IDReader RetainBin not ok disable {0}", currentTrans));
                    }
                    break;
            }

            return rlt;
        }


        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            if (null != IsCheckDevice && IsCheckDevice.ToLower() == "true")
            {
                object passportReaderOk;
                object idCardReaderOk;
                m_objContext.TerminalDataCache.Get(VTMDataDictionary.PassportReaderOK, out passportReaderOk, GetType());
                m_objContext.TerminalDataCache.Get(VTMDataDictionary.IDCardReaderOK, out idCardReaderOk, GetType());
                Log.Action.LogDebugFormat("passportReaderOk[{0}] idCardReaderOk[{1}]",
                    passportReaderOk, idCardReaderOk);
                if ((passportReaderOk != null && passportReaderOk.ToString().ToLower() == "false") &&
                    (idCardReaderOk != null && idCardReaderOk.ToString().ToLower() == "false"))
                {
                    m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                    m_objContext.ActionResult = emBusActivityResult_t.HardwareError;
                    return emBusActivityResult_t.HardwareError;
                    // m_objContext.MainUI.ExecuteScriptCommand("Sethidden");
                    // Log.Action.LogDebug("Device fault hides the menu");
                }

            }
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.ActionResult = emBusActivityResult_t.Failure;
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t emWaitResult;
            if (WaitPopu == 1)
            {
                emWaitResult = VTMWaitSignal();
            }
            else
            {
                emWaitResult = WaitSignal();
            }
            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.EndTips = "operation timeout";
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {

            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                string strKey = argUIEvent.Key as string;
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    m_objContext.NextCondition = strKey;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        #endregion
    }
}
