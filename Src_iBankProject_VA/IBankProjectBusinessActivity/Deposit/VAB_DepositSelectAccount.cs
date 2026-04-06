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
                     Name = "BAB_DepositSelectAccount",
                     NodeNameOfConfiguration = "BAB_DepositSelectAccount",
                     Author = "Louis")]
    public class BusinessActivityBAB_DepositSelectAccount : IBankProjectActivityBase
    {

        #region constructor
        private BusinessActivityBAB_DepositSelectAccount()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityBAB_DepositSelectAccount();
        }
        #endregion
        private string m_BindingData = "";
        [GrgBindTarget("bindingData", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string bindingData
        {
            get { return m_BindingData; }
            set
            {
                m_BindingData = value;
                OnPropertyChanged("bindingData");
            }
        }

        private string m_AccountNo = "";
        [GrgBindTarget("AccountNo", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string AccountNo
        {
            get { return m_AccountNo; }
            set
            {
                m_AccountNo = value;
                OnPropertyChanged("AccountNo");
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

            if (objCustomerInfo == null)
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emBusActivityResult_t.Success;
            }

            CustomerInfo customerInfo = new CustomerInfo();
            List<DEPAccountShowing> accListShowing = new List<DEPAccountShowing>();
            accListShowing.Clear();
            customerInfo = objCustomerInfo as CustomerInfo;
            //accList = DataTest();
            foreach (var item in customerInfo.Accounts)
            {
                DEPAccountShowing tmp = new DEPAccountShowing();
                tmp.name = item.AccountName;
                tmp.account = item.AccountNumber;
                tmp.amountAvailable = item.AvailableBalance;
                Log.Action.LogDebugFormat("data: {0}, {1}, {2}, {3}", tmp.name, tmp.account, tmp.amountAvailable);
                accListShowing.Add(tmp);
            }
            if (accListShowing.Count == 0)
            {
                Log.Action.LogDebug("acclist count is 0");
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                return emBusActivityResult_t.Success;
            }
            ProjVTMContext.TransactionDataCache.Set("VAB_SelectedAccount", accListShowing[0], GetType());

            bindingData = new JavaScriptSerializer().Serialize(accListShowing);
            Log.Action.LogDebugFormat("bindingData: {0}", bindingData);
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
                string strKey = m_AccountNo;
                Log.Action.LogDebugFormat("Selected Account: {0}", m_AccountNo);
                ProjVTMContext.TransactionDataCache.Set("BAB_SelectedAccount", m_AccountNo, GetType());
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    //ProjVTMContext.NextCondition = "OnNext";
                    //SignalCancel();
                    //return emBusiCallbackResult_t.Swallowd;
                }
                string strKeyOther = argUIEvent.Key as string;
                Log.Action.LogDebugFormat("strKeyOther:{0}", strKeyOther);
                if (strKeyOther == "OnYes")
                {
                    ProjVTMContext.NextCondition = "OnYes";
                    SignalCancel();
                }
                else if (strKeyOther == "OnNext")
                {
                    ProjVTMContext.NextCondition = "OnNext";
                    SignalCancel();
                }
                else if (strKeyOther == "OnBack")
                {
                    
                    ProjVTMContext.NextCondition = "OnBack";
                    SignalCancel();
                }
                else if (strKeyOther.Equals(EventDictionary.s_EventCancel, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Action.LogDebug("Button Click s_EventCancel.");
                    m_objContext.NextCondition = strKeyOther;
                    SignalCancel();
                }
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

     /*   private List<AccountInfo> DataTest()
        {
            ProjVTMContext.CustomerInfo.FullName = "Test Name";
            List<AccountInfo> accList = new List<AccountInfo>();
            accList.Clear();
            ///Test
            AccountInfo account = new AccountInfo();
            account.AccountNumber = "060001060002517";
            account.Status = "ACTIVE";
            account.AccountType = "CURRENT";
            account.ProductCode = "020";
            account.BranchCode = "050";
            account.OpenDate = "10/10/2010";
            account.Balance = "10000000";
            account.AvalBalance = "8000000";
            account.HoldBalance = "2000000";
            account.ClosedDate = "10/10/2010";
            accList.Add(account);
            AccountInfo account1 = new AccountInfo();
            account1.AccountNumber = "054001060000977";
            account1.Status = "ACTIVE";
            account1.AccountType = "CURRENT";
            account1.ProductCode = "020";
            account1.BranchCode = "050";
            account1.OpenDate = "10/10/2010";
            account1.Balance = "10000000";
            account1.AvalBalance = "8000000";
            account1.HoldBalance = "2000000";
            account1.ClosedDate = "10/10/2010";
            accList.Add(account1);
            VTMContext.TransactionDataCache.Set("BAB_CustomerAccountList", accList, GetType());
            return accList;
        }
     */
        #endregion
    }
    public class DEPAccountShowing
    {
        public string name { get; set; }
        public string account { get; set; }
        public string amountAvailable { get; set; }
    }
}
