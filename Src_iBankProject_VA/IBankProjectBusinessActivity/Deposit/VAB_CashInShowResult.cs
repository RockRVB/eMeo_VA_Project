/********************************************************************
Create date:      2013/3/13
File name:        VTMCashInShowResult.cs
Author:           gybiao
=====================================================================
File description: 此类完成存款过程中的等待放入钞票
---------------------------------------------------------------------
revised history:  2013/3/13, Created by gybiao
---------------------------------------------------------------------
    Copyright (C) 2012, Grgbanking CO,. Ltd. All rights reserved.
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Threading;

using eCATBusinessActivityBase;
using Attribute4ECAT;
using BusinessActivityBaseImp;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using DevServiceProtocol;
using UIServiceProtocol;
using VTMModelLibrary;
using InputOrSelect;
using CashAcceptorDeviceProtocol;
using ResourceManagerProtocol;
using eCATBusinessServiceProtocol.DataGateway;
using OCRServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using IBankProjectBusinessActivity;

namespace VTMBusinessActivity
{
    [GrgActivity("{DDC02BC8-4D1B-47BF-93D9-890019902831}",
            Name = "VAB_CashInShowResult",
            Description = "VAB_CashInShowResult",
            NodeNameOfConfiguration = "VAB_CashInShowResult",
            Catalog = "CashAcceptor",
            ForwardTargets = new string[] { "OnCashInEndForCancel", "OnRepeatNumReject", "OnRepeatNumRetract", EventDictionary.s_EventHardwareError, EventDictionary.s_EventKeyError, EventDictionary.s_EventCashInEnd })]

    public class VAB_CashInShowResult : BusinessActivitySelectFunctionBase //注意基类是BusinessActivitySelectFunction
    {
        #region method of creating
        [GrgCreateFunction("Create")]
        public new static IBusinessActivity Create()
        {
            return new VAB_CashInShowResult() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected VAB_CashInShowResult()
        {
        }
        #endregion

        #region define variable
        protected string m_JournalName = string.Empty;
        #endregion

        #region property
        [GrgBindTarget("journalKeyName", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string JournalName
        {
            get
            {
                return m_JournalName;
            }
            set
            {
                m_JournalName = value;
                OnPropertyChanged("JournalName");
            }
        }
        #endregion

        #region define variable
        protected string m_amountFlagString = string.Empty;
        protected string m_amountTotalString = string.Empty;
        protected string m_amountInputString = string.Empty;
        protected string m_jsonCountList = string.Empty;
        #endregion

        #region property
        [GrgBindTarget("AmountFlagString", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string AmountFlagString
        {
            get
            {
                return m_amountFlagString;
            }
            set
            {
                m_amountFlagString = value;
                OnPropertyChanged("AmountFlagString");
            }
        }
        #endregion

        #region property
        [GrgBindTarget("AmountTotalString", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string AmountTotalString
        {
            get
            {
                return m_amountTotalString;
            }
            set
            {
                m_amountTotalString = value;
                OnPropertyChanged("AmountTotalString");
            }
        }
        #endregion

        #region property
        [GrgBindTarget("AmountInputString", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string AmountInputString
        {
            get
            {
                return m_amountInputString;
            }
            set
            {
                m_amountInputString = value;
                OnPropertyChanged("AmountInputString");
            }
        }

        [GrgBindTarget("CountedNote", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CountedNote
        {
            get
            {
                return m_jsonCountList;
            }
            set
            {
                m_jsonCountList = value;
                OnPropertyChanged("CountedNote");
            }
        }
        /*
        bool m_DepositConfirm = true;
        
        [GrgBindTarget("DepositConfirm", Type = TargetType.Bool, Access = AccessRight.ReadAndWrite)]
        public bool DepositConfirm
        {
            get
            {
                return m_DepositConfirm;
            }
            set
            {
                m_DepositConfirm = value;
                OnPropertyChanged("DepositConfirm");
            }
        }
        */
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
        /// <summary>
        /// 存款明细
        /// </summary>
        List<VTMDepositDetail> m_depositDetailList = null;
        public List<VTMDepositDetail> SavDepositDetailList
        {
            get
            {
                return m_depositDetailList;
            }
            set
            {
                m_depositDetailList = value;
                OnPropertyChanged("SavDepositDetailList");//NoteInfo
            }
        }
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebug("Enter action: VAB_CashInShowResult");
            string totalAmount = string.Empty;
            object value = null;
            Int64 intToal = 0;
            Int64 intInput = 0;
            if (ProjConst.simulator == true)
            {
                input_val = new JavaScriptSerializer().Serialize(DataTest());
            }
            else {
                m_objContext.TransactionDataCache.Get("core_OriginalDepositAmount", out value, GetType());
                if (null != value && !string.IsNullOrEmpty(value.ToString()))
                {
                    totalAmount = value.ToString();
                    Log.Project.LogDebug("core_OriginalDepositAmount is :" + value.ToString());
                    m_objContext.TransactionDataCache.Set("proj_DEPAmount", value.ToString(), GetType());
                    intToal = Int64.Parse(totalAmount);

                    totalAmount = intToal.ToString("N");
                    m_objContext.TransactionDataCache.Set("core_FormatDepositAmount", totalAmount, GetType());
                    Log.Project.LogDebug("core_FormatDepositAmount is :" + totalAmount);
                }
                string inputAmount = string.Empty;
                object valueInput = null;
                m_objContext.TransactionDataCache.Get("core_OriginalOtherAmount", out valueInput, GetType());
                if (null != valueInput && !string.IsNullOrEmpty(valueInput.ToString()))
                {
                    Log.Project.LogDebug("core_OriginalOtherAmount is :" + valueInput.ToString());
                    inputAmount = valueInput.ToString();

                    intInput = Int64.Parse(inputAmount);
                    inputAmount = intInput.ToString("N");
                    Log.Project.LogDebug("core_FormatDepositAmount2 is :" + inputAmount);
                    m_objContext.TransactionDataCache.Set("core_FormatDepositAmount", inputAmount, GetType());
                }

                input_val = GetDepositDataList();
            }
            HandleShowCashInResult(argContext);
       //     OCRNumberSerial oCRNumberSerial = new OCRNumberSerial();
       //     oCRNumberSerial.PrintOcrInfo();
            Log.Action.LogDebug("Leave action: VAB_CashInShowResult");
            return emBusActivityResult_t.Success;
        }
        #endregion

        #region define function
        string GetDepositDataList()
        {
            Log.Action.LogDebug("Enter InputInfoMess SetDepositData()");
            string res = string.Empty;
            try
            {
                CountingResult data = new CountingResult();
                object objDepositDetail = null;
                Dictionary<int, int> depositDetail = new Dictionary<int, int>();
                m_objContext.DatabaseCache.Get(DataDictionary.s_coreDepositCountDetails, out objDepositDetail, GetType());
                if (objDepositDetail == null)
                {
                    m_objContext.TransactionDataCache.Get("core_DepositCountDetails", out objDepositDetail);
                    Log.Action.LogDebug("Datacache depositcount is null");
                }
                if (objDepositDetail != null)
                {
                    depositDetail = objDepositDetail as Dictionary<int, int>;

                    List<int> denomination = new List<int> { 500, 200, 100, 50, 20, 10 };

                    foreach (var deno in denomination)
                    {
                        string depositCount = "deposit_count_VND" + (deno * 1000).ToString();
                        string depositAmount = "deposit_amount_VND" + (deno * 1000).ToString();
                        Log.Project.LogDebugFormat("depositCount is:{0}, depositAmount is:{1}", depositCount, depositAmount);
                        object objDepositCount = null, objDepositAmount = null;

                        m_objContext.TransactionDataCache.Get(depositCount, out objDepositCount, GetType());
                        m_objContext.TransactionDataCache.Get(depositAmount, out objDepositAmount, GetType());

                        Log.Action.LogDebugFormat(" Deno is:{0}, Count is:{1}, Amount is:{2}", (deno * 1000).ToString("n0"), objDepositCount, objDepositAmount);

                        if (objDepositCount != null && objDepositAmount != null)
                        {
                            int NoteNum = 0;
                            long amountInt = 0;
                            int.TryParse(objDepositCount.ToString(), out NoteNum);
                            Int64.TryParse(objDepositAmount.ToString(), out amountInt);
                            
                            cash cash = new cash();
                            cash.price = CommonClass.ConvertMoney2((deno * 1000).ToString()) + " VND";
                            cash.quantity = NoteNum.ToString();
                            cash.total = CommonClass.ConvertMoney2(amountInt.ToString()) + " VND";

                            data.cash_slip.Add(cash);
                        }
                    }
                    res = new JavaScriptSerializer().Serialize(data);
                }
            }
            catch (System.Exception ex)
            {
                Log.Action.LogDebugFormat("InputInfoMess SetDepositData()-->Exception:{0}", ex);
            }
            return res;
        }
        List<VTMDepositDetail> GetDepositDataList1()
        {
            Log.Action.LogDebug("Enter InputInfoMess SetDepositData()");
            List<VTMDepositDetail> vtmDepositDetailList = new List<VTMDepositDetail>();

            try
            {
                object objDepositDetail = null;
                Dictionary<int, int> depositDetail = new Dictionary<int, int>();
                m_objContext.DatabaseCache.Get(DataDictionary.s_coreDepositCountDetails, out objDepositDetail, GetType());
                if (objDepositDetail == null)
                {
                    m_objContext.TransactionDataCache.Get("core_DepositCountDetails", out objDepositDetail);
                    Log.Action.LogDebug("Datacache depositcount is null");
                }
                if (objDepositDetail != null)
                {
                    depositDetail = objDepositDetail as Dictionary<int, int>;

                    List<int> denomination = new List<int> { 500, 200, 100, 50, 20, 10 };
                    
                    foreach (var deno in denomination)
                    {
                        string depositCount = "deposit_count_VND" + (deno * 1000).ToString();
                        string depositAmount = "deposit_amount_VND" + (deno * 1000).ToString();
                        //string depositCount = "deposit_count_VND" + (deno * 1000).ToString();
                        //string depositAmount = "deposit_amount_VND" + (deno * 1000).ToString();
                        Log.Project.LogDebugFormat("depositCount is:{0}, depositAmount is:{1}", depositCount,depositAmount);
                        object objDepositCount = null, objDepositAmount = null;

                        m_objContext.TransactionDataCache.Get(depositCount, out objDepositCount, GetType());
                        m_objContext.TransactionDataCache.Get(depositAmount, out objDepositAmount, GetType());

                        Log.Action.LogDebugFormat(" Deno is:{0}, Count is:{1}, Amount is:{2}", (deno * 1000).ToString("n0"), objDepositCount, objDepositAmount);

                        if (objDepositCount != null && objDepositAmount != null)
                        {
                            int NoteNum = 0;
                            long amountInt = 0;
                            int.TryParse(objDepositCount.ToString(), out NoteNum);
                            Int64.TryParse(objDepositAmount.ToString(), out amountInt);
                            VTMDepositDetail vtmDepositDetailItem = new VTMDepositDetail();
                            vtmDepositDetailItem.Denomination = (deno * 1000).ToString();
                            vtmDepositDetailItem.NoteNumber = NoteNum;
                            string amountStr = amountInt.ToString();//结果：1,234,567
                            vtmDepositDetailItem.Amount = amountStr;
                           // Log.Project.LogDebugFormat("Note50Amount is:{0}, Note50Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                          //  Log.Project.LogDebugFormat("vtmDepositDetailItem.NoteNumber is:{0},  vtmDepositDetailItem.Amount is:{1},Denomination is :{2}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount, vtmDepositDetailItem.Denomination);
                            switch (vtmDepositDetailItem.Denomination)
                            {
                                case "10000":
                                    m_objContext.TransactionDataCache.Set("proj_deno1", 10000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note10Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno1", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note10Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note10Amount is:{0}, Note10Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                case "20000":
                                    m_objContext.TransactionDataCache.Set("proj_deno2", 20000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note20Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno2", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note20Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note20Amount is:{0}, Note20Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                case "50000":
                                    m_objContext.TransactionDataCache.Set("proj_deno3", 50000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note50Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno3", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno5", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note50Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note50Amount is:{0}, Note50Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                case "100000":
                                    m_objContext.TransactionDataCache.Set("proj_deno4", 100000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note100Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno10", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note100Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note100Amount is:{0}, Note100Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                case "200000":
                                    m_objContext.TransactionDataCache.Set("proj_deno5", 200000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note200Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno20", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note200Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note200Amount is:{0}, Note200Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                case "500000":
                                    m_objContext.TransactionDataCache.Set("proj_deno6", 500000, GetType());
                                    m_objContext.TransactionDataCache.Set("Note500Amount", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("proj_rdeno50", vtmDepositDetailItem.NoteNumber, GetType());
                                    m_objContext.TransactionDataCache.Set("Note500Total", vtmDepositDetailItem.Amount, GetType());
                                    Log.Project.LogDebugFormat("Note500Amount is:{0}, Note500Total is:{1}", vtmDepositDetailItem.NoteNumber, vtmDepositDetailItem.Amount);
                                    break;
                                default:
                                    break;
                            }
                            vtmDepositDetailList.Add(vtmDepositDetailItem);
                        }
                    }
                    CountedNote = new JavaScriptSerializer().Serialize(vtmDepositDetailList);
                }
            }
            catch (System.Exception ex)
            {
                Log.Action.LogDebugFormat("InputInfoMess SetDepositData()-->Exception:{0}", ex);
            }
            return vtmDepositDetailList;
        }

        protected virtual void HandleShowCashInResult(BusinessContext argContext)
        {
            StatusInfo statusInfo;
            m_objContext.CashAcceptor.GetStatusInfo(out statusInfo, m_devTimeout);

            //如果不可继续放钞状态且TS无钞，结束存款状态 gyb add at 20130423
            object objValue = null;
            m_objContext.TransactionDataCache.Get("DepositCashInCount", out objValue);
            if (objValue != null && objValue is int)
            {
                if (int.Parse(objValue.ToString()) >= m_objContext.GeneralConfig.ContinueInterposeCashTimes && statusInfo.IntermediateStackerIsEmpty && statusInfo.TransportIsEmpty)
                {
                    Log.Action.LogError("InterposeCashTimes is overlimit and TS is empty");
                    m_objContext.NextCondition = "OnCashInEndForCancel";
                    return;
                }
            }

            //SENT重号拒钞处理 gyb add at 20130619
            if (m_objContext.CurrentRecord != null && m_objContext.GeneralConfig.EnableOCR.SENT == 1 &&
                m_objContext.GeneralConfig.EnableOCR.CIM == 1 && m_objContext.SENTConfig.RepeatNumReject > 0)
            {
                List<OCRInformation> lstOCRInfo = m_objContext.CurrentRecord.OCRInformations.ToList();
                int iRepeatNum = m_objContext.RepeatSerialNumberCount(lstOCRInfo);
                if (iRepeatNum >= m_objContext.SENTConfig.RepeatNumReject)
                {
                    Log.Action.LogWarnFormat("Current RepeatNumReject is over {0}", iRepeatNum);
                    Log.Action.LogWarnFormat("SENT DealwithModeReject is {0}", m_objContext.SENTConfig.DealwithModeReject);
                    if (m_objContext.SENTConfig.DealwithModeReject == 4)
                    {
                        m_objContext.TransactionDataCache.Set(DataDictionary.s_coreRepeatNumReject, true, GetType());
                        m_objContext.NextCondition = "OnRepeatNumReject";
                        return;
                    }
                    else if (m_objContext.SENTConfig.DealwithModeReject == 3)
                    {
                        m_objContext.TransactionDataCache.Set(DataDictionary.s_coreRepeatNumReject, true, GetType());
                        m_objContext.NextCondition = "OnRepeatNumRetract";
                        return;
                    }

                    if (m_objContext.SENTConfig.LogModeReject == 1)
                    {
                        m_objContext.LogJournalKey("IDS_RepeatSerialNumber");
                    }
                }
            }

            //set left count
            int iLeftCashCount = m_objContext.GetCurrentMaxInterposeCash();
            m_objContext.TransactionDataCache.Set(DataDictionary.s_coreLeftDepositCashCount, iLeftCashCount.ToString(), this.GetType());

            //set timeout
            m_objContext.ActionTimeout = m_objContext.GeneralConfig.Timeout.continueInterposeCash;
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                m_objContext.LogJournal("Execute base inner run fail!", LogSymbol.Alert);
                return;
            }

            //set already confirm flag
            if (!statusInfo.IntermediateStackerIsEmpty || !statusInfo.TransportIsEmpty)
            {
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreIsAlreadyConfirmedDespoitAmount, true, GetType());
            }
            
            if (EventDictionary.s_EventCancel == m_objContext.NextCondition)
            {
                //custom select cancel
                string strJournalInfo;
                m_objContext.CurrentJPTRResource.LoadString(JournalName, TextCategory.s_journal, out strJournalInfo);
                m_objContext.LogJournal(strJournalInfo);
                if (statusInfo.IntermediateStackerIsEmpty && statusInfo.TransportIsEmpty)
                {
                    //如果TS没钞，则结束存款状态
                    m_objContext.NextCondition = "OnCashInEndForCancel";
                }
            }
            else if (EventDictionary.s_EventHardwareError == m_objContext.NextCondition || EventDictionary.s_EventKeyError == m_objContext.NextCondition)
            {
                //增加键盘故障的处理(包括长按键)
                if (statusInfo.IntermediateStackerIsEmpty && statusInfo.TransportIsEmpty)
                {
                    //如果TS没有钞票，则结束存款状态
                    m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                }
                else
                {
                    //如果TS有钞，则发交易请求
                    m_objContext.NextCondition = EventDictionary.s_EventKeyError;
                    //设置键盘故障标记，在cashinend加以判断 by lsjie5
                    m_objContext.TransactionDataCache.Set(DataDictionary.b_coreIsKeyErrorInShowCashinResult, true,
                        GetType());
                }
            }
            else if (EventDictionary.s_EventTimeout == m_objContext.NextCondition)
            {
                //超时如果TS没有钞票，则结束存款状态
                if (statusInfo.IntermediateStackerIsEmpty && statusInfo.TransportIsEmpty)
                {
                    //如果TS没有钞票，则结束存款状态
                    m_objContext.NextCondition = EventDictionary.s_EventCashInEnd;
                }
                else
                {
                    //设置用户超时标记，在cashinend加以判断 by lsjie5
                    m_objContext.TransactionDataCache.Set(
                        DataDictionary.b_coreIsUserTimeoutInShowCashinResult, true,
                        GetType());
                }
            }
        }
        public CountingResult DataTest()
        {
            CountingResult data = new CountingResult();
            cash cash1 = new cash();
            cash1.price = CommonClass.ConvertMoney2("20000") + " VND";
            cash1.quantity = "5";
            cash1.total = CommonClass.ConvertMoney2("100000") + " VND";

            cash cash2 = new cash();
            cash2.price = CommonClass.ConvertMoney2("50000") + " VND";
            cash2.quantity = "5";
            cash2.total = CommonClass.ConvertMoney2("250000") + " VND";

            return data;
        }
            #endregion
        }
    public class CountingResult
    {
        public List<cash> cash_slip = null;
        public CountingResult()
        {
            cash_slip = new List<cash>();
            cash_slip.Clear();
        }
    }
    public class cash
    {
        public string price { get; set; }
        public string quantity { get; set; }
        public string total { get; set; }
        public cash()
        {
            price = string.Empty;
            quantity = string.Empty;
            total = string.Empty;
        }
    }
}
