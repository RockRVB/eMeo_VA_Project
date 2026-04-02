using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using System.Diagnostics;
using DevServiceProtocol;
using System.Collections.Generic;
using System;
using eCATBusinessServiceProtocol.DataGateway.QueryRule;
using eCATBusinessServiceProtocol.DataGateway;
using LogProcessorService;
using IBankProjectBusinessServiceProtocol;
using TwinScreen;
using eCATBusinessMaintenanceActivity;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{A723B46E-0D22-4308-A433-019A18433CBB}",
                Name = "LocalTotalsEx",
                Description = "LocalTotalsDes",
                NodeNameOfConfiguration = "LocalTotalsEx",
                Catalog = "LocalTotals",
                Author = "")]
    public class BusinessActivityLocalTotalsEx : BusinessActivityLocalTotals
    {

        #region method of creating
        /// <summary>
        /// 实现Create特性接口
        /// </summary>
        /// <returns></returns>
        [GrgCreateFunction("Create")]
        public static new IBusinessActivity Create()
        {
            return new BusinessActivityLocalTotalsEx() as IBusinessActivity;
        }
        #endregion

        #region constructor
        /// <summary>
        /// 构造函数，防止外部调用
        /// </summary>
        protected BusinessActivityLocalTotalsEx()
        {

        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        protected override bool SendSettlementMessage()
        {
            return true;
        }

        protected override bool ReceiptPrint(List<string> argBuilder)
        {
            Log.Action.LogDebugFormat("LocalTotalEx-->ReceiptPrint(List)");
            foreach (var item in argBuilder)
            {
                Log.Action.LogDebugFormat(item);
            }


            return base.ReceiptPrint(argBuilder);
        }

        protected override void HandleRPrint()
        {
            Log.Project.LogDebug("LocalTotalEx-->ReceiptPrint().");

            base.HandleRPrint();



            #region _Create Account Report
            object objAcc = null;
            Context.DatabaseCache.Get(TransactionReportType.CreateAccountList, out objAcc, this.GetType());

            if (objAcc != null)
            {
                List<AccountReportEntity> accList = objAcc as List<AccountReportEntity>;
                if (accList != null)
                {
                    try
                    {

                        List<string> ucblTransReportAcc = new List<string>();
                        ucblTransReportAcc.Add("Sl# AccNo.   Date&Time");

                        string item = string.Empty;
                        string accNumber = string.Empty;
                        string amlNumber = string.Empty;
                        for (int i = 0; i < accList.Count; i++)
                        {
                            accNumber = accList[i].AccountNumber;
                            amlNumber = accList[i].AMLNumber;
                            if (!string.IsNullOrEmpty(accNumber) && accNumber.Length > 6)
                                accNumber = accNumber.Substring(accNumber.Length - 6);
                            if (!string.IsNullOrEmpty(amlNumber) && amlNumber.Length > 6)
                                amlNumber = amlNumber.Substring(amlNumber.Length - 6);

                            item = i.ToString("000");
                            item = item + " " + "*" + accNumber;
                            //item = item + " " + "*" + amlNumber;
                            item = item + " " + accList[i].DateTime.ToString("MM-dd-yyyy hh:mm");//.Substring(accList[i].AMLNumber.Length - 6);

                            ucblTransReportAcc.Add(item);
                        }

                        //ucblTransReportAcc.Add("001 *068680 *123456 23-03-2021 10:01");
                       
                        ucblTransReportAcc.Add(" ");
                        ucblTransReportAcc.Add($"Total Account:{(ucblTransReportAcc.Count - 2)} ");

                        bool resultAcc = ReceiptPrint(ucblTransReportAcc);
                        //Log.Project.LogDebug($"Print Account Opening Report result is:{resultAcc}");

                    }
                    catch (Exception ex)
                    {
                        Log.Project.LogDebug($"Print Create Account Report Exception is:{ex}");
                    }
                }
                else
                    Log.Action.LogDebug("Create Account Report accList is null.");
            }
            else
                Log.Action.LogDebug("Create Account Report objAcc is null.");
            #endregion


            #region _Card Issue Report
            object objIssueCard = null;
            Context.DatabaseCache.Get(TransactionReportType.CardIssueList, out objIssueCard, this.GetType());
            if (objIssueCard != null)
            {
                List<CardIssueReportEntity> cardIssueList = objIssueCard as List<CardIssueReportEntity>;
                if (cardIssueList != null)
                {
                    try
                    {
                        //Log.Project.LogDebugFormat($"cardIssueList Count is:{cardIssueList.Count}");
                        List<string> transReports = new List<string>();
                        transReports.Add("Sl# AccNo.  CardNo. Stat Date&Time");

                        string item = string.Empty;
                        string accNumber = string.Empty;
                        string cardNumber = string.Empty;
                        for (int i = 0; i < cardIssueList.Count; i++)
                        {
                            accNumber = cardIssueList[i].AccountNumber;
                            cardNumber = cardIssueList[i].CardNumber;
                            if (!string.IsNullOrEmpty(accNumber) && accNumber.Length > 6)
                                accNumber = accNumber.Substring(accNumber.Length - 6);
                            if (!string.IsNullOrEmpty(cardNumber) && cardNumber.Length > 6)
                                cardNumber = cardNumber.Substring(cardNumber.Length - 6);

                            item = i.ToString("000");
                            item = item + " " + "*" + accNumber;
                            item = item + " " + "*" + cardNumber;
                            item = item + " " + cardIssueList[i].Status;
                            item = item + " " + cardIssueList[i].DateTime.ToString("MM-dd-yyyy hh:mm");//.Substring(accList[i].AMLNumber.Length - 6);

                            transReports.Add(item);
                        }


                        transReports.Add(" ");
                        transReports.Add("Total Account & Card Dispense: " + (transReports.Count - 2));
                        transReports.Add("Total Captured Card: " + transReports.FindAll(m => m.ToUpper().Contains("CAP")).Count);

                        bool result = ReceiptPrint(transReports);
                        //Log.Action.LogDebugFormat("Print Card Issueing Report result is:{0}", result);

                    }
                    catch (Exception ex)
                    {
                        Log.Action.LogDebugFormat($"Print Card Issueing Report result is:{ex}");
                    }
                }
                else
                    Log.Action.LogDebug("cardIssueList is null.");
            }
            else
                Log.Action.LogDebug("objIssueCard is null.");
            #endregion

        }

        protected override void HandleDefaultKeyPress(string argMenuId)
        {
            switch (argMenuId)
            {
                case "FUN_CLEAR":
                    Log.ElectricJournal.LogDebug("Clear all the tranaciton records.");

                    Context.DatabaseCache.Set(TransactionReportType.CreateAccountList, null, this.GetType());
                    Context.DatabaseCache.Set(TransactionReportType.CardIssueList, null, this.GetType());


                    RuleSet ruleSet = new RuleSet();
                    EqualRule equalRule = new EqualRule(TransactionTableColumnDefines.s_cardNumberColumn, "99999");
                    ruleSet.AddRule(equalRule);
                    var result = Context.TransactionGateway.RemoveRecords(ruleSet);
                    var result2 = Context.CaptureCardGateway.Clear();
                    if (result && result2)
                    {
                        base.HandleSetPageValue();
                        Context.TwinScreen.PromptOK("Clear the records successfully.");
                    }
                    else
                    {
                        Context.TwinScreen.PromptOK("Failed to clear the records.");
                    }
                    break;
                default:
                    base.HandleDefaultKeyPress(argMenuId);
                    break;
            }
        }
        private string jouPre = "Jou";

        private string recPre = "Rec";
        private int m_XDCReport = 0;
        [GrgBindTarget("XDCReport", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int XDCReport
        {
            get
            {
                return m_XDCReport;
            }
            set
            {
                m_XDCReport = value;
                OnPropertyChanged("XDCReport");
            }
        }
        /// <summary>
        /// 获取打印结账数据
        /// </summary>
        /// <param name="argFileName">文件路径</param>
        /// <returns></returns>
        protected override bool GetTTIDataInfo(out string argFileName, out string argJouFileName, out string argRecFileName, out Dictionary<int, string> argdicJournalNumber)
        {
            bool bResult = false;
            argdicJournalNumber = new Dictionary<int, string>();
            try
            {
                argFileName = string.Empty;
                argJouFileName = string.Empty;
                argRecFileName = string.Empty;
                List<string> listDetails = new List<string>();
                List<string> listJouDetails = new List<string>();
                List<string> listRecDetails = new List<string>();

                if (XDCReport == 0)
                {
                    #region 打印钱箱数据

                    GetCassetteInfo(ref listDetails, ref listJouDetails, ref listRecDetails);

                    #endregion

                    #region 打印发卡信息
                    GetCardDispenseInfo(ref listDetails, ref listJouDetails, ref listRecDetails);
                    #endregion

                    emRetCode emRet = emRetCode.Default;
                    if (m_objContext.BankInterface != null)
                    {
                        try
                        {
                            emRet = m_objContext.BankInterface.StaticDevInfo(ref listDetails, ref listJouDetails, ref listRecDetails);
                            Log.Action.LogInfoFormat("Execute StaticDevInfo return {0}", emRet);
                        }
                        catch (Exception ex)
                        {
                            Log.Action.LogError("Execute StaticDevInfo fail", ex);
                        }
                    }

                    Logger.LogInfo("Normal Total");
                    #region 统计正常交易

                    GetNormalTransTotalInfo(ref listDetails, ref listJouDetails, ref listRecDetails);

                    #endregion

                    Logger.LogInfo("Abnormal Total");
                    #region 统计异常交易

                    GetAbnormalTransTotalInfo(ref listDetails, ref listJouDetails, ref listRecDetails);

                    #endregion

                    #region 统计冲正成功交易

                    //统计取款冲正成功

                    if ((IsLocalTotalRulesExisted && Context.LocalTotalRule.PrintReversalSucTrans))
                    {
                        Logger.LogInfo("Reversal Successful Total");
                        GetReversalNormalTransTotalInfo(ref listDetails, ref listJouDetails, ref listRecDetails);
                    }

                    #endregion

                    #region 统计冲正失败交易

                    if (IsLocalTotalRulesExisted)
                    {
                        //统计取款冲正失败
                        GetReversalAbnormalTransTotalInfo(ref listDetails, ref listJouDetails, ref listRecDetails);
                    }
                    #endregion

                    int iTotalCount = listDetails.Count;
                    Logger.LogInfo("Abnormal Trans Details Total");
                    //异常交易明细
                    GetAbnormalTransDetail(Context.MiscGateway.CurrentPeriodNumber, ref listDetails, ref listJouDetails, ref listRecDetails, ref argdicJournalNumber);

                    if ((IsLocalTotalRulesExisted && Context.LocalTotalRule.IsPrintCaptureCardInfo)
                        || (!IsLocalTotalRulesExisted && Context.GeneralLocalTotalRule.IsPrintCaptureCardInfo))
                    {
                        Logger.LogInfo("Get Capture Card Info");
                        //吞卡信息
                        GetCaptureCardInfo(ref listDetails, ref listJouDetails, ref listRecDetails);
                    }

                    listDetails.Add(GetResValue("PrintTotalEnd"));
                    listJouDetails.Add(GetResValue("PrintTotalEnd", InfoMode.Jou));
                    listRecDetails.Add(GetResValue("PrintTotalEnd", InfoMode.Rec));

                }
                else
                {
                    listDetails = GetXDCPrintCounterData();
                    listJouDetails = listRecDetails = listDetails;
                }

                bResult = CommonFuncHandler.SaveFile(listDetails, out argFileName);
                bResult = CommonFuncHandler.SaveFile(listJouDetails, out argJouFileName, jouPre);
                bResult = CommonFuncHandler.SaveFile(listRecDetails, out argRecFileName, recPre);

                if (bResult)
                {

                    JudgeLocalTotalRule(argFileName, argJouFileName, argRecFileName);

                    try
                    {
                        // 发送结算报文
                        SendSettlementMessage();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                argFileName = string.Empty;
                argJouFileName = string.Empty;
                argRecFileName = string.Empty;
                Logger.LogError(ex.ToString());
            }

            return bResult;
        }

    }
}