using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{CBE93B56-A17C-445D-9876-73B6C0305AA3}",
             Name = "IBankProjSelectFunction",
             NodeNameOfConfiguration = "IBankProjSelectFunction",
             Author = "hzlin9")]
    public class IBankProjSelectFunction : IBankProjectActivityBase
    {
        #region constructor
        protected IBankProjSelectFunction()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new IBankProjSelectFunction();
        }
        #endregion

        Thread m_pollingThread;
        bool m_pollingStop;

        string m_pollingNextCondition = "0";

        [GrgBindTarget("pollingNextCondition", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string pollingNextCondition
        {
            get
            {
                return m_pollingNextCondition;

            }
            set
            {
                m_pollingNextCondition = value;
                OnPropertyChanged("pollingNextCondition");

            }
        }

        private string m_EnterTag = string.Empty;
        [GrgBindTarget("EnterTag", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string EnterTag
        {
            get
            {
                return m_EnterTag;
            }
            set
            {
                m_EnterTag = value;
                OnPropertyChanged("EnterTag");
            }
        }

        private string m_pollingTag = string.Empty;

        [GrgBindTarget("pollingTag", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string pollingTag
        {
            get
            {
                return m_pollingTag;

            }
            set
            {
                m_pollingTag = value;
                OnPropertyChanged("pollingTag");

            }
        }

        string m_projInput = "";

        [GrgBindTarget("ProjInput", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ProjInput
        {
            get
            {
                return m_projInput;

            }
            set
            {
                m_projInput = value;
                OnPropertyChanged("ProjInput");

            }
        }

        string m_inputBinding = "";

        [GrgBindTarget("inputBinding", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ProjInputBinding
        {
            get
            {
                return m_inputBinding;

            }
            set
            {
                m_inputBinding = value;
                OnPropertyChanged("inputBinding");

            }
        }

        string m_clearSpace = "0";

        [GrgBindTarget("clearSpace", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ClearSpace
        {
            get
            {
                return m_clearSpace;

            }
            set
            {
                m_clearSpace = value;
                OnPropertyChanged("clearSpace");

            }
        }

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("Execute base InnerRun failed");
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Fail;
                ProjVTMContext.ActionResult = emBusActivityResult_t.Failure;
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            ProjInput = string.Empty;
            if (!string.IsNullOrWhiteSpace(ProjInputBinding))
            {
                ProjVTMContext.TransactionDataCache.Set(ProjInputBinding, null, GetType());
            }

            if(pollingNextCondition == "1" && !string.IsNullOrWhiteSpace(pollingTag))
            {
                Log.Action.LogDebug("Start pollingThread");
                m_pollingThread = new Thread(PollingEnterTag);
                m_pollingThread.IsBackground = true;
                m_pollingThread.Start();
            }
            object objTransType = null;
            ProjVTMContext.CardHolderDataCache.Get("STM_TransTypeName", out objTransType, GetType());
            
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t emWaitResult = WaitSignal();
            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                CommonClass.WriteEJLogForSignalResult(emWaitResult);
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Fail;
                ProjVTMContext.ActionResult = emBusActivityResult_t.Timeout;
                ProjVTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            if (!string.IsNullOrWhiteSpace(ProjInput))
            {
                if (!string.IsNullOrWhiteSpace(ProjInputBinding))
                {
                    if (ClearSpace != "1")
                    {
                        ProjVTMContext.TransactionDataCache.Set(ProjInputBinding, ProjInput, GetType());
                    }
                    else
                    {
                        ProjVTMContext.TransactionDataCache.Set(ProjInputBinding, ProjInput.Replace(" ", ""), GetType());
                    }
                }
            }

            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                string strKey = argUIEvent.Key as string;
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    CommonClass.WriteEJLog(strKey);
                    //m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableBackButtonFunctionName, false);
                    //m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableConfirmButtonFunctionName, false);
                    //m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableButtonFunction1, false);
                    //m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableButtonFunction2, false);
                    //m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableButtonFunction3, false);
                    
                    m_objContext.NextCondition = strKey;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        protected override void InnerEndRun(BusinessContext argContext)
        {
            if(m_pollingThread != null)
            {
                m_pollingThread.Abort();
            }
            base.InnerEndRun(argContext);
        }

        private void PollingEnterTag()
        {
            while(!m_pollingStop )
            {
                ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                Thread.Sleep(100);
                if(EnterTag == pollingTag)
                {
                    Log.Action.LogDebug("EnterTag: " + EnterTag);
                    if (!string.IsNullOrWhiteSpace(ProjInput))
                    {
                        if (!string.IsNullOrWhiteSpace(ProjInputBinding))
                        {
                            if (ClearSpace != "1")
                            {
                                ProjVTMContext.TransactionDataCache.Set(ProjInputBinding, ProjInput, GetType());
                            }
                            else
                            {
                                ProjVTMContext.TransactionDataCache.Set(ProjInputBinding, ProjInput.Replace(" ", ""), GetType());
                            }
                        }
                    }
                    ProjVTMContext.NextCondition = pollingTag;
                    SignalCancel();
                    m_pollingStop = true;
                }
            }
        }
    }
}
