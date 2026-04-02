using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using CardReaderDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessMaintenanceActivity;
using eCATBusinessServiceProtocol;
using EmvServiceProtocol;
using FlashDeviceProtocol;
using LogProcessorService;
using Newtonsoft.Json;
using RequestTrans;
using RestfulServiceProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VTMBusinessServiceProtocol;
using VTMBusinessServiceProtocol.DataGateway;

namespace TwinScreen
{
    [GrgActivity("{F62F8CAE-D5D8-41C8-888F-74B6498A879F}",
                Name = "ClearDisorderCard",
                Description = "ClearDisorderCard Des",
                NodeNameOfConfiguration = "ClearDisorderCard",
                Catalog = "ClearDisorderCard",
                Author = "  ",
                ForwardTargets = new string[] { EventDictionary.s_EventContinue })]
    public class BusinessActivityClearDisorderCard : eCATBusinessTwinScreenActivityBase
    {

        #region method of creating
        /// <summary>
        /// 实现Create接口
        /// </summary>
        /// <returns></returns>
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityClearDisorderCard() as IBusinessActivity;
        }
        #endregion

        #region constructor
        /// <summary>
        /// 构造函数,防止外部调用
        /// </summary>
        protected BusinessActivityClearDisorderCard()
        {

        }
        #endregion

        #region override methods of base

