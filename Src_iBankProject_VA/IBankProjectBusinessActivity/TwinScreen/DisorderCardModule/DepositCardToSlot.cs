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
using RequestTrans;
using RestfulServiceProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTMBusinessServiceProtocol;
using VTMBusinessServiceProtocol.DataGateway;

namespace TwinScreen
{
    [GrgActivity("{60F65345-4852-455F-8327-8DCBF17EBB91}",
                Name = "DepositCardToSlot",
                Description = "DepositCardToSlot Des",
                NodeNameOfConfiguration = "DepositCardToSlot",
                Catalog = "DepositCardToSlot",
                Author = "  ",
                ForwardTargets = new string[] { EventDictionary.s_EventContinue })]
    public class BusinessActivityDepositCardToSlot : eCATBusinessTwinScreenActivityBase
    {

        #region method of creating
        /// <summary>
        /// 实现Create接口
        /// </summary>
        /// <returns></returns>
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityDepositCardToSlot() as IBusinessActivity;
        }
        #endregion

        #region constructor
        /// <summary>
        /// 构造函数,防止外部调用
        /// </summary>
        protected BusinessActivityDepositCardToSlot()
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
            TwinUI = "123";
            return base.InnerInit();
        }
        bool stopaddcard = false;
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
                        stopaddcard = false;
                        HandleFun1();
                        break;
                    case TwinPressEvent.s_FunOK:
                        stopaddcard = true;
                        Context.TwinScreen.UnPromptBox();
                        Context.TwinScreen.PromptBox(GetResValue("Processing"));
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
            if (!IsDisorderCardDispenser)
            {
                Context.TwinScreen.SetValue("FUN_1,", "0,0" + TwinSeparator.FieldSeparator, true);
                Context.TwinScreen.SetValue("STATIC_TXTBOX,", GetResValue("NODEVICE"));
            }
            Context.TwinScreen.UnPromptBox();
        }

        protected override void InnerExit()
        {
            base.InnerExit();
        }

        #endregion
        string _pan = string.Empty;
        List<DisorderCardInfo> DisorderCardInfoList = new List<DisorderCardInfo>();
        #region handle event
        /// <summary>
        /// 存卡
        /// </summary>
        protected virtual void HandleFun1()
        {
            Log.Action.LogDebug("Click Fun1");
            DepositCardToSlotAsyn();
        }

        private async Task DepositCardToSlotAsyn()
        {
            await Task.Run(() =>
           {
               try
               {
                   Context.TwinScreen.PromptBox(GetResValue("Processing"));
                   string prompt_msg = GetResValue("DepositCardFailed");
                   if (DepositCardToSlot(out DisorderCardInfoList, ref prompt_msg))
                   {
                       VTMContext.TransactionDataCache.Set("proj_SEM_Process", "TRUE", GetType());
                       var GetSlotInfoRet = VTMContext.CardDispenser.GetSlotInfo(out var slotInfo, m_devTimeout);
                       if (GetSlotInfoRet.IsSuccess)
                       {
                           int emptySlot = 0;
                           int present = 0;
                           var slotStatus = slotInfo.SlotStatus.Split(new[] { ',' }, StringSplitOptions.None);
                           for (int i = 0; i < slotStatus.Length; i++)
                           {
                               if (!"PRESENT".Equals(slotStatus[i]))
                               {
                                   emptySlot++;
                               }
                               else
                               {
                                   present++;
                               }
                           }
                           Context.TwinScreen.SetValue("STATIC_TXTBOX,", string.Format(GetResValue("DisorderCardModuleEmptySlot"), emptySlot));
                           Context.TwinScreen.SetValue("STATIC_TXTBOX,", string.Format(GetResValue("DisorderCardModuleNoEmptySlot"), present));
                           Context.TwinScreen.SetValue("STATIC_TXTBOX,", string.Format(GetResValue("DepositCardSuccess"), DisorderCardInfoList.Count));
                       }
                   }
                   else
                   {
                       VTMContext.TransactionDataCache.Set("proj_SEM_Process", "FLASE", GetType());
                   }
                   if (DisorderCardInfoList.Count > 0)
                   {
                       VTMContext.VTMDataGateway.AddDisorderCardInfo(DisorderCardInfoList);
                       sendAddCardMasage();
                   }
                   else {
                       prompt_msg = GetResValue("DepositCardFailed");
                   }
                   Context.TwinScreen.SetValue("STATIC_PROMPT,", prompt_msg + TwinSeparator.FieldSeparator);
                   Context.TwinScreen.UnPromptBox();
               }
               catch(Exception ex)
               {
                   Log.Action.LogError(ex.ToString());
                   Context.TwinScreen.UnPromptBox();
               }
           });
        }

        public bool DepositCardToSlot(out List<DisorderCardInfo> DisorderCardInfoList, ref string prompt_msg)
        {
            DisorderCardInfoList = new List<DisorderCardInfo>();
            try
            {
                var inputCardNumber = Context.TwinScreen.GetValue("INPUT_CARD_NUMBER,");
                if (string.IsNullOrWhiteSpace(inputCardNumber) || int.Parse(inputCardNumber) <= 0)
                {
                    prompt_msg = GetResValue("Invalidaddcardnumber");
                    
                }
                else
                {
                    var GetSlotInfoRet = VTMContext.CardDispenser.GetSlotInfo(out var slotInfo, m_devTimeout);
                    Dictionary<int, bool> slotNumberList = new Dictionary<int, bool>();
                    if (GetSlotInfoRet.IsSuccess)
                    {
                        var slotStatus = slotInfo.SlotStatus.Split(new[] { ',' }, StringSplitOptions.None);
                        for (int i = 0; i < slotStatus.Length; i++)
                        {
                            if (!"PRESENT".Equals(slotStatus[i]))
                            {
                                slotNumberList.Add(i, false);
                            }
                        }

                        List<CardUnitInfo> carUnitList = null;
                        VTMContext.CardDispenser.GetCardUnitInfo(out carUnitList);
                        if (null != carUnitList && carUnitList.Count > 0)
                        {
                            Context.TwinScreen.UnPromptBox();
                            Context.TwinScreen.PromptOK(GetResValue("EndAddCard"));
                            foreach (CardUnitInfo cardUnit in carUnitList)
                            {
                                if (cardUnit.Count < 1 || cardUnit.Status== "EMPTY"|| cardUnit.Status == "MISSING")
                                {
                                    continue;
                                }
                                if (cardUnit.Type.Equals(emCRDType.SupplyBin))
                                {
                                    if (cardUnit.CardName.Equals("CONTACTLESS"))
                                    {
                                        IsContactlessCard = true;
                                    }
                                    else { IsContactlessCard = false; }
                                    for (int i = 0; i < cardUnit.Count; i++)
                                    {
                                        if (stopaddcard)//主动请求停止加卡
                                        {
                                            Log.Action.LogDebug("Stop Add Card!");
                                            prompt_msg = GetResValue("DepositCardSuccess");
                                            Context.TwinScreen.UnPromptBox();
                                            return true;
                                        }
                                        if (slotNumberList.Count > 0)
                                        {
                                            if (DisorderCardInfoList.Count >= int.Parse(inputCardNumber))
                                            {
                                                prompt_msg = GetResValue("DepositCardSuccess");
                                                Context.TwinScreen.UnPromptBox();
                                                return true;
                                            }
                                            var emRet = VTMContext.CardDispenser.DispenseCard((SupplyBin)cardUnit.Number, (DispenseType)0, m_devTimeout);
                                            if (emRet.IsSuccess)
                                            {
                                                if (GetPAN() && !string.IsNullOrWhiteSpace(_pan))
                                                {
                                                    var SlotDepositRet = VTMContext.CardDispenser.SlotDeposit((ushort)(slotNumberList.FirstOrDefault().Key), m_devTimeout);
                                                    if (SlotDepositRet.IsSuccess)
                                                    {
                                                        DisorderCardInfoList.Add(new DisorderCardInfo
                                                        {
                                                            cardID = Guid.NewGuid().ToString("N"),
                                                            cardNumber = _pan,
                                                            depositTime = DateTime.Now.ToString("yyyy-MM-dd"),
                                                            cardType=cardUnit.CardName,
                                                            slotNumber= slotNumberList.FirstOrDefault().Key
                                                        });
                                                        slotNumberList.Remove(slotNumberList.FirstOrDefault().Key);
                                                    }
                                                    _pan = string.Empty;
                                                }
                                                else
                                                {
                                                    VTMContext.CardDispenser.RetainCard();
                                                }
                                            }
                                            else {
                                                Context.TwinScreen.UnPromptBox();
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            prompt_msg = GetResValue("DepositCardSuccess");
                                            Context.TwinScreen.UnPromptBox();
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Context.TwinScreen.UnPromptBox();
                return false;
            }
            catch (Exception ex)
            {
                prompt_msg = GetResValue("DepositCardFailed");
                Log.Action.LogError(ex.ToString());
                Context.TwinScreen.UnPromptBox();
                return false;
            }
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

        public bool sendAddCardMasage()
        {
            try
            {
                Log.Action.LogDebug("***sendAddCardMasage***");
                VTMContext.TransactionDataCache.Set("proj_SEM_TerminalId", VTMContext.TerminalConfig.Terminal.ATMNumber, GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_SerialNumber", Guid.NewGuid().ToString("N"), GetType());
                VTMContext.TransactionDataCache.Set("proj_SEM_AddCardList",DisorderCardInfoList, GetType());
                string responseCode;
                VTMContext.RestfulService.SendMessage("AddCard", out responseCode, MessageFormat.JSON);
            }
            catch(Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return true;
        }
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
