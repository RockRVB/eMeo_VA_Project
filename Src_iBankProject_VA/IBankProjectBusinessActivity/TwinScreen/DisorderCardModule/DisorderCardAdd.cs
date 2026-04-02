using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using DataFormatterServiceProtocol;
using DevServiceProtocol;
using eCATBusinessMaintenanceActivity;
using eCATBusinessServiceProtocol;
using FeelViewServiceProtocol;
using FlashDeviceProtocol;
using LogProcessorService;
using NetworkServiceProtocol;
using ResourceManagerProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using VTMBusinessServiceProtocol;
using VTMBusinessServiceProtocol.DataGateway;
using VTMModelLibrary;

namespace TwinScreen
{
    [GrgActivity("{51D5F7A0-0051-4F71-8EF0-BABC19DAFE1D}",
               Name = "DisorderCardAdd",
               Description = "DisorderCardAdd Des",
               NodeNameOfConfiguration = "DisorderCardAdd",
               Catalog = "DisorderCardAdd",
               ForwardTargets = new string[] { })]
    public class DisorderCardAdd : eCATBusinessTwinScreenActivityBase
    {
        private static string m_DevError;
        private static string m_CleanCard;
        private static string m_CleanCardFailed;
        private static string m_ClenaCardSuccess;
        private static string m_AddCard;
        private static string m_ClearRetainBin;
        private static string m_AddCardFailed;
        private static string m_AddCardSuccess;
        private static string m_AddCardLimited;
        private static string m_AfterCleanCardSuccess;
        private static string m_DispenserUnitName;
        private static string m_CleanRetainBinFailed;
        private static string m_CleanRetainBinSuccess;
        private static string m_UnitRejectType;
        private string m_ErrorMsg;
        Regex regex = new Regex(@"\d+");
        /// <summary>
        /// 数据字典保存卡箱信息（根据语言配置文件TwinScreen.xml设置的内容）
        /// </summary>
        private Dictionary<string, string> dicCardUnit = new Dictionary<string, string>();
        private Dictionary<int, string> m_dicCard2Seq = new Dictionary<int, string>();

        private Dictionary<string, string> dicCardType = new Dictionary<string, string>();
        #region create interface
        /// <summary>
        /// 必须实现Create接口
        /// </summary>
        /// <returns></returns>
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new DisorderCardAdd() as IBusinessActivity;
        }

        /// <summary>
        /// 构造函数,防止外部调用
        /// </summary>
        protected DisorderCardAdd()
        {

        }
        #endregion

        #region override method
        /// <summary>
        /// 处理环境初始值
        /// </summary>
        /// <returns></returns>
        protected override bool InnerInit()
        {
            if (m_dicCard2Seq != null)
            {
                m_dicCard2Seq.Clear();
            }
            return base.InnerInit();
        }
        protected bool IsDisorderCardDispenser { get; set; }
        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override void HandleInitPage()
        {
            base.HandleInitPage();
            InitialUiResource();
            if (DispenserMantainType == (int)CardMantainType.Add)//加卡
            {
                TwinUI = "194_1";
                if (VTMContext.CardDispenser != null)
                {
                    var devResult = VTMContext.CardDispenser.GetCapabilitiesInfo(out var argCap);
                    IsDisorderCardDispenser = devResult.IsSuccess && argCap.IsDisorderCardDispenser == 1;
                }
                if (IsDisorderCardDispenser)
                {
                    TwinUI = "194_2";
                }
            }
            else         //--显示卡箱信息  2,0
            {
                TwinUI = "194";// 卡箱信息;
            }
            Context.TwinScreen.PromptBox(GetResValue("Loading"));
            Context.TwinScreen.ChangeUI(TwinUI);
        }
        /// <summary>
        /// 设置界面元素
        /// </summary>
        protected override void HandleSetPageValue()
        {
            base.HandleSetPageValue();
            if (DispenserMantainType == (int)CardMantainType.Add)
            {//加卡
                FillAddCardUI();
            }
            else //显示卡箱信息和清卡在一个页面
            {
                FillShowCardUnitInfoUI();
            }
        }

