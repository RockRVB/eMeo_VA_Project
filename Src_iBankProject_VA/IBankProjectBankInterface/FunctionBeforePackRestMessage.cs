using LogProcessorService;
using System;
using VTMBusinessActivity.VTMBankInterface;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using IBankProjectBusinessServiceProtocol;
namespace IBankProjectBankInterface
{
    public partial class IBankProjectBankInterface : VTMBankInterface
    {
        private void SetJsonHeader(string argTransType)
        {
            try
            {
                string header = string.Empty;
                switch (argTransType)
                {
                    default:
                        break;
                }
                //Log.Project.LogDebugFormat("[{0}][{1}] proj_header : {2}", GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, header);
                //ProjVTMContext.TransactionDataCache.Set("proj_header", header, GetType());
            }
            catch (Exception ex)
            {
                Log.XdcTrace.LogFatal("Exception: SetJsonHeader Error!" + ex.Message);
            }
        }
        private void WriteJournalLogBefore(string argTransType)
        {
            try
            {
                object value = null;
                ProjVTMContext.LogJournal("TRANSACTION REQUEST [" + argTransType + "]");
                switch (argTransType)
                {
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Log.XdcTrace.LogFatal("Exception: WriteJournalLogBefore Error!" + ex.Message);
            }
        }
        private bool SetJsonData(string argTransType)
        {
            Log.Action.LogDebugFormat("TransType: {0}", argTransType);
            try
            {
                string terminalID = ProjVTMContext.TerminalConfig.Terminal.ATMNumber;

                switch (argTransType)
                {
                    case "QueryCustomerInfo":
                        SetQueryCustomerInfo();
                        break;
                    case "QueryTransFee":
                        SetQueryTransFee();
                        break;
                    case "GetQRString":
                    case "VerifyQR":
                        SetGetQRString();
                        break;
                    default:
                        break;
                }
            }
            catch { }
            return true;
        }
        private void SetGetQRString()
        {
            try
            {
                object value = null;
                ProjVTMContext.CardHolderDataCache.Get("VTM_TransTypeName", out value,GetType());
                if (value != null)
                {
                    if (value.ToString() == "CWD_NoCard")
                        ProjVTMContext.TransactionDataCache.Set("proj_TransType", value.ToString(), GetType());
                }
            }
            catch { }
            return;
        }
        private void SetQueryCustomerInfo()
        {
            try
            {
                object value = null;
                ProjVTMContext.TransactionDataCache.Set("VAB_ExistAccount", "N", GetType());
                ProjVTMContext.CardHolderDataCache.Get("VTM_TransTypeName", out value, GetType());
                if (value != null)
                {
                    ProjVTMContext.TransactionDataCache.Set("proj_TransType", value?.ToString(), GetType());
                }
            }
            catch { }

            return;
        }
        private void SetQueryTransFee()
        {
            try
            {
                ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", "", GetType());
                ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", "", GetType());
                ProjVTMContext.TransactionDataCache.Set("proj_TransType", "CASH_DEPOSIT", GetType());
                object objAmount = null;
                ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out objAmount, GetType());
                if (objAmount != null)
                {
                    ProjVTMContext.TransactionDataCache.Set("proj_Amount", objAmount.ToString(), GetType());
                }
                object Account = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_SelectedAccount", out Account, GetType());
                Log.Action.LogDebugFormat("VAB_SelectedAccount: {0}", Account);
                if (!string.IsNullOrEmpty(Account.ToString()))
                {
                    JObject oJson = JObject.Parse(Account.ToString());
                    ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", oJson["accountNumber"]?.ToString(), GetType());
                }
            }
            catch { }

            return;

        }
    }
}
