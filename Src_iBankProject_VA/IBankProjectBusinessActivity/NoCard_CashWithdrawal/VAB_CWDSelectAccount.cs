using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using RemoteTellerServiceProtocol;
using System;
using System.Linq;
using VTMBusinessActivityBase;
using UIServiceProtocol;
using IBankProjectBusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{DEE180DE-6AF3-473B-ADF0-1C271CA890ED}",
                     Name = "VAB_CWDSelectAccount",
                     NodeNameOfConfiguration = "VAB_CWDSelectAccount",
                     Author = "rocky",
                     ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]
    public class VAB_CWDSelectAccount : IBankProjectActivityBase
    {
        private bool ResetTimer = false;
        private string m_input_val = string.Empty;
        [GrgBindTarget("input_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string input_val
        {
            get { return m_input_val; }
            set
            {
                m_input_val = value;
                OnPropertyChanged("input_val");
            }
        }
        private string m_output_val = string.Empty;
        [GrgBindTarget("output_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string output_val
        {
            get { return m_output_val; }
            set
            {
                m_output_val = value;
                OnPropertyChanged("output_val");
            }
        }
        public VAB_CWDSelectAccount()
        {

        }

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_CWDSelectAccount();
        }
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                Log.Action.LogErrorFormat("execute {0} InnerRun failed", GetType());
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            object objCustomerInfo = null;
            ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
            CustomerInfo customerInfo = new CustomerInfo();
            if (objCustomerInfo == null)
            {
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                return emBusActivityResult_t.Success;
            }

            customerInfo = objCustomerInfo as CustomerInfo;
            var HTMLJson_input = new
            {
                cus_info = new
                {
                    cus_name = customerInfo.FullName,
                    cif = customerInfo.CIF
                },
                acc_list = customerInfo.Accounts.Select(item => new
                {
                    id = item.AccountNumber,
                    accountNumber = item.AccountNumber,
                    balance = item.AvailableBalance
                }).ToArray()
            };
            input_val = HTMLJson_input.ToJSON();

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();
            //After UI HTML show
            while (ResetTimer)
            {
                ResetTimeOut();
                ResetTimer = false;
                emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();
            }
            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
			// Xử lý button khi click vào sự kiện
			// Ví dụ string strKeyOther = argUIEvent.Key as string; strKeyOther sẽ bằng OnBack hoặc OnConfirm dựa theo html định nghĩa. Mọi xử lý nhấn button đều đưa về hàm này xử ly
			
            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
				// ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData); nó có nhiệm vụ đồng bộ lại data binding giửa C# và html
				ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
				
                string strKeyOther = argUIEvent.Key as string;
                if (strKeyOther == "OnResetTimer")
                {
                    ResetTimer = true;
                }
                else if (strKeyOther == "OnContinueWithdraw")
                {
                    Log.Project.LogDebugFormat("Selected Account: {0}", output_val);
                    ProjVTMContext.TransactionDataCache.Set("VAB_SelectedAccount", output_val, GetType());
                    VTMContext.NextCondition = strKeyOther;
                }
                else
                    VTMContext.NextCondition = strKeyOther;
                SignalCancel();
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }
        protected override emWaitSignalResult_t HandlePageTimeout(string argSignals, ref int argTimeoutOrInterval)
        {

            Log.Action.LogDebug("HandlePageTimeout ->");
            emWaitSignalResult_t emResult = emWaitSignalResult_t.Timeout;
            return emResult;
        }
        #endregion
    }
}