        /// <summary>
        /// 双屏设备事件处理
        /// </summary>
        /// <param name="argIDev">设备</param>
        /// <param name="argObjArg">事件参数</param>
        /// <returns></returns>
        protected override emBusiCallbackResult_t InnerOnDevEvtHandle(IDeviceService argIDev, DeviceEventArg argObjArg)
        {
            Log.Action.LogDebug("In action DisorderCardAdd");
            Debug.Assert(null != argIDev && null != argObjArg);
            emBusiCallbackResult_t result = base.InnerOnDevEvtHandle(argIDev, argObjArg);
            if (emBusiCallbackResult_t.Swallowd == result)
            {
                return result;
            }

            Debug.Assert(null != Context);
            if (argObjArg.Source == Context.TwinScreen.HostedDevice)
            {
                FlashDeviceEventArg arg = argObjArg as FlashDeviceEventArg;
                Debug.Assert(arg != null);
                //set ui cache for next flow
                SetUICache(arg.Key);

                Log.Action.LogDebug(arg.Key);
                //flash press event handle
                if (arg.Key.Contains("FUN_"))
                {
                    string[] funStrs = arg.Key.Split('_');
                    Log.Action.LogDebug(funStrs[1]);
                    if (regex.IsMatch(funStrs[1]))
                    {
                        if (funStrs[1].Equals("10") || funStrs[1].Equals("11")) //确定清卡动作
                        {
                            Context.TwinScreen.PromptOK(VTMContext.CurrentUIResource.LoadString("IDS_AddCardTip", TextCategory.s_TwinConfig));
                           
                            if (funStrs[1].Equals("11"))
                            {
                                DispenserMantainType = 3;
                            }
                        }
                        else
                        {
                            //跳转至设置号段页面
                            Context.TerminalDataCache.Set("SetSegmentCardNum", funStrs[1], GetType());
                            VTMContext.NextCondition = EventDictionary.s_EventContinue;
                            SignalCancel();
                        }
                    }
                    else
                    {
                        switch (arg.Key)
                        {
                            case TwinPressEvent.s_FunOK:
                                if (DispenserMantainType == (int)CardMantainType.Add)
                                {
                                    //当添加卡由于为空或非零数字导致加卡失败
                                    bool allowAddCardflag = true;
                                    if (null != ListCardUnitInfo && ListCardUnitInfo.Count > 0)
                                    {
                                        foreach (CardDispenserDeviceProtocol.CardUnitInfo carInfo in ListCardUnitInfo)
                                        {
                                            //为发卡箱
                                            if (carInfo.Type == emCRDType.SupplyBin)
                                            {
                                                //获得界面输入元素对应的输入值
                                                string strCardCount = Context.TwinScreen.GetValue(string.Format("INPUT_AddCount_S{0}", carInfo.Number));

                                                if (string.IsNullOrEmpty(strCardCount))
                                                {
                                                    allowAddCardflag = false;
                                                    m_ErrorMsg = string.Format(GetResValue("CardErrorMsg"), carInfo.Number, dicCardUnit[carInfo.Number.ToString()]);
                                                    Log.Action.LogError(m_ErrorMsg);
                                                    AddCardResultInfo(false, m_ErrorMsg);
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        Context.TwinScreen.SetValue("STATIC_PROMPT,", m_DevError + TwinSeparator.FieldSeparator);
                                        allowAddCardflag = false;
                                    }

                                    if (allowAddCardflag)
                                    {
                                        if (ClearDispenseCard()) //清卡
                                        {
                                            //清除表
                                            bool clearResult = VTMContext.VTMDataGateway.Clear(UkeyOrCard.Card);
                                            if (!clearResult)
                                            {

                                                Log.Action.LogError("Result Clear Card DataGateway is fail");
                                            }
                                            AddDispenseCard();//加卡
                                        }
                                    }
                                    else
                                    {
                                        Context.TwinScreen.UnPromptBox();
                                    }
                                }
                                else if (DispenserMantainType == 3)
                                {
                                    DispenserMantainType = 0;
                                    //清回收箱
                                    if (ClearRetainBin())
                                    {
                                        ShowCardUnitInfo();
                                        Context.TwinScreen.UnPromptBox();
                                    }
                                }
                                else if (DispenserMantainType == 4)
                                {
                                    Context.TwinScreen.UnPromptBox();
                                }
                                else
                                {
                                    Context.TwinScreen.UnPromptBox();
                                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                                    SignalCancel();
                                }
                                break;
                            case TwinPressEvent.s_FunRPrint: //增加打凭条
                                DispenserMantainType = 4;
                                HandleRPrint();
                                break;
                            default:
                                HandleDefaultKeyPress(arg.Key);
                                break;
                                //break;
                        }
                    }
                }
                else
                {
                    switch (arg.Key)
                    {
                        case TwinPressEvent.s_KeyBoardEnter:
                            if (DispenserMantainType == (int)CardMantainType.Add)
                            {
                                Context.TwinScreen.PromptOKCancel(VTMContext.CurrentUIResource.LoadString("IDS_ConfirmCleanAndAddCard", TextCategory.s_TwinConfig));
                            }
                            break;
                        default:
                            HandleDefaultKeyPress(arg.Key);
                            break;
                    }
                }

            }
            return emBusiCallbackResult_t.Bypass;
        }

        /// <summary>
        /// 凭条打印
        /// </summary>
        protected override void HandleRPrint()
        {
            CallBankInterFace4TwinScreenPrompt(emTwinScreenPromptType.StartPrintRPTR, null, () =>
            {
                Context.TwinScreen.PromptBox(GetResValue("JournalPrinting"));
            });
            var m_listRecInfo = new List<string>();
            //取卡箱信息
            VTMContext.CardDispenser.GetCardUnitInfo(out ListCardUnitInfo, m_devTimeout);
           
            if (ListCardUnitInfo != null)
            {
                //卡箱标题信息
                string strCardUnitInfo = GetResValue("CardInfo", InfoMode.Rec);
                m_listRecInfo.Add(GetResValue("CardTitle", InfoMode.Rec));
                m_listRecInfo.Add(strCardUnitInfo);
                ListCardUnitInfo.ForEach(p=> {
                    int len = GetFieldLength(strCardUnitInfo, 0, 2);
                    string number = p.Number.ToString().PadRight(len);
                    len = GetFieldLength(strCardUnitInfo, 1, 18);

                    string cardUnitType = string.Empty;
                    if (p.Type == emCRDType.SupplyBin)
                    {
                        //发卡箱
                        cardUnitType = GetResValue("SupplyBin");
                    }
                    else
                    {
                        //回收箱
                        cardUnitType = GetResValue("RetainBin");
                    }
                    string strCardUnitType = cardUnitType.PadRight(len);
                    len = GetFieldLength(strCardUnitInfo, 2, 6);
                    string initCount = p.InitialCount.ToString().PadRight(len);
                    len = GetFieldLength(strCardUnitInfo, 3, 6);
                    string count = p.Count.ToString().PadRight(len);
                    len = GetFieldLength(strCardUnitInfo, 4, 6);
                    string status = string.Empty;
                    if (p.Status != null)
                        status = p.Status.PadRight(len);
                    m_listRecInfo.Add(number + strCardUnitType + initCount + count + status);
                });
            }
            var bResult = ReceiptPrint(m_listRecInfo);

            CallBankInterFace4TwinScreenPrompt(emTwinScreenPromptType.EndPrintRPTR, bResult, () =>
            {
                if (bResult)
                {
                    Context.TwinScreen.PromptOK(GetResValue("PrintComplete"));
                }
                else
                {
                    Context.TwinScreen.PromptOK(GetResValue("PrintError"));
                }
            });
        }

        #endregion

        #region private method
        /// <summary>
        /// 取UI资源
        /// </summary>
        private void InitialUiResource()
        {
            m_DevError = VTMContext.CurrentUIResource.LoadString("IDS_DeviceError", TextCategory.s_TwinConfig);
            m_AddCard = VTMContext.CurrentUIResource.LoadString("IDS_AddCard", TextCategory.s_TwinConfig);
            m_AddCardFailed = VTMContext.CurrentUIResource.LoadString("IDS_AddCardFailed", TextCategory.s_TwinConfig);
            m_AddCardSuccess = VTMContext.CurrentUIResource.LoadString("IDS_AddCardSuccess", TextCategory.s_TwinConfig);
            m_AddCardLimited = VTMContext.CurrentUIResource.LoadString("IDS_AddCardLimited", TextCategory.s_TwinConfig);
            m_CleanCard = VTMContext.CurrentUIResource.LoadString("IDS_CleanCard", TextCategory.s_TwinConfig);
            m_CleanCardFailed = VTMContext.CurrentUIResource.LoadString("IDS_CleanCardFailed", TextCategory.s_TwinConfig);
            m_ClenaCardSuccess = VTMContext.CurrentUIResource.LoadString("IDS_CleanCardSuccess", TextCategory.s_TwinConfig);
            m_AfterCleanCardSuccess = VTMContext.CurrentUIResource.LoadString("IDS_AfterCleanCardSuccess", TextCategory.s_TwinConfig);
            m_DispenserUnitName = VTMContext.CurrentUIResource.LoadString("IDS_DispenserUnitName", TextCategory.s_TwinConfig);
            m_UnitRejectType = VTMContext.CurrentUIResource.LoadString("IDS_RetainBin", TextCategory.s_TwinConfig);
            m_ClearRetainBin = VTMContext.CurrentUIResource.LoadString("IDS_ClearRetainBin", TextCategory.s_TwinConfig);
            m_CleanRetainBinFailed = VTMContext.CurrentUIResource.LoadString("IDS_CleanRetainBinFailed", TextCategory.s_TwinConfig);
            m_CleanRetainBinSuccess = VTMContext.CurrentUIResource.LoadString("IDS_CleanRetainBinSuccess", TextCategory.s_TwinConfig);
        }

        /// <summary>
        /// 检查是否已经清卡
        /// 0:已经清卡；
        /// 大于0:还没清卡
        /// -1:设备错误
        /// </summary>
        private int HaveClearDispenser()
        {
            int result = 0;
            if (VTMContext.CardDispenser != null)
            {
                GrgCmdDispenserGetCardUnit cardUnit1;
                DevResult result11 = VTMContext.CardDispenser.GetCardUnitInfo("2", CardUnitType.INITCOUNT, out cardUnit1);

                if (result11.IsSuccess && !string.IsNullOrEmpty(cardUnit1.result))
                {
                    int.TryParse(cardUnit1.result, out result);
                }
                return result;
            }
            else
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// 填充加卡UI
        /// </summary>
        private void FillAddCardUI()
        {
            Debug.Assert(VTMContext != null);
            if (VTMContext.CardDispenser == null)
            {
                //设备故障不允许做号段设置
                Context.TwinScreen.SetValue("FUN_2,", "0,0" + TwinSeparator.FieldSeparator, true);
                Context.TwinScreen.SetValue("STATIC_PROMPT,", m_DevError + TwinSeparator.FieldSeparator);
            }
            else
            {
                ShowCardUnitInfo();
            }
            Context.TwinScreen.UnPromptBox();
        }

        /// <summary>
        /// 显示卡箱信息
        /// (如果是单发卡箱，第2个是发卡箱
        /// 如果是3卡箱，则第1，2，3是发卡箱
        /// ）
        /// </summary>
        private void FillShowCardUnitInfoUI()
        {
            Debug.Assert(VTMContext != null);

            if (VTMContext.CardDispenser == null)
            {
                Context.TwinScreen.SetValue("STATIC_PROMPT,", m_DevError + TwinSeparator.FieldSeparator);
                //设备故障不能进行清卡操作
                Context.TwinScreen.SetValue("FUN_10,", "0,0" + TwinSeparator.FieldSeparator, true);
                Context.TwinScreen.SetValue("FUN_11,", "0,0" + TwinSeparator.FieldSeparator, true);
                Context.TwinScreen.SetValue("FUN_PRINT,", "0,0" + TwinSeparator.FieldSeparator, true);
            }
            else
            {
                Context.TwinScreen.SetValue("FUN_10,", m_AddCard + TwinSeparator.FieldSeparator);
                Context.TwinScreen.SetValue("FUN_11,", m_ClearRetainBin + TwinSeparator.FieldSeparator);
                //如果凭条打印机故障，则屏蔽打印按钮
                if (VTMContext.ReceiptPrinter == null)
                {
                    Context.TwinScreen.SetValue("FUN_PRINT,", "0,0" + TwinSeparator.FieldSeparator, true);
                }
                //加载卡箱对应加卡类型
                ShowCardUnitInfo();
            }
            //--将获取的数据填充到UI 
            Context.TwinScreen.UnPromptBox();
        }
        /// <summary>
        /// 显示及填充卡箱详细信息
        /// </summary>
        private void ShowCardUnitInfo()
        {
            //取卡箱信息
            VTMContext.CardDispenser.GetCardUnitInfo(out ListCardUnitInfo, m_devTimeout);
            //加载卡箱显示标题信息
            LoadCardUnitName();
            // int idx = 0;
            string elements;
            string trHead = string.Empty;
            string values;
            string unitType = string.Empty;

            //读取配置文件VTMGeneralConfig.xml中卡类别配置<CardDispenserType cardType="VASA,UNIONPAY">
            var cardTypeList = VTMContext.VTMGeneralConfig.CardDispenserType;
            foreach (CardUnitInfo cardUnitInfo in ListCardUnitInfo)
            {
                string cardType = string.Empty;
                    if (DispenserMantainType == (int)CardMantainType.Add)
                    {
                        //加卡时，发卡箱显示下拉框选择卡类别
                        if (cardUnitInfo.Type == emCRDType.SupplyBin)
                        {
                            if (cardTypeList != null && cardTypeList.Count > 0)
                            {
                                foreach (var m_cardType in cardTypeList)
                                {
                                    if (m_cardType == cardUnitInfo.CardName)
                                    {
                                    Log.Action.LogDebug("m_cardType:" + cardUnitInfo.CardName);
                                        cardType += string.Format("<option  selected='true' value='{0}'>{0}</option>", m_cardType);
                                    }
                                    else
                                    {
                                        cardType += string.Format("<option value='{0}'>{0}</option>", m_cardType);
                                    }
                                }
                            }
                            else
                            {
                                Log.Action.LogErrorFormat("CardType can not find in VTMGeneralConfig xml file");
                            }
                        }
                }
                
                if (cardUnitInfo.Type == emCRDType.SupplyBin)
                {
                    //unitType = VTMContext.CurrentUIResource.LoadString("IDS_SupplyBin" + cardUnitInfo.Number, TextCategory.s_TwinConfig);
                    unitType = VTMContext.CurrentUIResource.LoadString("IDS_SupplyBin", TextCategory.s_TwinConfig);
                    trHead = string.Format("STATIC_TR_S{0}", cardUnitInfo.Number);
                    elements = string.Format("STATIC_UnitSeq_S{0},STATIC_UnitType_S{0},STATIC_CardType_S{0},STATIC_InitCount_S{0},STATIC_CurrentCount_S{0},INPUT_AddCount_S{0},SELECT_CardType_S{0},STATIC_State_S{0}", cardUnitInfo.Number);

                    //m_dicCard2Seq.Add(cardUnitInfo.Number, string.Format("INPUT_AddCount_S{0}", cardUnitInfo.Number));

                }
                else
                {
                    unitType = m_UnitRejectType;
                    trHead = string.Format("STATIC_TR_R{0}", cardUnitInfo.Number);
                    elements = string.Format("STATIC_UnitSeq_R{0},STATIC_UnitType_R{0},STATIC_CardType_R{0},STATIC_InitCount_R{0},STATIC_CurrentCount_R{0},INPUT_AddCount_R{0},INPUT_CardType_R{0},STATIC_State_R{0}", cardUnitInfo.Number);
                }

                values = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}",
                    TwinSeparator.FieldSeparator,
                    cardUnitInfo.Number,
                    unitType,
                    cardType,
                    cardUnitInfo.InitialCount,
                    cardUnitInfo.Count,
                    cardUnitInfo.Count, 
                    cardType,
                    cardUnitInfo.Status);


                Log.MaintenanceAction.LogDebug("trHead:" + trHead);
                Log.MaintenanceAction.LogDebug("elements:" + elements + ",values:" + values);
                Context.TwinScreen.SetValue(elements, values);
                Context.TwinScreen.SetValue(trHead, "1,1" + TwinSeparator.FieldSeparator, true);
            }
        }
        /// <summary>
        /// 加载卡箱标题信息
        /// </summary>
        private void LoadCardUnitName()
        {
            List<string> cardUnitComment = new List<string>(DispenserUnitName.Split(','));
            string[] strs;
            if (null != cardUnitComment && cardUnitComment.Count > 0)
            {
                foreach (string str in cardUnitComment)
                {
                    strs = str.Split(':');
                    if (null != strs && strs.Count() > 1)
                    {
                        if (!dicCardUnit.ContainsKey(strs[1]))
                        {
                            dicCardUnit.Add(strs[1], strs[0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除回收箱数据
        /// </summary>
        /// <returns></returns>
        protected bool ClearRetainBin()
        {
            string unitNumbers = string.Empty;
            GrgCmdDispenserStatus cardDispenserInfo = null;
            DevResult rets = VTMContext.CardDispenser?.GetStatusInfo(out cardDispenserInfo, 1000);
            if (cardDispenserInfo != null)
            {
                Log.Action.LogDebug("CardDispenser DeviceState is:" + cardDispenserInfo.DeviceState);
            }
            if (cardDispenserInfo == null || (HardwareState.Online != cardDispenserInfo.DeviceState && HardwareState.Busy != cardDispenserInfo.DeviceState))
            {
                Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                ClearCardResult(false);
                return false;
            }
            bool isSuccess = false;
            DevResult result;
            if (null != ListCardUnitInfo && ListCardUnitInfo.Count > 0)
            {
                foreach (CardDispenserDeviceProtocol.CardUnitInfo carInfo in ListCardUnitInfo)
                {
                    if (carInfo.Type == emCRDType.RetainBin)
                    {
                        unitNumbers += carInfo.Number + ",";
                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "COUNT", "0");
                        if (result.IsSuccess)
                        {
                            result = VTMContext.CardDispenser.SetCardUnitInfo();
                            if (result.IsSuccess)
                            {
                                if (VTMContext.CardReader != null)
                                {
                                    result = VTMContext.CardReader.ResetCount();
                                    VTMContext.CardReader.UpdateStatus();
                                }
                            }
                            
                        }
                        isSuccess = result.IsSuccess;
                        if (result.IsFailure)
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanRetainBinFailed + TwinSeparator.FieldSeparator);
                            ClearCardResult(isSuccess);
                            return false;
                        }
                    }
                }
                if (isSuccess)
                {
                    if (!string.IsNullOrEmpty(unitNumbers))
                    {
                        //删除数据库记录
                        isSuccess = VTMContext.VTMDataGateway.DeleteUkeyOrCardInfo(unitNumbers.TrimEnd(','));
                    }
                    Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanRetainBinSuccess + TwinSeparator.FieldSeparator);
                    ClearCardResult(isSuccess);
                }

            }
            return isSuccess;
        }
        /// <summary>
        /// 清卡操作
        /// </summary>
        protected bool ClearDispenseCard()
        {
            bool isSuccess = false;
            DevResult result;
            string cardCount = "0";       //清卡将卡设置为0；
            List<string> infoList = new List<string>();
            KeyValueDictionary dicPack;
            KeyValueDictionary dicUnpack;
            ukeyOrCards.Clear();
            Debug.Assert(VTMContext != null);
            VTMContext.TwinScreen.PromptBox(GetResValue("Processing"));
            //流水打印
            Log.Action.LogDebugFormat("CardDispenserMaintain :Clear card count.");

            string msg = string.Empty;
           
            GrgCmdDispenserStatus cardDispenserInfo = null;
            DevResult rets = VTMContext.CardDispenser?.GetStatusInfo(out cardDispenserInfo, 1000);
            if (cardDispenserInfo != null)
            {
                Log.Action.LogDebug("CardDispenser DeviceState is:" + cardDispenserInfo.DeviceState);

                //判断读卡器是否故障,则复位读卡器
                if (cardDispenserInfo.DeviceState == HardwareState.HWError)
                {
                    //复位读卡器
                    VTMContext.CardDispenser.Reset();
                    //再次取状态
                    VTMContext.CardDispenser.GetStatusInfo(out cardDispenserInfo, 1000);
                }
                else
                {
                    //判断读卡器中是否有卡，有则回收卡
                    if (cardDispenserInfo.MediaState == MediaState.Present)
                    {

                        var devResult = VTMContext.CardDispenser.RetainCard();
                        if (devResult.IsSuccess)
                        {
                            Log.Action.LogDebug("CardDispenser RetainCard Success");
                        }
                        else
                        {
                            Log.Action.LogDebug("CardDispenser RetainCard Fail");
                        }
                    }
                }
            }
           
            if (cardDispenserInfo == null || (HardwareState.Online != cardDispenserInfo.DeviceState && HardwareState.Busy != cardDispenserInfo.DeviceState))
            {
                Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                ClearCardResult(false);
                return false;
            }
            //  VTMContext.UKey.GetCardUnitInfo(out ukeyUnitList, m_devTimeout);
            int numb = 0;
            int returnnum = 0;
            if (null != ListCardUnitInfo && ListCardUnitInfo.Count > 0)
            {
                foreach (CardDispenserDeviceProtocol.CardUnitInfo carInfo in ListCardUnitInfo)
                {
                    if (carInfo.Type == emCRDType.RetainBin)
                    {
                        returnnum += carInfo.Count;
                        ukeyOrCards.Add(new UkeyOrCardInfo()
                        {
                            AddType = EMAddType.ClearCard,
                            CardInitCount = 0,
                            Number = 0,
                            ReturnCount = carInfo.Count,
                            CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CardUnitNumber = carInfo.Number,
                            IsReturnBin = IsCardOrUkeyReturnBin.Return
                        });

                        //CardUnitType.Count,表示设置卡箱当前值
                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "COUNT", cardCount);
                        isSuccess = result.IsSuccess;
                        if (result.IsFailure)
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                            ClearCardResult(isSuccess);
                            return false;
                        }
                        else
                        {
                            if (VTMContext.CardReader != null)
                            {
                                result = VTMContext.CardReader.ResetCount();
                                VTMContext.CardReader.UpdateStatus();
                            }
                        }
                    }
                    else if (carInfo.Type == emCRDType.SupplyBin)
                    {
                        numb += carInfo.Count;
                        ukeyOrCards.Add(new UkeyOrCardInfo()
                        {
                            AddType = EMAddType.ClearCard,
                            CardInitCount = carInfo.InitialCount,
                            Number = carInfo.Count,
                            ReturnCount = 0,
                            CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CardUnitNumber = carInfo.Number,
                            IsReturnBin = IsCardOrUkeyReturnBin.Supply
                        });

                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "INITCOUNT", cardCount);
                        //CardUnitType.Count,表示设置卡箱当前值
                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "COUNT", cardCount);
                        isSuccess = result.IsSuccess;
                        if (result.IsFailure)
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                            ClearCardResult(isSuccess);
                            return false;
                        }
                    }
                    else
                    {

                        ukeyOrCards.Add(new UkeyOrCardInfo()
                        {
                            AddType = EMAddType.ClearCard,
                            CardInitCount = carInfo.InitialCount,
                            Number = carInfo.Count,
                            ReturnCount = 0,
                            CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CardUnitNumber = carInfo.Number,
                            IsReturnBin = IsCardOrUkeyReturnBin.UnKnown
                        });

                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "INITCOUNT", cardCount);
                        //CardUnitType.Count,表示设置卡箱当前值
                        result = VTMContext.CardDispenser.SetCUInfo(carInfo.Number, "COUNT", cardCount);
                        isSuccess = result.IsSuccess;
                        if (result.IsFailure)
                        {
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                            ClearCardResult(isSuccess);
                            return false;
                        }
                    }
                }
            }