        /// <summary>
        /// 处理环境初始值
        /// </summary>
        /// <returns></returns>
        protected override bool InnerInit()
        {
            //init fields
            TwinUI = "124";
            return base.InnerInit();
        }
        bool IsContactlessCard = false;
        /// <summary>
        /// 双屏、读卡器设备事件处理
        /// </summary>
        /// <param name="argIDev">设备</param>
        /// <param name="argObjArg">事件参数</param>
        /// <returns></returns>
        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService argIDev, DeviceEventArg argObjArg)
        {
            Debug.Assert(null != argIDev && null != argObjArg);
            Debug.Assert(null != Context);

            emBusiCallbackResult_t result = base.InnerOnDevEvtHandle(argIDev, argObjArg);
            if (emBusiCallbackResult_t.Swallowd == result)
            {
                return result;
            }
            if (argObjArg.Source == Context.TwinScreen.HostedDevice)
            {
                FlashDeviceEventArg arg = argObjArg as FlashDeviceEventArg;
                Debug.Assert(arg != null);
                SetUICache(arg.Key);
                switch (arg.Key)
                {
                    case TwinPressEvent.s_Fun_1:
                        Context.TwinScreen.SetValue("STATIC_PROMPT,", string.Empty + TwinSeparator.FieldSeparator);
                        HandleFun1();
                        break;
                    default:
                        HandleDefaultKeyPress(arg.Key);
                        break;
                }
            }
            return emBusiCallbackResult_t.Bypass;
        }
        #endregion

        #region page initialize

        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override void HandleInitPage()
        {
            Context.TwinScreen.PromptBox(GetResValue("Loading"));
            if (VTMContext.CardDispenser != null)
            {
                var devResult = VTMContext.CardDispenser.GetCapabilitiesInfo(out var argCap);
                IsDisorderCardDispenser = devResult.IsSuccess && argCap.IsDisorderCardDispenser == 1;
            }
            Context.TwinScreen.ChangeUI(TwinUI);
        }

        /// <summary>
        /// 设置界面元素
        /// </summary>
        protected override void HandleSetPageValue()
        {
            base.HandleSetPageValue();
            //if (!IsDisorderCardDispenser)
            //{
            //    Context.TwinScreen.SetValue("FUN_1,", "0,0" + TwinSeparator.FieldSeparator, true);
            //    Context.TwinScreen.SetValue("STATIC_TXTBOX,", GetResValue("NODEVICE"));
            //}
            Context.TwinScreen.UnPromptBox();
        }

        protected override void InnerExit()
        {
            base.InnerExit();
        }

        #endregion
        int _day = 0;
        string CardNumer = string.Empty;
        string _pan = string.Empty;
        List<DisorderCardInfo> DisorderCardInfoList = new List<DisorderCardInfo>();
        #region handle event
        /// <summary>
        /// 存卡
        /// </summary>
        protected virtual void HandleFun1()
        {
            Context.TwinScreen.PromptBox(GetResValue("Loading"));
            Log.Action.LogDebug("Click Fun1");
            var ClearType=Context.TwinScreen.GetValue("INPUT_ClearType");
            string cardIdList = string.Empty;
            List<string> CardNumberList = new List<string>();
            if (string.IsNullOrEmpty(ClearType) || ClearType.Equals("CARDNUMBER"))
            {
                cardIdList = GetClearCardData(1,ref CardNumberList);
            } else if (ClearType.Equals("ALL"))
            {
                cardIdList = GetClearCardData(3,ref CardNumberList);
            }
            else {
                cardIdList = GetClearCardData(2,ref CardNumberList);
            }
            if (cardIdList.Length > 1&& CardNumberList.Count>0)
            {
                Context.TwinScreen.SetValue("STATIC_TXTBOX,", "Clear Card Success!");
                Context.TwinScreen.SetValue("STATIC_TXTBOX,", "The card numbers are as follows:");
                foreach (var cardnumber in CardNumberList)
                {
                    Context.TwinScreen.SetValue("STATIC_TXTBOX,", cardnumber);
                }
                delDatabaseData(cardIdList);
                Log.Action.LogInfo("Clear Card Success!");
            }
            Context.TwinScreen.UnPromptBox();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clearType">1：卡号 2：存入时间</param>
        /// <returns></returns>
        public string GetClearCardData(int clearType,ref List<string>CardNumberList)
        {
            var DBCardInfo = VTMContext.VTMDataGateway.QueryDisorderCardInfo();
            string cardIdList = "(";
            string SQLStr = "select * from SEMTransDetail {0}";
            try {
                if (DBCardInfo != null && DBCardInfo.Count > 0)
                {
                    if (clearType == 2)
                    {
                        _day = int.Parse(Context.TwinScreen.GetValue("INPUT_DAYS"));

                        DBCardInfo = DBCardInfo.Where(m => (DateTime.Now.Subtract(DateTime.Parse(m.depositTime))).Days >= _day)?.ToList();
                        if (DBCardInfo.Count <= 0)
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", "No eligible data!" + TwinSeparator.FieldSeparator);
                            return cardIdList;
                        }
                        foreach (var cardmode in DBCardInfo)
                        {
                            if (cardIdList.Length > 1)
                            {
                                cardIdList += ",'" + cardmode.cardID + "'";
                            }
                            else
                            {
                                cardIdList += "'" + cardmode.cardID + "'";
                            }
                        }
                        if (cardIdList.Length > 1)
                        {
                            SQLStr = string.Format(SQLStr, string.Format("where CardID in {0}", cardIdList + ")"));
                            if (GetHostCardInfo(ref DBCardInfo, SQLStr))
                            {
                                cardIdList = "(";
                                foreach (var mode in DBCardInfo)
                                {
                                    var emRet = VTMContext.CardDispenser.SlotDispense((ushort)(mode.slotNumber), 0);
                                    if (emRet.IsSuccess)
                                    {
                                        emRet = VTMContext.CardDispenser.RetainCard();
                                        if (cardIdList.Length > 1)
                                        {
                                            cardIdList += ",'" + mode.cardID + "'";
                                        }
                                        else
                                        {
                                            cardIdList += "'" + mode.cardID + "'";
                                        }
                                        CardNumberList.Add(mode.cardNumber);
                                        if (!emRet.IsSuccess)
                                        {
                                            Log.Action.LogError("RetainCard Fail!");
                                            VTMContext.CardDispenser.Reset();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Log.Action.LogError("SlotDispense Fail!");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                cardIdList = "(";
                            }
                        }
                    }
                    else if (clearType == 1)
                    {
                        var inputCardNumber = Context.TwinScreen.GetValue("INPUT_CARD_NUMBER,");
                        if (string.IsNullOrWhiteSpace(inputCardNumber))
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", GetResValue("Invalidaddcardnumber") + TwinSeparator.FieldSeparator);
                        }
                        else
                        {
                            if (GetHostCardInfo(ref DBCardInfo, string.Format(SQLStr, string.Format("where cardNumber='{0}'", inputCardNumber))))
                            {
                                cardIdList = "(";
                                foreach (var mode in DBCardInfo)
                                {
                                    var emRet = VTMContext.CardDispenser.SlotDispense((ushort)(mode.slotNumber), 0);
                                    if (emRet.IsSuccess)
                                    {
                                        if (mode.cardType.Equals("CONTACTLESS"))
                                        {
                                            IsContactlessCard = true;
                                        }
                                        else
                                        {
                                            IsContactlessCard = false;
                                        }
                                        if (GetPAN())
                                        {
                                            if (_pan.Equals(inputCardNumber))
                                            {
                                                if (cardIdList.Length > 1)
                                                {
                                                    cardIdList += ",'" + mode.cardID + "'";
                                                }
                                                else
                                                {
                                                    cardIdList += "'" + mode.cardID + "'";
                                                }
                                                CardNumberList.Add(mode.cardNumber);
                                                emRet = VTMContext.CRDReader.EjectCard();
                                                if (emRet.IsFailure)
                                                {
                                                    VTMContext.CardDispenser.RetainCard();
                                                }

                                            }
                                            else
                                            {
                                                Log.Action.LogInfo("The input card number is inconsistent, and the card removal fails!");
                                                Context.TwinScreen.SetValue("STATIC_PROMPT,", "The input card number is inconsistent, and the card removal fails!" + TwinSeparator.FieldSeparator);
                                                VTMContext.CardDispenser.SlotDeposit((ushort)mode.slotNumber);
                                            }
                                        }
                                        else
                                        {
                                            if (cardIdList.Length > 1)
                                            {
                                                cardIdList += ",'" + mode.cardID + "'";
                                            }
                                            else
                                            {
                                                cardIdList += "'" + mode.cardID + "'";
                                            }
                                            CardNumberList.Add(mode.cardNumber);
                                            VTMContext.CRDReader.RetainCard();
                                            Log.Action.LogError("GetPAN Fail!");
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Log.Action.LogError("SlotDispense Fail!");
                                        Context.TwinScreen.SetValue("STATIC_PROMPT,", "SlotDispense Fail!" + TwinSeparator.FieldSeparator);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (GetHostCardInfo(ref DBCardInfo, string.Format(SQLStr, "where Status==-1")))
                        {
                            cardIdList = "(";
                            foreach (var mode in DBCardInfo)
                            {
                                var emRet = VTMContext.CardDispenser.SlotDispense((ushort)(mode.slotNumber), 0);
                                if (emRet.IsSuccess)
                                {
                                    emRet = VTMContext.CardDispenser.RetainCard();
                                    if (cardIdList.Length > 1)
                                    {
                                        cardIdList += ",'" + mode.cardID + "'";
                                    }
                                    else
                                    {
                                        cardIdList += "'" + mode.cardID + "'";
                                    }
                                    CardNumberList.Add(mode.cardNumber);
                                    if (!emRet.IsSuccess)
                                    {
                                        Log.Action.LogError("RetainCard Fail!");
                                        VTMContext.CardDispenser.Reset();
                                        break;
                                    }
                                }
                                else
                                {
                                    Log.Action.LogError("SlotDispense Fail!");
                                    Context.TwinScreen.SetValue("STATIC_PROMPT,", "SlotDispense Fail!" + TwinSeparator.FieldSeparator);
                                    break;
                                }
                            }
                        }
                    }
                }
                else {
                    Context.TwinScreen.SetValue("STATIC_PROMPT,", "The database no have card data" + TwinSeparator.FieldSeparator);
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return cardIdList;
        }

        public void delDatabaseData(string cardIdlist)
        {
            try {
                VTMContext.VTMDataGateway.DelDisorderCardInfo(cardIdlist+")");
                Log.Action.LogDebug("***Send DelCardInfo***");
                VTMContext.TransactionDataCache.Set("proj_SEM_SerialNumber", Guid.NewGuid().ToString("N"), GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_TerminalId", VTMContext.TerminalConfig.Terminal.ATMNumber, GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_CardIdList", cardIdlist+")", GetType());
                string responseCode;
                VTMContext.RestfulService.SendMessage("DelCardInfo", out responseCode, MessageFormat.JSON);
            }
            catch(Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
        }
        public bool GetHostCardInfo(ref List<DisorderCardInfo> cardList,string SQL)
        {
            try
            {
                cardList = new List<DisorderCardInfo>();
                Log.Action.LogDebug("***Send QureyCardInfoList***");
                VTMContext.TransactionDataCache.Set("proj_SEM_SerialNumber", Guid.NewGuid().ToString("N"), GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_TerminalId", VTMContext.TerminalConfig.Terminal.ATMNumber, GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_SQLStr", SQL, GetType());
                string responseCode;
                VTMContext.RestfulService.SendMessage("QureyCardInfoList", out responseCode, MessageFormat.JSON);
                if (responseCode.Equals("0"))
                {
                    VTMContext.TransactionDataCache.Get("proj_SEM_cardInfoList", out var slotNumber);
                    if (slotNumber != null)
                    {
                        cardList = JsonConvert.DeserializeObject<List<DisorderCardInfo>>(slotNumber?.ToString());
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return false;
        }
        public bool getlessCardPan()
        {
            try
            {
                Log.Action.LogError("***getlessCardPan***");
                VTMContext.CRDReader.MoveCard(3);
                GrgCmdReadRawDataOutArg objOutArg;
                DevResult devResult = VTMContext.CRDReader.ReadRawData(out objOutArg);
                VTMContext.CRDReader.MoveCard(4);
                VTMContext.CRDReader.CancelReadRawData();
                if (devResult.IsFailure || objOutArg == null)
                {
                    return false;
                }
                else
                {
                    _pan = objOutArg.StrATR;
                    Log.Action.LogInfo("StrATR:" + _pan);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 获取卡号
        /// </summary>
        /// <returns></returns>
        public bool GetPAN()
        {
            if (null != VTMContext.CRDReader)
            {
                try
                {
                    _pan = null;
                    if (IsContactlessCard)
                    {
                        return getlessCardPan();
                    }
                    GrgCmdReadRawDataOutArg objOutArg;
                    DevResult devResult = VTMContext.CRDReader.ReadRawData(out objOutArg);
                    if (devResult.IsFailure || objOutArg == null)
                    {
                        return false;
                    }
                    else
                    {
                        //IC 发卡
                        VTMContext.CardType = VTMContext.CRDReader.GetCardType();
                        Log.Action.LogError("card type" + VTMContext.CardType);
                        VTMContext.CRDReader.CancelReadRawData();
                        //end IC 发卡 
                        if (!SaveRawData(objOutArg.StrTrack1, objOutArg.StrTrack2, objOutArg.StrTrack3))
                        {
                            if (VTMContext.CardType == emCardType.SimplexCard || VTMContext.CardType == emCardType.ComplexCard)
                            {
                                if (GetIcPan())
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(_pan))
                            {
                                return true;
                            }
                            if (VTMContext.CardType == emCardType.SimplexCard || VTMContext.CardType == emCardType.ComplexCard)
                            {
                                if (GetIcPan())
                                {
                                    return true;
                                }
                            }
                            else
                            {
    
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Action.LogError(ex.ToString());
                }
                //finally
                //{
                //    VTMContext.CRDReader.CancelReadRawData();
                //}
            }
            return false;
        }
        /// <summary>
        /// 芯片卡处理
        /// </summary>
        /// <returns></returns>
        public bool GetIcPan()
        {
            try
            {
                short shRet = VTMContext.EmvService4CRDReader.EmvBuildAppCandidates();
                if (shRet == ICFuncReturnValueType.Success)
                {
                    short appCount = VTMContext.EmvService4CRDReader.EmvGetAppCount();
                    if (appCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        shRet = VTMContext.EmvService4CRDReader.EmvApplicationSelected(-1);
                        if (shRet == ICFuncReturnValueType.Success)
                        {
                            shRet = VTMContext.EmvService4CRDReader.EmvApplicationInitialised();
                            if (shRet == ICFuncReturnValueType.Success)
                            {
                                shRet = VTMContext.EmvService4CRDReader.EmvApplicationLoaded();
                                if (shRet == ICFuncReturnValueType.Success)
                                {
                                    string track1 = VTMContext.EmvService4CRDReader.EmvGetTag("9F1F");

                                    // get IC track2
                                    string track2 = VTMContext.EmvService4CRDReader.EmvGetTag("57");

                                    if (string.IsNullOrEmpty(track2))
                                    {
                                        // get IC track2
                                        track2 = VTMContext.EmvService4CRDReader.EmvGetTag("5A");
                                    }
                                    if (!string.IsNullOrEmpty(track2))
                                    {
                                        track2 = track2.ToLowerInvariant();
                                        track2 = track2.Replace('d', '=');
                                        track2 = track2.TrimEnd('f');
                                    }
                                    if (SaveRawData(track1, track2, null))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return false;
        }

        private bool SaveRawData(string track1,string track2,string track3)
        {

            FitItem fitItem = null;

            MatchResult matchResult = m_objContext.SettingOfCardAndPB.CheckCardType(track1, track2, null, out fitItem);

            if (MatchResult.Success == matchResult)
            {
                _pan = fitItem.PAN;
                return true;
            }
            return false;
        }
        #endregion
        #region define function

        protected VTMBusinessContext VTMContext
        {
            get
            {
                return (VTMBusinessContext)m_objContext;
            }
        }

        protected bool IsDisorderCardDispenser { get; set; }

        #endregion
    }
}
