using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using System.Configuration;
using eCATBusinessServiceProtocol;
using System.Threading;
using IBankProjectBusinessActivityBase;
using RemoteTellerServiceProtocol;
using UIServiceProtocol;
using VTMModelLibrary;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using IBankProjectBusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{47BCE22F-E6F5-448A-80F0-E527E081D880}",
                         Name = "VAB_DepositSuccess",
                         NodeNameOfConfiguration = "VAB_DepositSuccess",
                         Author = "Louis")]
    public class VAB_DepositSuccess : IBankProjectActivityBase
    {
        // private bool simulator = true;
        private bool resettimer = false;
        #region constructor
        private VAB_DepositSuccess()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_DepositSuccess();
        }
        #endregion
        private string m_InputVal = string.Empty;
        [GrgBindTarget("input_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string input_val
        {
            get
            {
                return m_InputVal;
            }
            set
            {
                m_InputVal = value;
                OnPropertyChanged("input_val");
            }
        }
        private string m_OutputVal = string.Empty;
        [GrgBindTarget("output_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string output_val
        {
            get
            {
                return m_OutputVal;
            }
            set
            {
                m_OutputVal = value;

                OnPropertyChanged("output_val");
            }
        }

        #region methods

        List<string> AccountListNew = new List<string>() { };

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Fail;
                ProjVTMContext.ActionResult = emBusActivityResult_t.Failure;
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            object objdepositResutl = null;
            ProjVTMContext.TransactionDataCache.Get("VAB_DepositResutl", out objdepositResutl, GetType());
            DepositResutl depositResutl = new DepositResutl();
            DepositResult data = new DepositResult();
            if (ProjConst.simulator == false)
            {
                data = DataTest();
            }
            else
            {
                if (objdepositResutl == null)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventFail;
                    return emBusActivityResult_t.Success;
                }
                depositResutl = objdepositResutl as DepositResutl;

                data.deposit_successful_info.receive_acccount_name = depositResutl.AccountName;
                data.deposit_successful_info.receive_info = depositResutl.AccountNumber;
                data.deposit_successful_info.transaction_code = depositResutl.TransReference;
                data.deposit_successful_info.transaction_time = CommonClass.ConvertDateTime(depositResutl.ResponseTime);
                
                object objfeeTotal = null, objDepositVal = null, objAmount = null;
                ProjVTMContext.TransactionDataCache.Get("proj_FeeTotal", out objfeeTotal, GetType());
                ProjVTMContext.TransactionDataCache.Get("proj_final_deposit_val", out objDepositVal, GetType());
                ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out objAmount, GetType());
                data.deposit_successful_info.deposit_amount = CommonClass.ConvertMoney2(objAmount?.ToString());
                data.deposit_successful_info.fee_amount = CommonClass.ConvertMoney2(objfeeTotal?.ToString());
                data.deposit_successful_info.final_amount_added = CommonClass.ConvertMoney2(objAmount?.ToString());
            }
            input_val = new JavaScriptSerializer().Serialize(data);
            Log.Action.LogDebugFormat("bindingData: {0}", input_val);
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);

            emWaitSignalResult_t emWaitResult;
            Log.Action.LogDebug("Start WaitSignal 1");
            if (WaitPopu == 1)
            {
                emWaitResult = VTMWaitSignal();
            }
            else
            {
                emWaitResult = WaitSignal();
            }

            while (resettimer == true)
            {
                resettimer = false;
                ResetTimeOut(ProjVTMContext.ActionTimeout);
                emWaitResult = WaitSignal();
                if (resettimer == false) break;
            }

            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                ProjVTMContext.ActionResult = emBusActivityResult_t.Timeout;
                ProjVTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }


            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());

            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            Log.Action.LogDebugFormat("InnerOnUIEvtHandle");
            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {

                ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                //string strKey = m_AccountNo;
                Log.Action.LogDebugFormat("Selected Account: {0}", output_val);
                ProjVTMContext.TransactionDataCache.Set("VAB_SelectedAccount", output_val, GetType());
                /*if (!string.IsNullOrWhiteSpace(strKey))
                {
                    //ProjVTMContext.NextCondition = "OnNext";
                    //SignalCancel();
                    //return emBusiCallbackResult_t.Swallowd;
                }*/
                string strKeyOther = argUIEvent.Key as string;
                Log.Action.LogDebugFormat("strKeyOther:{0}", strKeyOther);

                if (strKeyOther == "continueDepositBtn")
                {
                    ProjVTMContext.NextCondition = "OnNext";
                }
                else if (strKeyOther == "OnResetTimer")
                {
                    resettimer = true;
                }
                else if (strKeyOther.ToUpper() == "ONEXIT" || strKeyOther.ToUpper() == "CANCELANDBACKHOME")
                {
                    ProjVTMContext.NextCondition = "OnExit";
                }
                else
                {
                    m_objContext.NextCondition = strKeyOther;
                }
                SignalCancel();
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        private DepositResult DataTest()
        {
            DepositResult data = new DepositResult();
            data.deposit_successful_info.receive_acccount_name = "Tran Van Lam";
            data.deposit_successful_info.receive_info = "09123456789";
            data.deposit_successful_info.transaction_code = CommonClass.ConvertMoney2("20002000");
            data.deposit_successful_info.transaction_time = CommonClass.ConvertMoney2("2000");
            data.deposit_successful_info.deposit_amount = CommonClass.ConvertMoney2("20000000");
            data.deposit_successful_info.fee_amount = "09123456789";
            data.deposit_successful_info.final_amount_added = CommonClass.ConvertMoney2("20002000");

            return data;
        }
        #endregion
    }
    
     public class DepositResult
     {
         public DepositSuccess deposit_successful_info { get; set; }

         public DepositResult()
         {
             deposit_successful_info = new DepositSuccess();
         }
     }
     public class DepositSuccess
     {
         public string receive_acccount_name { get; set; }
         public string receive_info { get; set; }
         public string transaction_code { get; set; }
         public string transaction_time { get; set; }
         public string deposit_amount { get; set; }
         public string fee_amount { get; set; }
         public string final_amount_added { get; set; }
         public DepositSuccess()
         {
             receive_acccount_name = string.Empty;
             receive_info = string.Empty;
             transaction_code = string.Empty;
             transaction_time = string.Empty;
             deposit_amount = string.Empty;
             fee_amount = string.Empty;
             final_amount_added = string.Empty;
         }
     }

}


