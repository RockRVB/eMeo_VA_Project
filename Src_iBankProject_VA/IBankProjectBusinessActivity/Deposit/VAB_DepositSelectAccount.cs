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
using IBankProjectBusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{56E2B465-B33D-470B-A472-25E0E72A0F3B}",
                     Name = "VAB_DepositSelectAccount",
                     NodeNameOfConfiguration = "VAB_DepositSelectAccount",
                     Author = "Louis")]
    public class VAB_DepositSelectAccount : IBankProjectActivityBase
    {
       // private bool simulator = true;
        private bool resettimer = false;
        #region constructor
        private VAB_DepositSelectAccount()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_DepositSelectAccount();
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
            ProjVTMContext.TransactionDataCache.Set("proj_DepSelectedAccount", null, GetType());
            object objCustomerInfo = null;
            ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
            CustomerInfo customerInfo = new CustomerInfo();
            DEPAccountShowing data = new DEPAccountShowing();
            if (ProjConst.simulator == true)
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
                data.countdown =  (ProjVTMContext.ActionTimeout/1000).ToString();
                data.cus_info.cif = customerInfo.CIF;
                data.cus_info.cus_name = customerInfo.FullName;
                foreach (var item in customerInfo.Accounts)
                {
                    Acct acc = new Acct();
                    acc.id = item.AccountNumber;
                    acc.accountNumber = item.AccountNumber;
                    acc.balance = CommonClass.ConvertMoney2(item.AvailableBalance);
                    data.acc_list.Add(acc);
                }
                 
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

                /*else if (strKeyOther == "OnBack")
                {
                    
                    ProjVTMContext.NextCondition = "OnBack";
                    SignalCancel();
                }
                else if (strKeyOther.Equals(EventDictionary.s_EventCancel, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Action.LogDebug("Button Click s_EventCancel.");
                    m_objContext.NextCondition = strKeyOther;
                    SignalCancel();
                }*/
                SignalCancel();
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        public DEPAccountShowing DataTest()
        {
            DEPAccountShowing data = new DEPAccountShowing();
            data.countdown = (ProjVTMContext.ActionTimeout/1000).ToString(); //"120";
            data.cus_info.cif = "123";
            data.cus_info.cus_name = "Tran Van Lam";
            
            Header head1 = new Header();
            head1.label = "Số tài khoản";
            head1.ids = "ids_VAB_cash_deposit_qr_account_num";

            Header head2 = new Header();
            head2.label = "Số dư khả dụng";
            head2.ids = "ids_VAB_cash_deposit_qr_avail_balance";
            
            data.header_list.Add(head1);
            data.header_list.Add(head2);

            Acct acc1 = new Acct();
            acc1.id = "123456789000";
            acc1.accountNumber = "123456789000";
            acc1.balance = CommonClass.ConvertMoney2("20000000");

            Acct acc2 = new Acct();
            acc2.id = "123456789001";
            acc2.accountNumber = "123456789001";
            acc2.balance = CommonClass.ConvertMoney2("20000000");

            data.acc_list.Add(acc1);
            data.acc_list.Add(acc2);

            return data;
        }
        #endregion
    }
    
    public class DEPAccountShowing
    {
        public string countdown { get; set; }
        public CustomerIfo cus_info { get; set; }
        public List<Header> header_list = null;
        public List<Acct> acc_list = null;
        public DEPAccountShowing()
        {
            countdown = string.Empty;
            cus_info = new CustomerIfo();
            header_list = new List<Header>();
            header_list.Clear();
            acc_list = new List<Acct>();
            acc_list.Clear();
        }
    }
    public class CustomerIfo 
    {
        public string cus_name { get; set; }
        public string cif { get; set; }
    }
    public class Header
    {
        public string label { get; set; }
        public string ids { get; set; }
    }
    public class Acct
    {
        public string id { get; set; }
        public string accountNumber { get; set; }
        public string balance { get; set; }
    }
}