            VTMContext.TerminalDataCache.Set(DataCacheKey.VTM_ClearCardInfo, ukeyOrCards, GetType());

            if (!string.IsNullOrEmpty(ClearCardTrans))
            {
                if (Context.RequestTransaction(out dicPack, out dicUnpack, ClearCardTrans, null, null, false) == emSendRecvResult.Success)
                {
                    Context.TwinScreen.SetValue("STATIC_PROMPT,", m_ClenaCardSuccess + TwinSeparator.FieldSeparator);
                    DevResult ret = VTMContext.CardDispenser.SetCardUnitInfo();
                    if (ret.IsSuccess)
                    {
                        bool isDataSuccess = VTMContext.VTMDataGateway.AddUkeyOrCardInfo(ukeyOrCards);
                        if (!isDataSuccess)
                        {
                            Log.Action.LogError("clear card add Database fail");
                        }
                        else
                        {
                            Log.Action.LogDebug("clear card success");
                        }
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                        Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                    }
                }
                else
                {
                    isSuccess = false;
                    Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                }
            }
            else
            {
                DevResult ret = VTMContext.CardDispenser.SetCardUnitInfo();
                // VTMContext.TerminalDataCache.Set("CleanCardRet", ret.IsSuccess, GetType());
                if (ret.IsSuccess)
                {
                    //IList<UkeyOrCardInfo> ukeyOrCards = new List<UkeyOrCardInfo>(){
                    //        new UkeyOrCardInfo(){ AddType= EMAddType.ClearCard, 
                    //            Number=numb, ReturnCount=returnnum, 
                    //            CreateDate=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}};
                    bool isDataSuccess = VTMContext.VTMDataGateway.AddUkeyOrCardInfo(ukeyOrCards);
                    if (!isDataSuccess)
                    {
                        Log.Action.LogError("clear ukey add Database fail");
                    }
                    else
                    {
                        Log.Action.LogDebug("clear ukey success");
                    }
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                    Context.TwinScreen.SetValue("STATIC_PROMPT,", m_CleanCardFailed + TwinSeparator.FieldSeparator);
                }
            }
            ClearCardResult(isSuccess);
            return isSuccess;
        }

        private void ClearCardResult(bool isSuccess)
        {
            PrintOperationInfo(string.Format("<{0}>  {1} {2}", DeviceServiceName.s_CRD, "ClearCard", ""), isSuccess);
            Context.TwinScreen.UnPromptBox();
        }
        /// <summary>
        /// 处理加卡页面中，按钮事件(加卡操作)
        /// (将输入的卡信息保存)
        /// </summary>
        protected void AddDispenseCard()
        {
            Debug.Assert(VTMContext != null);
            DevResult result;
            bool isSuccess = false;
            string msg = string.Empty;
            //int idx = 0;
            ukeyOrCards.Clear();
            string cardCount = string.Empty;
            Log.Action.LogDebug("AddDispenseCard");
            if (null != ListCardUnitInfo && ListCardUnitInfo.Count > 0)
            {
                var cardTypeInfoList = new List<CardTypeInfo>();
                CardTypeInfo cardTypeInfo = null;
                foreach (CardDispenserDeviceProtocol.CardUnitInfo carInfo in ListCardUnitInfo)
                {               
                    if (carInfo.Type == emCRDType.RetainBin)
                    {

                    }
                    else if (carInfo.Type == emCRDType.SupplyBin)
                    {
                        cardTypeInfo = new CardTypeInfo();
                        cardTypeInfo.CardUnitNo = carInfo.Number;
                        cardTypeInfo.CardType = Context.TwinScreen.GetValue(string.Format("INPUT_CardType_S{0}", carInfo.Number));
                        cardTypeInfoList.Add(cardTypeInfo);

                        //获得界面输入元素对应的输入值
                        cardCount = Context.TwinScreen.GetValue(string.Format("INPUT_AddCount_S{0}", carInfo.Number));
                        if (!string.IsNullOrEmpty(cardCount) && regex.IsMatch(cardCount))
                        {
                            ukeyCardInf = new UkeyOrCardInfo()
                            {
                                AddType = EMAddType.AddCard,
                                CardInitCount = Convert.ToInt32(cardCount),
                                Number = Convert.ToInt32(cardCount),
                                ReturnCount = 0,
                                CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                CardUnitNumber = carInfo.Number,
                                IsReturnBin = IsCardOrUkeyReturnBin.Supply
                            };
                            VTMContext.DatabaseCache.Get(DataCacheKey.VTM_SegmentNumInfo + carInfo.Number, out obj, GetType());
                            if (null != obj)
                            {
                                listukeyOrCardDtails = obj as List<UkeyOrCardDetails>;
                                if (null != listukeyOrCardDtails && listukeyOrCardDtails.Count > 0)
                                {
                                    ukeyCardInf.Details = listukeyOrCardDtails;
                                }
                            }
                            ukeyOrCards.Add(ukeyCardInf);
                            Context.DatabaseCache.Set(DataCacheKey.VTM_SegmentNumInfo + carInfo.Number, null, GetType());
                            result = VTMContext.CardDispenser.SetCUInfo(Convert.ToInt32(carInfo.Number), "INITCOUNT", cardCount); 
                             //CardUnitType.Count,表示设置卡箱当前值
                             result = VTMContext.CardDispenser.SetCUInfo(Convert.ToInt32(carInfo.Number), "COUNT", cardCount);

                            result = VTMContext.CardDispenser.SetCUInfo(Convert.ToInt32(carInfo.Number), "CARDNAME", cardTypeInfo.CardType);
                            if (result.IsFailure)
                            {
                                if (string.IsNullOrEmpty(msg))
                                {
                                    msg = m_AddCardFailed;
                                }
                                AddCardResultInfo(isSuccess, msg);
                                return;
                            }
                        }
                        else
                        {
                            //m_ErrorMsg = string.Format("cardNumber: {0} cardName:{1} cardCount is empty or is Not number", carInfo.Number, dicCardUnit[carInfo.Number.ToString()]);
                            m_ErrorMsg = string.Format(GetResValue("CardErrorMsg"), carInfo.Number, dicCardUnit[carInfo.Number.ToString()]);
                            Log.Action.LogError(m_ErrorMsg);
                            AddCardResultInfo(false, m_ErrorMsg);
                            return;
                        }

                    }
                    else
                    {
                        cardCount = Context.TwinScreen.GetValue(string.Format("INPUT_AddCount_S{0}", carInfo.Number));
                        if (!string.IsNullOrEmpty(cardCount) && regex.IsMatch(cardCount))
                        {
                            Context.DatabaseCache.Get(DataCacheKey.VTM_SegmentNumInfo + carInfo.Number, out obj, GetType());
                            ukeyCardInf = new UkeyOrCardInfo()
                            {
                                AddType = EMAddType.AddCard,
                                CardInitCount = Convert.ToInt32(cardCount),
                                Number = Convert.ToInt32(cardCount),
                                ReturnCount = 0,
                                CreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                CardUnitNumber = carInfo.Number,
                                IsReturnBin = IsCardOrUkeyReturnBin.UnKnown
                            };
                            VTMContext.DatabaseCache.Get(DataCacheKey.VTM_SegmentNumInfo + carInfo.Number, out obj, GetType());
                            if (null != obj)
                            {
                                listukeyOrCardDtails = obj as List<UkeyOrCardDetails>;
                                if (null != listukeyOrCardDtails && listukeyOrCardDtails.Count > 0)
                                {
                                    ukeyCardInf.Details = listukeyOrCardDtails;
                                }
                            }
                            ukeyOrCards.Add(ukeyCardInf);
                            Context.DatabaseCache.Set(DataCacheKey.VTM_SegmentNumInfo + carInfo.Number, null, GetType());
                            result = VTMContext.CardDispenser.SetCUInfo(Convert.ToInt32(carInfo.Number), "INITCOUNT", cardCount);
                            //CardUnitType.Count,表示设置卡箱当前值
                            result = VTMContext.CardDispenser.SetCUInfo(Convert.ToInt32(carInfo.Number), "COUNT", cardCount);

                            if (result.IsFailure)
                            {
                                if (string.IsNullOrEmpty(msg))
                                {
                                    msg = m_AddCardFailed;
                                }
                                AddCardResultInfo(isSuccess, msg);
                                return;
                            }
                        }
                        else
                        {
                            //m_ErrorMsg = string.Format("cardNumber: {0} cardName:{1} cardCount is empty or is Not number", carInfo.Number, dicCardUnit[carInfo.Number.ToString()]);
                            m_ErrorMsg = string.Format(GetResValue("CardErrorMsg"), carInfo.Number, dicCardUnit[carInfo.Number.ToString()]);
                            Log.Action.LogError(m_ErrorMsg);
                            AddCardResultInfo(false, m_ErrorMsg);
                            return;
                        }
                    }
                }

                VTMContext.TerminalDataCache.Set(DataCacheKey.VTM_AddCardInfo, ukeyOrCards, GetType());
                if (!string.IsNullOrEmpty(AddCardTrans))
                {
                    Log.Action.LogDebug("AddCardTrans" + AddCardTrans);
                    KeyValueDictionary dicPack;
                    KeyValueDictionary dicUnpack;
                    if (Context.RequestTransaction(out dicPack, out dicUnpack, AddCardTrans, null, null, false) == emSendRecvResult.Success)
                    {
                        msg = m_AddCardSuccess;
                        isSuccess = VTMContext.CardDispenser.SetCardUnitInfo().IsSuccess;
                        if (isSuccess)
                        {
                            VTMContext.TerminalDataCache.Set("F_ADDCARD_REPORTTIME", DateTime.Now.ToString("yyyyMMddHHmmss"), GetType());
                            //发送加卡报文至v端
                            if (null != VTMContext.FeelViewService)
                            {
                                VTMContext.FeelViewService.SendFeelViewMsg(FeelViewMsgType.AddCard, false);
                            }

                            bool isDataSuccess = VTMContext.VTMDataGateway.AddUkeyOrCardInfo(ukeyOrCards);
                            if (!isDataSuccess)
                            {
                                Log.Action.LogError("clear card add Database fail");
                                Context.TwinScreen.SetValue("STATIC_PROMPT,", "clear card add Database fail" + TwinSeparator.FieldSeparator);
                            }
                            else
                            {
                                Log.Action.LogDebug("clear card success");
                                Context.TwinScreen.SetValue("STATIC_PROMPT,", "clear card success" + TwinSeparator.FieldSeparator);
                            }
                        }
                    }
                    else
                    {
                        msg = m_AddCardFailed;
                    }
                }
                else
                {
                    DevResult ret = VTMContext.CardDispenser.SetCardUnitInfo();
                    isSuccess = ret.IsSuccess;
                    msg = ret.IsSuccess ? m_AddCardSuccess : m_AddCardFailed;
                    VTMContext.TerminalDataCache.Set("AddCardResult", msg, this.GetType());
                    VTMContext.TerminalDataCache.Set("F_ADDCARD_REPORTTIME", DateTime.Now.ToString("yyyyMMddHHmmss"), GetType());
                    if (isSuccess)
                    {
                        bool isDataSuccess = VTMContext.VTMDataGateway.AddUkeyOrCardInfo(ukeyOrCards);
                        if (!isDataSuccess)
                        {
                            Log.Action.LogError("clear card add Database fail");
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", "clear card add Database fail" + TwinSeparator.FieldSeparator);
                        }
                        else
                        {
                            Log.Action.LogDebug("clear card success");
                            Context.TwinScreen.SetValue("STATIC_PROMPT,", "clear card success" + TwinSeparator.FieldSeparator);
                        }
                    }
                    if (null != VTMContext.FeelViewService)
                    {
                        //发送加卡报文至v端
                        VTMContext.FeelViewService.SendFeelViewMsg(FeelViewMsgType.AddCard, false);
                    }
                }
                if (isSuccess && cardTypeInfoList != null)
                {
                    //发卡类别信息写入到数据库 add by xjyong
                    VTMContext.VTMDataGateway.AddCardTypeInfo(cardTypeInfoList);
                }
                List<CardUnitInfo> carUnitList = null;
                VTMContext.CardDispenser.GetCardUnitInfo(out carUnitList);
                if (null != carUnitList && carUnitList.Count > 0)
                {
                    foreach (CardUnitInfo cardUnit in carUnitList)
                    {
                        if (cardUnit.Type == emCRDType.SupplyBin)
                        {
                            m_objContext.LogJournalWithTime(string.Format("{0} : {1},{2} : {3} ", m_objContext.CurrentJPTRResource.LoadString("IDS_CardNumber"), cardUnit.Number, m_objContext.CurrentJPTRResource.LoadString("IDS_CardCount"), cardUnit.Count));
                        }
                        if (cardUnit.Type == emCRDType.RetainBin)
                        {
                            m_objContext.LogJournalWithTime(string.Format("{0} : {1} ", m_objContext.CurrentJPTRResource.LoadString("IDS_ReturnCardCount"), cardUnit.Count));
                        }
                    }
                } 
                AddCardResultInfo(isSuccess, msg);
            }
        }
        /// <summary>
        /// 提示显示加卡信息
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="msg"></param>
        private void AddCardResultInfo(bool isSuccess, string msg)
        {
            PrintOperationInfo(string.Format("<{0}>  {1} {2}", DeviceServiceName.s_CRD, "AddCard", ""), isSuccess);
            Context.TwinScreen.SetValue("STATIC_PROMPT,", msg + TwinSeparator.FieldSeparator);
        }
        #endregion

        #region prop
        /// <summary>
        /// 
        /// </summary>
        protected VTMBusinessContext VTMContext
        {
            get
            {
                return (VTMBusinessContext)m_objContext;
            }
        }
        int m_DispenserMantainType = (int)CardMantainType.Clear;
        /// <summary>
        /// 发箱维护类型
        /// 0:清卡
        /// 1：加卡
        /// 2:显示卡箱信息
        /// </summary>
        [GrgBindTarget("DispenserMantainType", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int DispenserMantainType
        {
            get { return m_DispenserMantainType; }
            set
            {
                m_DispenserMantainType = value;
                OnPropertyChanged("DispenserMantainType");
            }
        }

        /// <summary>
        /// 发卡器卡箱名称，如果多个就用","隔开
        /// (如:"磁条卡，IC卡，信用卡")
        /// </summary>
        [GrgBindTarget("DispenserUnitName", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string DispenserUnitName
        {
            get
            {
                return m_DispenserUnitName;
            }
            set
            {
                m_DispenserUnitName = value;
                OnPropertyChanged("DispenserUnitName");
            }
        }
        string m_CardNoFormat = string.Empty;
        int m_MaxCardCount = 150;
        /// <summary>
        /// 卡箱最大加卡张数
        /// </summary>
        [GrgBindTarget("MaxCardCount", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int MaxCardCount
        {
            get { return m_MaxCardCount; }
            set
            {
                m_MaxCardCount = value;
                OnPropertyChanged("MaxCardCount");
            }
        }

        int m_AddCardResult;
        /// <summary>
        /// 交易后返回此action
        /// TransResult=0:不是加卡后显示卡箱信息
        /// TransResult=1:加卡后显示卡箱信息；
        /// </summary>
        [GrgBindTarget("AddCardResult", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int AddCardResult
        {
            get { return m_AddCardResult; }
            set
            {
                m_AddCardResult = value;
                OnPropertyChanged("AddCardResult");
            }
        }
        string m_ClearCardTrans = string.Empty;
        /// <summary>
        /// 清卡交易
        /// (如果是空，就不发交易)
        /// </summary>
        [GrgBindTarget("ClearCardTrans", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string ClearCardTrans
        {
            get { return m_ClearCardTrans; }
            set
            {
                m_ClearCardTrans = value;
                OnPropertyChanged("ClearCardTrans");
            }
        }
        string m_AddCardTrans = string.Empty;
        /// <summary>
        /// 加卡交易
        /// (如果是空，就不发交易)
        /// </summary>
        [GrgBindTarget("AddCardTrans", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string AddCardTrans
        {
            get { return m_AddCardTrans; }
            set
            {
                m_AddCardTrans = value;
                OnPropertyChanged("AddCardTrans");
            }
        }

        private List<CardDispenserDeviceProtocol.CardUnitInfo> ListCardUnitInfo = null;

        private IList<UkeyOrCardInfo> ukeyOrCards = new List<UkeyOrCardInfo>();

        object obj;
        IList<UkeyOrCardDetails> listukeyOrCardDtails = null;
        UkeyOrCardInfo ukeyCardInf = null;
        #endregion

    }

}
