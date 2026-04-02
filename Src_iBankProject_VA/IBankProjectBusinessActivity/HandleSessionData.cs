using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivity;
using IBankProjectBusinessActivityBase;
using IBankProjectBusinessServiceProtocol;
using LogProcessorService;
using System;
using System.Collections.Generic;

namespace VTMBusinessActivity
{
    [GrgActivity("{98797A3B-AFEE-42D7-9BF3-1860E759AAD4}",
                  Name = "HandleSessionData",
                  NodeNameOfConfiguration = "HandleSessionData",
                  Author = "")]
    public class HandleSessionData : IBankProjectActivityBase
    {
        #region constructor
        public HandleSessionData()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new HandleSessionData();
        }
        #endregion
      

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                m_objContext.ActionResult = emBusActivityResult_t.Failure;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            try
            {
                IBankProjectBusinessServiceContext ProjVTMContext = m_objContext as IBankProjectBusinessServiceContext;
                if (ProjVTMContext.SessionRecordService?.LastSession?.RecordList?.Count > 0)
                {
                    foreach (var re in ProjVTMContext.SessionRecordService.LastSession.RecordList)
                    {
                        switch (re.Transaction.ITransactionName)
                        {
                            case "AccountOpen":
                                if (re.Result.Equals("Success", StringComparison.OrdinalIgnoreCase))
                                {
                                    object objAccReportList = null;
                                    List<AccountReportEntity> accReportList = null;
                                    ProjVTMContext.DatabaseCache.Get(TransactionReportType.CreateAccountList, out objAccReportList, this.GetType());
                                    if (objAccReportList == null)
                                        accReportList = new List<AccountReportEntity>();
                                    else
                                        accReportList = objAccReportList as List<AccountReportEntity>;
                                    accReportList.Add(new AccountReportEntity()
                                    {
                                        AccountNumber = re.DataDictionary.ContainsKey("AccountNo") ? re.DataDictionary["AccountNo"] : string.Empty,
                                        AMLNumber = string.Empty,
                                        DateTime = DateTime.Now
                                    });
                                    Log.Project.LogDebug("Update Create Account Report");
                                    ProjVTMContext.DatabaseCache.Set(TransactionReportType.CreateAccountList, accReportList, this.GetType());
                                }
                                break;
                            case "CardIssue":
                                if (re.Result.Equals("Success", StringComparison.OrdinalIgnoreCase) ||re.Result.Equals("Capture", StringComparison.OrdinalIgnoreCase)  )
                                {
                                    object objCardIssueReportList = null;
                                    List<CardIssueReportEntity> cardIssueList = null;
                                    ProjVTMContext.DatabaseCache.Get(TransactionReportType.CardIssueList, out objCardIssueReportList, this.GetType());
                                    if (objCardIssueReportList == null)
                                        cardIssueList = new List<CardIssueReportEntity>();
                                    else
                                        cardIssueList = objCardIssueReportList as List<CardIssueReportEntity>;



                                    string accoutNum = string.Empty;
                                    string cardNum = re.DataDictionary.ContainsKey("CardNo")?re.DataDictionary["CardNo"] : string.Empty;
                                    string status = "ACT";
                                    if (re.Result.Equals("Capture", StringComparison.OrdinalIgnoreCase))
                                    {
                                        status = "CAP";
                                    }
                                    if (string.IsNullOrEmpty(accoutNum))
                                        Log.Project.LogDebug("accoutNum is empty.");
                                    if (string.IsNullOrEmpty(cardNum))
                                        Log.Project.LogDebug("cardNum is empty.");

                                    cardIssueList.Add(new CardIssueReportEntity()
                                    {
                                        AccountNumber = accoutNum,
                                        CardNumber = cardNum,
                                        Status = status,
                                        DateTime = DateTime.Now
                                    });
                                    Log.Project.LogDebug("Update Card Issue Report");

                                    ProjVTMContext.DatabaseCache.Set(TransactionReportType.CardIssueList, cardIssueList, this.GetType());

                                }
                                break;
                        }
                      


                    }
                     
                }
                else
                {
                    Log.Action.LogDebug("Empty session record.");
                }
                m_objContext.NextCondition = EventDictionary.s_EventContinue;

            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;

        }
    }
}
