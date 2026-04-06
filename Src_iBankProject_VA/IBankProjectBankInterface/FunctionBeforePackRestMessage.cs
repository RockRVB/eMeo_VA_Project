using LogProcessorService;
using System;
using VTMBusinessActivity.VTMBankInterface;

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
                    case "GetQRString":
                        header = string.Format("x-request-id={0};x-request-timeout={1}", Guid.NewGuid().ToString(), "30000");
                        break;
                    
                    default:
                        break;
                }
                Log.Project.LogDebugFormat("[{0}][{1}] proj_header : {2}", GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, header);
                ProjVTMContext.TransactionDataCache.Set("proj_header", header, GetType());
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
                string strAmt = "";
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
                    case "GetQRString":
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
                    
                }
            }
            catch { }

            return;
        }
        private void SetQueryCustomerInfo()
        {
            try
            {
            }
            catch { }

            return;
        }
    }
}
