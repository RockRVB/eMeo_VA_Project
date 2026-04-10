using eCATBusinessServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
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
                    default:
                        break;
                }
                //Log.Project.LogDebugFormat("[{0}][{1}] proj_header : {2}", GetType(), System.Reflection.MethodBase.GetCurrentMethod().Name, header);
                //ProjVTMContext.TransactionDataCache.Set("proj_header", header, GetType());
                GenerateRequestIDNumber();
                GetCurrentTimestamp();
                GetDeviceId();
                GetLocalIPAddress();
                ProjVTMContext.TransactionDataCache.Set("proj_STMID", ProjVTMContext.TerminalConfig.Terminal.ATMNumber, GetType());
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
        public bool GenerateRequestIDNumber()
        {
            bool result = false;
            try
            {
                string vtmno = ProjVTMContext.TerminalConfig.Terminal.ATMNumber;
                string requestId = string.Format("{0}{1}", vtmno, DateTimeOffset.Now.ToUnixTimeSeconds());
                ProjVTMContext.TransactionDataCache.Set("proj_RequestId ", requestId, GetType());
                string journalString = "- requestId    = [{0}]";
                ProjVTMContext.LogJournal(string.Format(journalString, requestId));
                result = true;
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
            return result;
        }
        private void GetDeviceId()
        {
            string deviceId = "";
            try
            {
                ProjVTMContext.TransactionDataCache.Set("proj_DeviceId ", ProjVTMContext.TerminalConfig.Terminal.ATMNumber, GetType());
               /* ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
                foreach (ManagementObject obj in searcher.Get())
                {
                    deviceId = obj["UUID"].ToString();
                    if (!String.IsNullOrEmpty(deviceId))
                    {
                        ProjVTMContext.TransactionDataCache.Set("proj_DeviceId ", deviceId, GetType());
                    }

                    break;
                }*/
            }
            catch
            {
            }
        }
        private void GetLocalIPAddress()
        {
            ProjVTMContext.TransactionDataCache.Set("proj_IpAddress ", "", GetType());
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ProjVTMContext.TransactionDataCache.Set("proj_IpAddress ", ip.ToString(), GetType());
                        break;
                    }
                }
            }
            catch
            {
            }
        }
        private void GetCurrentTimestamp()
        {
            ProjVTMContext.TransactionDataCache.Set("proj_RequestTime", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), GetType());
           // return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
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
                    case "DepositConfirm":
                        SetDepositConfirm();
                        break;
                    case "CashWithdrawalNoCardConfirm":
                        SetCashWithdrawalNoCardConfirm();
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
                //TransType vs amount
                object value = null;
                ProjVTMContext.CardHolderDataCache.Get("VTM_TransTypeName", out value, GetType());
                ProjVTMContext.TransactionDataCache.Set("proj_TransType", value?.ToString(), GetType());
				if (value.ToString() == "DEPOSIT")
				{
					ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out value, GetType());
					if (value != null)
					{
						ProjVTMContext.TransactionDataCache.Set("proj_Amount", value.ToString(), GetType());
					}
				}
				else if (value.ToString() == "CashWithdrawal_NoCard")
				{
					ProjVTMContext.TransactionDataCache.Get(DataDictionary.s_coreOriginalWithdrawalAmount, out value, GetType());
					if (value != null)
					{
						ProjVTMContext.TransactionDataCache.Set("proj_Amount", value.ToString(), GetType());
					}
				}
				//End TransType vs amount
                //branchcode vs AccountNumber
                ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", "", GetType());
                object Account = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_SelectedAccount", out Account, GetType());
                Log.Action.LogDebugFormat("VAB_SelectedAccount: {0}", Account);
                if (!string.IsNullOrEmpty(Account.ToString()))
                {
                    JObject oJson = JObject.Parse(Account.ToString());
                    ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", oJson["accountNumber"]?.ToString(), GetType());
                    object objCustomerInfo = null;
                    ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
                    CustomerInfo customerInfo = new CustomerInfo();
                    if (objCustomerInfo != null)
                    {
                        customerInfo = objCustomerInfo as CustomerInfo;
                        foreach (var item in customerInfo.Accounts)
                        {
                            if (oJson["accountNumber"]?.ToString() == item.AccountNumber)
                            {
                                ProjVTMContext.TransactionDataCache.Set("proj_BranchCode", item.BranchCode, GetType());
                                break;
                            }
                        }
                    }
                }
                //end branchcode vs AccountNumber
            }
            catch { }

            return;
        }
        private void SetDepositConfirm()
        {
            try
            {   
                //branchcode vs AccountNumber
                object Account = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_SelectedAccount", out Account, GetType());
                Log.Action.LogDebugFormat("VAB_SelectedAccount: {0}", Account);
                if (!string.IsNullOrEmpty(Account.ToString()))
                {
                    JObject oJson = JObject.Parse(Account.ToString());
                    ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", oJson["accountNumber"]?.ToString(), GetType());
                    object objCustomerInfo = null;
                    ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
                    CustomerInfo customerInfo = new CustomerInfo();
                    if (objCustomerInfo != null)
                    {
                        customerInfo = objCustomerInfo as CustomerInfo;
                        foreach (var item in customerInfo.Accounts)
                        {
                            if (oJson["accountNumber"]?.ToString() == item.AccountNumber)
                            {
                                ProjVTMContext.TransactionDataCache.Set("proj_BranchCode", item.BranchCode, GetType());
                                break;
                            }
                        }
                    }
                }
                //end branchcode vs AccountNumber
                
                ProjVTMContext.TransactionDataCache.Set("proj_Description", "Nop tien mat vao tai khoan", GetType());
                //Amount
                object objAmount = null;
                ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out objAmount, GetType());
                if (objAmount != null)
                {
                    ProjVTMContext.TransactionDataCache.Set("proj_Amount", objAmount.ToString(), GetType());
                }
                //end amount
                //Fee
                List<Fee> lstFee = new List<Fee>();
                lstFee.Clear();
                object objFee = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_Fee", out objFee, GetType());
                if (objFee != null)
                {
                    lstFee = objFee as List<Fee>;
                    object objValue = null;
                    ProjVTMContext.CardHolderDataCache.Get("VAB_ComfirmDepositTimeout", out objValue, GetType());
                    if (objValue != null && objValue?.ToString() == "TRUE")//no receipt
                    {
                        foreach (var item in lstFee)
                        {
                            if (item.FeeCode == "TRANS_FEE")
                            {
                                ProjVTMContext.TransactionDataCache.Set("proj_Fee", item.FeeAmount, GetType());
                                ProjVTMContext.TransactionDataCache.Set("proj_Tax", item.TaxAmount, GetType());
                                break;
                            }
                        }
                        
                    }
                    else
                    {
                        int fee1, fee2, fee;
                        int.TryParse(lstFee[0].FeeAmount, out fee1);
                        int.TryParse(lstFee[1].FeeAmount, out fee2);
                        fee = fee1 + fee2;

                        int tax1, tax2, tax;
                        int.TryParse(lstFee[0].TaxAmount, out tax1);
                        int.TryParse(lstFee[1].TaxAmount, out tax2);
                        tax = tax1 + tax2;
                        ProjVTMContext.TransactionDataCache.Set("proj_Fee", fee.ToString(), GetType());
                        ProjVTMContext.TransactionDataCache.Set("proj_Tax", tax.ToString(), GetType());
                    }
                }
                //End Fee
                //TransType
                object value = null;
                ProjVTMContext.CardHolderDataCache.Get("VTM_TransTypeName", out value, GetType());
                if (value != null)
                {
                    ProjVTMContext.TransactionDataCache.Set("proj_TransType", value?.ToString(), GetType());
                }
                //End TransType
            }
            catch { }

            return;
        }
        private void SetCashWithdrawalNoCardConfirm()
        {
            try
            {
                object value = null;
                ProjVTMContext.CardHolderDataCache.Get("VTM_TransTypeName", out value, GetType());
                if (value != null)
                {
                    ProjVTMContext.TransactionDataCache.Set("proj_TransType", value?.ToString(), GetType());
                    if (value.ToString() == "DEPOSIT")
                    {
                        ProjVTMContext.TransactionDataCache.Get("core_OriginalDepositAmount", out value, GetType());
                        if (value != null)
                        {
                            ProjVTMContext.TransactionDataCache.Set("proj_Amount", value.ToString(), GetType());
                        }
                    }
                    else if (value.ToString() == "CashWithdrawal_NoCard")
                    {
                        ProjVTMContext.TransactionDataCache.Get(DataDictionary.s_coreOriginalWithdrawalAmount, out value, GetType());
                        if (value != null)
                        {
                            ProjVTMContext.TransactionDataCache.Set("proj_Amount", value.ToString(), GetType());
                        }
                    }
                }
                object Account = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_SelectedAccount", out Account, GetType());
                Log.Action.LogDebugFormat("VAB_SelectedAccount: {0}", Account);
                if (!string.IsNullOrEmpty(Account.ToString()))
                {
                    JObject oJson = JObject.Parse(Account.ToString());
                    ProjVTMContext.TransactionDataCache.Set("proj_AccountNumber", oJson["accountNumber"]?.ToString(), GetType());
                    object objCustomerInfo = null;
                    ProjVTMContext.TransactionDataCache.Get("VAB_CustomerInfo", out objCustomerInfo, GetType());
                    CustomerInfo customerInfo = new CustomerInfo();
                    if (objCustomerInfo != null)
                    {
                        customerInfo = objCustomerInfo as CustomerInfo;
                        foreach (var item in customerInfo.Accounts)
                        {
                            if (oJson["accountNumber"]?.ToString() == item.AccountNumber)
                            {
                                ProjVTMContext.TransactionDataCache.Set("proj_BranchCode", item.BranchCode, GetType());
                                break;
                            }
                        }
                    }
                }
                List<Fee> lstFee = new List<Fee>();
                lstFee.Clear();
                object objFee = null;
                ProjVTMContext.TransactionDataCache.Get("VAB_Fee", out objFee, GetType());
                if (objFee != null)
                {
                    lstFee = objFee as List<Fee>;
                    ProjVTMContext.TransactionDataCache.Get("VTM_CWDNoCardReceipt", out value, GetType());
                    if (value != null && value?.ToString() == "0")//no receipt
                    {
                        foreach (var item in lstFee)
                        {
                            if (item.FeeCode == "TRANS_FEE")
                            {
                                ProjVTMContext.TransactionDataCache.Set("proj_Fee", item.FeeAmount, GetType());
                                ProjVTMContext.TransactionDataCache.Set("proj_Tax", item.TaxAmount, GetType());
                                break;
                            }
                        }

                    }
                    else if (value != null && value?.ToString() == "1") // have receipt
                        {
                        int fee1, fee2, fee;
                        int.TryParse(lstFee[0].FeeAmount, out fee1);
                        int.TryParse(lstFee[1].FeeAmount, out fee2);
                        fee = fee1 + fee2;

                        int tax1, tax2, tax;
                        int.TryParse(lstFee[0].TaxAmount, out tax1);
                        int.TryParse(lstFee[1].TaxAmount, out tax2);
                        tax = tax1 + tax2;
                        ProjVTMContext.TransactionDataCache.Set("proj_Fee", fee.ToString(), GetType());
                        ProjVTMContext.TransactionDataCache.Set("proj_Tax", tax.ToString(), GetType());
                    }
                }
            }
            catch { }

            return;
        }
    }
}
