/********************************************************************
Create date:      2012/12/12
File name:        BusinessActivityDispenseCash.cs
Author:           Shalom Huang
=====================================================================
File description: Dispense cash, after inputting withdrawal amount
---------------------------------------------------------------------
revised history:  2012/12/12, Created by Shalom Huang
---------------------------------------------------------------------
    Copyright (C) 2012, Grgbanking CO,. Ltd. All rights reserved.
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCATBusinessActivityBase;
using Attribute4ECAT;
using BusinessActivityBaseImp;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using System.Diagnostics;
using System.Threading;
using CashDispenserDeviceProtocol;
using DevServiceProtocol;
using DataCacheServiceProtocol;
using LogProcessorService;
using OCRServiceProtocol;
using ResourceManagerProtocol;
using VTMBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{E993E7BF-9435-4683-8C72-454CB78781DC}",
                Name = "DispenseCashEx",
                Description = "DispenseCashDes",
                NodeNameOfConfiguration = "DispenseCashEx",
                Catalog = "DispenseCashEx",
                ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]
    public class BusinessActivityDispenseCashEx : IBankProjectActivityBase
    {
        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityDispenseCashEx() as IBusinessActivity;
        }
        #endregion

        #region constructor
        public BusinessActivityDispenseCashEx()
        {
        }
        #endregion

        #region define variable
        protected int m_dispenseWithDrawalAmount = -1;
        protected Dictionary<string, int> m_dicCassetteIDAndCountBefore;
        protected Dictionary<string, int> m_dicCassetteIDAndCountAfter;
        #endregion

        #region property
        public Dictionary<string, int> DicCassetteIDAndCount
        {
            get
            {
                Dictionary<string, int> dicCassetteIDAndCount = new Dictionary<string, int>();

                try
                {
                    List<CashUnitInfo> cashUnitInfo = GetCashUnitInfo();
                    foreach (CashUnitInfo item in cashUnitInfo)
                    {
                        if (!dicCassetteIDAndCount.ContainsKey(item.UnitID))
                        {
                            dicCassetteIDAndCount.Add(item.UnitID, item.Count);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Action.LogError(ex?.ToString());
                }

                return dicCassetteIDAndCount;
            }
        }
        #endregion

        #region override methods of base
        // valid user input amount and dispense cash
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebug("Enter action: DispenseCash");

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                m_objContext.LogJournal("Execute base inner run fail!", LogSymbol.Alert);
                Log.Action.LogDebug("Leave action: DispenseCash");
                return emRet;
            }

            //clear exception id
            if (m_objContext.EnableOCRForCDM)
            {
                m_objContext.TerminalDataCache.Set(DataDictionary.s_coreOCRExceptionTransID, null, this.GetType());
            }

            //add by wangluocheng 20140807 取款前，如果有FSN文件，则备份（非当前交易的文件，对当前交易无用，备份，需要时供查问题就好）
            if (m_objContext.BankInterface != null)
            {
                try{
                    m_objContext.BankInterface.HandleFSNFile(0);
                }
                catch (Exception ex)
                {
                    Log.Action.LogError("BankInterface.HandleFSNFile execute fail", ex);
                }
            }
            //add end

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

            if (null != m_objContext.CashDispenser)
            {
                object value = null;
                m_objContext.TransactionDataCache.Get(DataDictionary.s_coreOriginalWithdrawalAmount, out value, GetType());
                Debug.Assert(null != value);
                if (null != value)
                {
                    try
                    {
                        m_dispenseWithDrawalAmount = int.Parse(value.ToString());
                    }
                    catch (System.Exception ex)
                    {
                        m_objContext.LogJournal(string.Format(m_objContext.CurrentJPTRResource.LoadString("IDS_InvalidDispenseAmount", TextCategory.s_journal), value), LogSymbol.Alert);
                        m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReversalForDispenseFail", TextCategory.s_journal), LogSymbol.Alert);

                        if (SwitchUIState(m_objContext.MainUI, "AmountError", 3000))
                            WaitSignal();

                        m_objContext.NextCondition = EventDictionary.s_EventHardwareError;

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            m_objContext.CurrentRecord.CashDevErrorFlag = CashDevErrorFlag.s_DenominateFail;     // 1-取款配钞失败
                            m_objContext.CurrentRecord.CashDevErrorCode = "Amount is invalid";
                            m_objContext.ReversalService.SetReversalFlagAndData();
                            m_objContext.CurrentRecord.Submit();
                        }
                        Log.Action.LogError(ex?.ToString());
                        goto _exit;
                    }

                    // validate withdrawal flow
                    if (!ValidateWithdrawalFlow())
                    {
                        m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                        goto _exit;
                    }
                    var timeOut = m_objContext.GeneralConfig.Timeout;

                    if (m_withdrawalSecurityWatch.ElapsedMilliseconds > timeOut.sendToDispense)
                    {
                        m_objContext.LogJournal("Withdrawal: send to dispense time too long", LogSymbol.Alert);
                        m_objContext.NextCondition = EventDictionary.s_EventHardwareError;

                        // update transaction record
                        if (null != m_objContext.CurrentRecord)
                        {
                            m_objContext.CurrentRecord.CashDevErrorFlag = CashDevErrorFlag.s_OtherError;     // 其他故障
                            m_objContext.CurrentRecord.CashDevErrorCode = "Send to dispense time too long";
                            m_objContext.ReversalService.SetReversalFlagAndData();
                            m_objContext.CurrentRecord.Submit();
                        }

                        goto _exit;
                    }

                    // dispense cash
                    value = null;
                    List<CashUnitInfo> tmpBeforeCashUnitInfo = new List<CashUnitInfo>();
                    tmpBeforeCashUnitInfo.Clear();

                    List<CashUnitInfo> tmpAfterCashUnitInfo = new List<CashUnitInfo>();
                    tmpAfterCashUnitInfo.Clear();
                    m_dicCassetteIDAndCountBefore = DicCassetteIDAndCount;
                    tmpBeforeCashUnitInfo = m_objContext.GetCDMCashUnitInfo();

                    //**********************************************************************************************
                    //增加配钞标志位，如果发送配钞指令则记录为true modify by liaoyuyu 2015-07-10
                    m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreAleadyDispenseCashFlag, null, GetType());
                    //**********************************************************************************************

                    //**********************************************************************************************
                    //增加送钞标志位，如果发送送钞指令则记录为true modify by liaoyuyu 2015-07-10
                    m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreAleadyPresentCashFlag, null, GetType());
                    //**********************************************************************************************
                    // Define the dictionary cassette note command to dispense note
                    Dictionary<string, int> dictCassetteNoteCommand = new Dictionary<string, int>();
                    Dictionary<string, int> tmpDictCassetteNoteResult = new Dictionary<string, int>();
                    Dictionary<string, int> tmpDictCassetteRejectResult = new Dictionary<string, int>();

                    tmpDictCassetteNoteResult.Clear();
                    tmpDictCassetteRejectResult.Clear();

                    object objValue = null;
                    m_objContext.TransactionDataCache.Get("core_NotesToDispense", out objValue);

                    object objType1, objType2, objType3, objType4;
                    m_objContext.TerminalDataCache.Get("core_CassType1UID", out objType1);
                    m_objContext.TerminalDataCache.Get("core_CassType2UID", out objType2);
                    m_objContext.TerminalDataCache.Get("core_CassType3UID", out objType3);
                    m_objContext.TerminalDataCache.Get("core_CassType4UID", out objType4);

                    if (string.IsNullOrEmpty(objType1.ToString()) || string.IsNullOrEmpty(objType2.ToString()) || string.IsNullOrEmpty(objType3.ToString()) || string.IsNullOrEmpty(objType4.ToString()))
                    {
                        Log.Action.LogDebug("************Dispense Error");
                        m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                        goto _exit;
                    }

                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType1", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType2", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType3", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType4", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType1", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType2", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType3", "0", GetType());
                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType4", "0", GetType());

                    List<CashUnitInfo> cashUnitInfo = GetCashUnitInfo();
                    foreach (CashUnitInfo item in cashUnitInfo)
                    {
                        string UnitID = item.UnitID;
                        if (!string.IsNullOrWhiteSpace(UnitID))
                        {
                            Log.Action.LogDebugFormat("******************* cassette status test*********{0}, count{1}", item.Status, item.Count);
                            if (CashUnitStatus.EMPTY == item.Status)
                            {
                                Log.Action.LogDebugFormat("******************* cassette {0} status is empty!", UnitID);
                                if (objType1.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType1", "3", GetType());
                                }

                                else if (objType2.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType2", "3", GetType());
                                }

                                else if (objType3.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType3", "3", GetType());
                                }

                                else if (objType4.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType4", "3", GetType());
                                }
                            }
                            else if (CashUnitStatus.LOW == item.Status)
                            {
                                Log.Action.LogDebugFormat("******************* cassette {0} status is Low!", UnitID);
                                if (objType1.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType1", "2", GetType());
                                }

                                else if (objType2.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType2", "2", GetType());
                                }

                                else if (objType3.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType3", "2", GetType());
                                }

                                else if (objType4.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_SuppliesStatusCassType4", "2", GetType());
                                }
                            }
                            else if (CashUnitStatus.MISSING == item.Status || CashUnitStatus.UNKNOWN == item.Status || CashUnitStatus.NOVAL == item.Status)
                            {
                                Log.Action.LogDebugFormat("******************* cassette {0} status is Error!", UnitID);
                                if (objType1.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType1", "4", GetType());
                                }

                                else if (objType2.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType2", "4", GetType());
                                }

                                else if (objType3.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType3", "4", GetType());
                                }

                                else if (objType4.ToString() == UnitID)
                                {
                                    m_objContext.TerminalDataCache.Set("core_ErrorSeverityCassType4", "4", GetType());
                                }
                            }
                        }
                    } 

                    List<string> CassetteList = new List<string>();
                    CassetteList.Add(objType1.ToString());
                    CassetteList.Add(objType2.ToString());
                    CassetteList.Add(objType3.ToString());
                    CassetteList.Add(objType4.ToString());

                    //if (cashUnitInfo.Count > 7 && objValue != null)
                    if (cashUnitInfo.Count > 3 && objValue != null)
                    {
                        for (int i=0; i<4; i++)
                        {
                            string str = objValue.ToString().Substring(i*2, 2);
                            int temp = int.Parse(str);
                            if (temp > 0)
                            {
                               // Log.Action.LogDebugFormat("cashUnitInfo ID:{0}, Numbers:{1}", CassetteList[i], temp);
                                dictCassetteNoteCommand.Add(CassetteList[i], temp);
                            }
                        }
                    }

                    DevResult bResult = m_objContext.CashDispenser.Dispense(dictCassetteNoteCommand, out tmpDictCassetteNoteResult);
                    //bool bResult = ExecuteDevCommand(CashDispenserCmdIndex.s_DispenseCmd, out value, m_dispenseWithDrawalAmount, GetCurrencyForCashTrans());

                    //************************************************************************************************
                    //增加配钞标志位，如果发送配钞指令则记录为true modify by liaoyuyu 2015-07-10
                    m_objContext.CardHolderDataCache.Set(DataDictionary.s_coreAleadyDispenseCashFlag, bResult.IsSuccess, GetType());
                    //************************************************************************************************

                    tmpAfterCashUnitInfo = m_objContext.GetCDMCashUnitInfo();
                    m_dicCassetteIDAndCountAfter = DicCassetteIDAndCount;

                    GetDispenseNoteResult(tmpBeforeCashUnitInfo, tmpAfterCashUnitInfo, ref tmpDictCassetteNoteResult, ref tmpDictCassetteRejectResult);

                    Log.Action.LogDebugFormat("Dispense result: {0}", bResult.IsSuccess);
                    if (bResult.IsFailure)
                    {
                        foreach (var item in tmpDictCassetteNoteResult)
                            ;// Log.Action.LogDebugFormat("tmpDictCassetteNoteResult Dispense result: {0}-{1}", item.Key, item.Value);
                        foreach (var item in tmpDictCassetteRejectResult)
                            ;// Log.Action.LogDebugFormat("tmpDictCassetteRejectResult Dispense result: {0}-{1}", item.Key, item.Value);
                    }

                    if ((null == tmpDictCassetteNoteResult) || (tmpDictCassetteNoteResult.Count <= 0))
                    {
                        Log.Action.LogDebug("*No Note Dispensed!");
                    }

                    if (bResult.IsSuccess)
                    {
                        string dispenseInfo = string.Empty;
                        Dictionary<int, int> denominationList = new Dictionary<int, int>();
                        Dictionary<string, int> dispenseList = tmpDictCassetteNoteResult;
                        foreach (var item in dispenseList)
                        {
                            if (item.Value > 0)
                            {
                                string outValue = null;
                                m_objContext.CashDispenser.GetPUInfo(out outValue, item.Key, CashUnitInfoType.NoteValue);
                                if (null != outValue)
                                {
                                    int denomination = 0;
                                    if (int.TryParse(outValue, out denomination))
                                    {
                                        if (!denominationList.ContainsKey(denomination))
                                        {
                                            denominationList.Add(denomination, item.Value);
                                        }
                                        else
                                        {
                                            denominationList[denomination] += item.Value;
                                        }

                                        if (string.IsNullOrWhiteSpace(dispenseInfo))
                                        {
                                            dispenseInfo = string.Format("{0}.{1}.{2}", item.Key, denomination, item.Value);
                                        }
                                        else
                                        {
                                            dispenseInfo += string.Format("/{0}.{1}.{2}", item.Key, denomination, item.Value);
                                            m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_DispenseInfo", TextCategory.s_journal) + dispenseInfo);
                                            dispenseInfo = string.Empty;
                                        }
                                    }
                                }
                            }
                        }

                        m_objContext.TransactionDataCache.Set("core_WithdrawalCountDetails", denominationList, GetType());

                        if (!string.IsNullOrWhiteSpace(dispenseInfo))
                        {
                            m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_DispenseInfo", TextCategory.s_journal) + dispenseInfo);
                        }
                    }

                    //add ocr method
                    //*****gyb add at 20130419 begin*****
                    if (m_objContext.EnableOCRForCDM)
                    {
                        //save ocr info
                        if (null != m_objContext.CurrentRecord || InsertTransRecord())
                        {
                            m_objContext.OCRService.SaveOCRInfo(m_objContext.CurrentRecord.ID, OCRCommandType.Dispense);
                        }
                    }
                    //*****gyb add at 20130419 end*****

                    if (bResult.IsFailure)
                    {
                        m_objContext.TerminalDataCache.Set("core_DispenseError", true, GetType());
                        PrintDispenseInfoAfterDispenseFail();
                        HandleDispenseFail();
                        m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                    }
                    else
                    {
                        // close transaction if card is inserted and card reader is error or card is not inside after dispense
                        if (!CheckIDCStatus())
                        {
                            m_objContext.LogJournal("IDC ERROR IN DISPENSE CASH", LogSymbol.Alert);
                            m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReversalForIDCErrorInDispense", TextCategory.s_journal), LogSymbol.Alert);
                            HandleDispenseFail();
                            m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                        }
                        else
                        {
                            m_objContext.NextCondition = EventDictionary.s_EventConfirm;
                            m_withdrawalSecurityWatch.Restart();
                        }
                    }
                }
                else
                {
                    m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_DispenseAmountNull", TextCategory.s_journal), LogSymbol.Alert);
                    m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_ReversalForDispenseFail", TextCategory.s_journal), LogSymbol.Alert);

                    // update transaction record
                    if (null != m_objContext.CurrentRecord)
                    {
                        m_objContext.CurrentRecord.CashDevErrorFlag = CashDevErrorFlag.s_DenominateFail;     // 1-取款配钞失败
                        m_objContext.CurrentRecord.CashDevErrorCode = "Amount is null";
                        m_objContext.ReversalService.SetReversalFlagAndData();
                        m_objContext.CurrentRecord.Submit();
                    }

                    if (SwitchUIState(m_objContext.MainUI, "AmountError", 3000))
                        WaitSignal();

                    m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                }
            }

        _exit:
            Log.Action.LogDebug("Leave action: DispenseCash");
            return emBusActivityResult_t.Success;
        }
        #endregion

        /// <summary>
        /// Get the dispense note result by give cash unit information (before change and after change)
        /// </summary>
        /// <param name="tmpBeforeCashUnitInfo"></param>
        /// <param name="tmpAfterCashUnitInfo"></param>
        /// <param name="tmpDictCassetteNoteResult"></param>
        private void GetDispenseNoteResult(List<CashUnitInfo> tmpBeforeCashUnitInfo,
                                                                    List<CashUnitInfo> tmpAfterCashUnitInfo,
                                                                    ref Dictionary<string, int> tmpDictCassetteNoteResult, ref Dictionary<string, int> tmpDictCassetteRejectResult)
        {
            int dispenseNoteNum = 0;
            int rejectNoteNum = 0;
            try
            {
                if (null == tmpDictCassetteNoteResult)
                {
                    tmpDictCassetteNoteResult = new Dictionary<string, int>();
                }
                if (null == tmpDictCassetteRejectResult)
                {
                    tmpDictCassetteRejectResult = new Dictionary<string, int>();
                }
                tmpDictCassetteNoteResult.Clear();
                tmpDictCassetteRejectResult.Clear();

                //没有钱箱信息传入时直接跳出
                if (tmpAfterCashUnitInfo == null)
                {
                    return;
                }
                foreach (var item in tmpBeforeCashUnitInfo)
                {
                    var afterItem = tmpAfterCashUnitInfo.Find(unitInfo =>
                    {
                        if (unitInfo.UnitID == item.UnitID)
                        {
                            return true;
                        }

                        return false;
                    });

                    if (afterItem == null)
                    {
                        continue;
                    }

                    dispenseNoteNum = (item.Count + item.RejectCount) - (afterItem.Count + afterItem.RejectCount);
                    rejectNoteNum = afterItem.RejectCount - item.RejectCount;

                    tmpDictCassetteNoteResult.Add(item.UnitID, dispenseNoteNum);
                    tmpDictCassetteRejectResult.Add(item.UnitID, rejectNoteNum);
                }
            }
            catch (System.Exception ex)
            {
                Trace.TraceError(ex.Message);
                Log.XdcTrace.LogFatal("*Exception: GetDispenseNoteResult Error!", XdcTraceLogKeyItem.m_strException, ex);
            }

            return;
        }
        /*
        /// <summary>
        /// Set cassettes information to be dispensed
        /// </summary>
        /// <param name="argDictCassetteNoteAllot">The allocation result with dictionary container</param>
        /// <param name="argDictCassetteNoteCommand">The cassette notes to be dispensed</param>
        public void SetCassettesToBeDispensed(Dictionary<string, CassettesNoteAllot> argDictCassetteNoteAllot,
                                                                           ref Dictionary<string, int> argDictCassetteNoteCommand)
        {
            try
            {
                argDictCassetteNoteCommand.Clear();

                foreach (var item in argDictCassetteNoteAllot)
                {
                    if (argDictCassetteNoteCommand.ContainsKey(item.Value.cassetteId))
                    {
                        continue;
                    }

                    argDictCassetteNoteCommand.Add(item.Value.cassetteId, item.Value.allotNum);
                }

            }
            catch (System.Exception ex)
            {
                Trace.TraceError(ex.Message);
                Log.XdcTrace.LogFatal("Exception: SetCassettesToBeDispensed Error!",
                                       XdcTraceLogKeyItem.m_strException, ex);
                return;
            }

            return;

        }
         * */

        #region define function
        // handle reject cash
        public virtual void HandleRejectCash()
        {
            // show "rejecting cash"
            SwitchUIState(m_objContext.MainUI, "RejectingCash", -int.MaxValue);

            // reject cash
            bool bResult = ExecuteDevCommand(CashDispenserCmdIndex.s_RejectCmd);

            if (-316 != m_CDMErrorCode)
            {
                if (!bResult)
                {
                    //record exception id
                    if (m_objContext.EnableOCRForCDM)
                    {
                        StatusInfo statusInfo;
                        m_objContext.CashDispenser.GetStatusInfo(out statusInfo, m_devTimeout);
                        if (!statusInfo.IntermediateStackerIsEmpty || !statusInfo.OutInPositionIsEmpty || !statusInfo.TransportIsEmpty)
                        {
                            if (null != m_objContext.CurrentRecord || InsertTransRecord())
                            {
                                m_objContext.TerminalDataCache.Set(DataDictionary.s_coreOCRExceptionTransID, m_objContext.CurrentRecord.ID, this.GetType());
                            }
                        }
                    }
                }

                //add ocr method
                //*****gyb add at 20130419 begin*****
                if (m_objContext.EnableOCRForCDM)
                {
                    //save ocr info
                    if (null != m_objContext.CurrentRecord || InsertTransRecord())
                    {
                        m_objContext.OCRService.SaveOCRInfo(m_objContext.CurrentRecord.ID, OCRCommandType.Reject);
                    }
                }
                //*****gyb add at 20130419 end*****
            }

            if (bResult)
            {
                bResult = SwitchUIState(m_objContext.MainUI, "RejectCashForHardwareError", 3000);
            }
            else
            {
                bResult = SwitchUIState(m_objContext.MainUI, "RejectCashFail", 3000);
            }

            if (bResult) WaitSignal();
        }

        // validate withdrawal flow
        public virtual bool ValidateWithdrawalFlow()
        {
            // valid dispense cash
            object isDenominated = null;
            m_objContext.TransactionDataCache.Get(DataDictionary.s_coreIsAlreadyDenominated, out isDenominated, GetType());

            object isSendedWithdrawalRequest = null;
            m_objContext.TransactionDataCache.Get(DataDictionary.s_coreIsAlreadySentWithdrawalRequest, out isSendedWithdrawalRequest, GetType());

            object IsUnpackedWithdrawalTrans = null;
            m_objContext.TransactionDataCache.Get(DataDictionary.s_coreIsAlreadyUnpackedWithdrawalTrans, out IsUnpackedWithdrawalTrans, GetType());

            if (null != isDenominated && null != isSendedWithdrawalRequest && null != IsUnpackedWithdrawalTrans &&
                (bool)isDenominated && (bool)isSendedWithdrawalRequest && (bool)IsUnpackedWithdrawalTrans)
            {
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreIsAlreadyDenominated, false, GetType());

                // validate backup withdrawal and dispense amount
                object backupWithdrawalAmount = null;
                m_objContext.TransactionDataCache.Get("7AD00161-2FB1-42FE-8592-41895E899CCE", out backupWithdrawalAmount, GetType());
                if (null != backupWithdrawalAmount)
                {
                    try
                    {
                        if (int.Parse(backupWithdrawalAmount.ToString()) != m_dispenseWithDrawalAmount)
                        {
                            m_objContext.LogJournal("BACKUP AND DISPENSE AMOUNT UNMATCH", LogSymbol.Alert);
                            return false;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        m_objContext.LogJournal("INVALID BACKUP OR DISPENSE AMOUNT", LogSymbol.Alert);
                        Log.Action.LogErrorFormat("Invalid backup or dispense amount: {0}, {1}", backupWithdrawalAmount, m_dispenseWithDrawalAmount);
                        Log.Action.LogError(ex?.ToString());
                        return false;
                    }
                }
            }
            else
            {
                m_objContext.LogJournal("CANNOT DISPENSE CASH FOR FLOW ERROR", LogSymbol.Alert);
                Log.Action.LogErrorFormat("Cannot dispense cash for flow error, IsDenominated: {0}, IsSendedWithdrawalRequest: {1}, IsUnpackedWithdrawalTrans: {2}", isDenominated, isSendedWithdrawalRequest, IsUnpackedWithdrawalTrans);
                m_objContext.NextCondition = EventDictionary.s_EventHardwareError;
                return false;
            }

            return true;
        }

        // handle dispense fail
        public virtual void HandleDispenseFail()
        {
            // update transaction record
            if (null != m_objContext.CurrentRecord)
            {
                m_objContext.CurrentRecord.CashDevErrorFlag = CashDevErrorFlag.s_DenominateFail;     // 1-取款配钞失败
                m_objContext.CurrentRecord.CashDevErrorCode = m_CDMErrorCode.ToString();
                m_objContext.ReversalService.SetReversalFlagAndData();
                m_objContext.CurrentRecord.Submit();
            }

            // dispense cash fail, reject cash
            HandleRejectCash();

            // print cassette remain count
            PrintCassetteRemainCount();
        }

        public virtual int GetRejectCount(string argUnitID, int argOutCount)
        {
            try
            {
                int rejectCount = m_dicCassetteIDAndCountBefore[argUnitID] - m_dicCassetteIDAndCountAfter[argUnitID] - argOutCount;
                if (rejectCount < 0)
                {
                    rejectCount = 0;
                }

                return rejectCount;
            }
            catch
            {
                return 0;
            }
        }

        public virtual void PrintDispenseInfoAfterDispenseFail()
        {
            try
            {
                string dispenseInfo = string.Empty;
                List<CashUnitInfo> cashUnitInfo = GetCashUnitInfo();
                foreach (CashUnitInfo item in cashUnitInfo)
                {
                    if (CashUnitType.BILL == item.UseType || CashUnitType.RECYCLING == item.UseType)
                    {
                        int retractCount = m_dicCassetteIDAndCountBefore[item.UnitID] - m_dicCassetteIDAndCountAfter[item.UnitID];
                        if (0 != retractCount)
                        {
                            if (string.IsNullOrWhiteSpace(dispenseInfo))
                            {
                                dispenseInfo = string.Format("{0}.{1}.{2}.{3}", item.UnitID, item.DenoValue, 0, retractCount);
                            }
                            else
                            {
                                dispenseInfo += string.Format("/{0}.{1}.{2}.{3}", item.UnitID, item.DenoValue, 0, retractCount);
                                m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_DispenseInfo", TextCategory.s_journal) + dispenseInfo);
                                dispenseInfo = string.Empty;
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(dispenseInfo))
                {
                    m_objContext.LogJournal(m_objContext.CurrentJPTRResource.LoadString("IDS_DispenseInfo", TextCategory.s_journal) + dispenseInfo);
                }
            }
            catch (System.Exception ex)
            {
                Log.Action.LogError(ex?.ToString());
            }
        }
        #endregion
    }
}