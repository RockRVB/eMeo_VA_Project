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
                     Name = "VAB_DepositConfirm",
                     NodeNameOfConfiguration = "VAB_DepositConfirm",
                     Author = "Louis")]
    public class VAB_DepositConfirm : IBankProjectActivityBase
    {
        // private bool simulator = true;
        private bool resettimer = false;
        #region constructor
        private VAB_DepositConfirm()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_DepositConfirm();
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
            object objCustomerInfo = null;
            ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
            CustomerInfo customerInfo = new CustomerInfo();
            DepositConfirm data = new DepositConfirm();
            if (ProjConst.simulator == false)
            {
                data = DataTest();
            }
            else
            {   
                if (objCustomerInfo == null)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventFail;
                    return emBusActivityResult_t.Success;
                }
                customerInfo = objCustomerInfo as CustomerInfo;

                data.confirm_deposit_info.account_name = customerInfo.FullName;

                object Account = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_SelectedAccount", out Account, GetType());
                Log.Action.LogDebugFormat("VAB_SelectedAccount: {0}", Account);
                if (!string.IsNullOrEmpty(Account.ToString()))
                {
                    JObject oJson = JObject.Parse(Account.ToString());
                    data.confirm_deposit_info.account_num = oJson["accountNumber"]?.ToString();
                }
                object objAmount = null;
                int amount = 0;
                ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out objAmount, GetType());
                if (objAmount != null)
                {
                    int.TryParse(objAmount?.ToString(), out amount);
                    data.confirm_deposit_info.deposit_value = CommonClass.ConvertMoney2(objAmount?.ToString());
                }
                
                List<Fee> lstFee = new List<Fee>();
                lstFee.Clear();
                object objFee = null;
                int feeTotal = 0;
                ProjVTMContext.TransactionDataCache.Get("VAB_Fee", out objFee, GetType());
                if (objFee != null)
                {
                    lstFee = objFee as List<Fee>;
                    object value = null;
                    ProjVTMContext.CardHolderDataCache.Get("VAB_ComfirmDepositTimeout", out value, GetType());
                    if (value != null && value?.ToString() == "TRUE")//no receipt
                    {
                        foreach (var item in lstFee)
                        {
                            if (item.FeeCode == "TRANS_FEE")
                            {
                                int feeVal = 0, taxVal = 0;
                                int.TryParse(item.FeeAmount, out feeVal);
                                int.TryParse(item.TaxAmount, out taxVal);
                                feeTotal = feeVal + taxVal;
                                break;
                            }
                        }

                    }
                    else
                    {
                        int fee1, fee2, tax1, tax2;
                        int.TryParse(lstFee[0].FeeAmount, out fee1);
                        int.TryParse(lstFee[1].FeeAmount, out fee2);
                        int.TryParse(lstFee[0].TaxAmount, out tax1);
                        int.TryParse(lstFee[1].TaxAmount, out tax2);
                        feeTotal = fee1 + fee2 + tax1 + tax2;
                    }
                }
                ProjVTMContext.TransactionDataCache.Set("proj_FeeTotal", feeTotal.ToString(), GetType());
                ProjVTMContext.TransactionDataCache.Set("proj_final_deposit_val", (amount - feeTotal).ToString(), GetType());
                data.confirm_deposit_info.fee_value = CommonClass.ConvertMoney2(feeTotal.ToString());
                data.confirm_deposit_info.final_deposit_val = CommonClass.ConvertMoney2((amount - feeTotal).ToString());
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

        private DepositConfirm DataTest()
        {
            DepositConfirm data = new DepositConfirm();
            data.confirm_deposit_info.account_name = "Tran Van Lam";
            data.confirm_deposit_info.account_num = "09123456789";
            data.confirm_deposit_info.deposit_value = CommonClass.ConvertMoney2("20002000");
            data.confirm_deposit_info.fee_value = CommonClass.ConvertMoney2("2000");
            data.confirm_deposit_info.final_deposit_val = CommonClass.ConvertMoney2("20000000");

            return data;
        }
        #endregion
    }
    public class DepositConfirm
    {
        public DepositInfo confirm_deposit_info { get; set; }

        public DepositConfirm()
        {
            confirm_deposit_info = new DepositInfo();
        }
    }
    public class DepositInfo
    {
        public string account_name { get; set; }
        public string account_num { get; set; }
        public string deposit_value { get; set; }
        public string fee_value { get; set; }
        public string final_deposit_val { get; set; }
        public DepositInfo()
        {
            account_name = string.Empty;
            account_num = string.Empty;
            deposit_value = string.Empty;
            fee_value = string.Empty;
            final_deposit_val = string.Empty;
        }
    }
    /* public class DepositConfirm
     {
         public DepositInfo deposit_successful_info { get; set; }

         public DepositConfirm()
         {
             deposit_successful_info = new DepositInfo();
         }
     }
     public class DepositInfo
     {
         public string receive_acccount_name { get; set; }
         public string receive_info { get; set; }
         public string transaction_code { get; set; }
         public string transaction_time { get; set; }
         public string deposit_amount { get; set; }
         public string fee_amount { get; set; }
         public string final_amount_added { get; set; }
         public DepositInfo()
         {
             receive_acccount_name = string.Empty;
             receive_info = string.Empty;
             transaction_code = string.Empty;
             transaction_time = string.Empty;
             deposit_amount = string.Empty;
             fee_amount = string.Empty;
             final_amount_added = string.Empty;
         }
     }*/

}

