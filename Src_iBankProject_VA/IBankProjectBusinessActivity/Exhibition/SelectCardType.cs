using System;
using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using System.Collections.Generic;
using System.Linq;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMBusinessServiceProtocol;
using VTMBusinessServiceProtocol.DataGateway;
using VTMModelLibrary;

namespace VTMBusinessActivity.DebitCard
{
    [GrgActivity("{28E18830-C9C0-4020-9DAD-4FB281C33AB3}",
                  Name = "SelectCardType",
                  NodeNameOfConfiguration = "SelectCardType",
                  Author = "Tommy")]
    public class SelectCardType : BusinessActivityVTMBase
    {
        private List<CardTypeInfo> _cardTypeInfos;

        #region constructor

        private SelectCardType()
        {
        }

        #endregion

        #region create

        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new SelectCardType();
        }

        #endregion

        #region Overrided methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            try
            {
                //先执行基类方法，执行失败不再继续执行
                var result = base.InnerRun(argContext);
                if (emBusActivityResult_t.Success != result)
                {
                    Log.Action.LogError("Failed to run base's implement");
                    return result;
                }

                #region 处理逻辑

                var vtmBusinessContext = m_objContext as VTMBusinessContext;
                if (vtmBusinessContext == null)
                {
                    throw new Exception("(m_objContext as VTMBusinessContext) is null!");
                }
                //从数据库获取所有卡类型
                if (!vtmBusinessContext.VTMDataGateway.IsOpen)
                {
                    throw new Exception("vtmBusinessContext.VTMDataGateway.IsOpen is false!");
                }
                _cardTypeInfos = vtmBusinessContext.VTMDataGateway.QueryCardTypeInfo();
                if (_cardTypeInfos == null || _cardTypeInfos.Count <= 0) //无法获取卡类型
                {
                    Log.Action.LogError("Failed to get card type list from database!");
                    m_objContext.NextCondition = EventDictionary.s_EventFail;
                    return emBusActivityResult_t.Success;
                }

                //获取所有卡槽单元信息
                List<CardUnitInfo> cardUnitList;
                VTMContext.CardDispenser.GetCardUnitInfo(out cardUnitList);
                if (cardUnitList == null || cardUnitList.Count <= 0)
                {
                    Log.Action.LogError("Failed to get card unit info of card dispenser!");
                    m_objContext.NextCondition = EventDictionary.s_EventFail;
                    return emBusActivityResult_t.Success;
                }

                //按照卡类型统计卡剩余数量
                var sumByCardType = new Dictionary<string, int>();
                _cardTypeInfos.ForEach(info =>
                {
                    if (!sumByCardType.ContainsKey(info.CardType))
                    {
                        sumByCardType.Add(info.CardType, 0);
                    }
                    var cardUnit = cardUnitList.FirstOrDefault(unit => unit.Number == info.CardUnitNo);
                    if (cardUnit != null)
                    {
                        sumByCardType[info.CardType] += cardUnit.Count;
                    }
                    else
                    {
                        Log.Action.LogError($"Card unit with Number [{info.CardUnitNo}] not found!");
                    }
                });

                //为UI展示准备数据字符串
                var cardTypeInfoForUi = sumByCardType.Aggregate(string.Empty, (current, kv) => current + kv.Key + "," + kv.Value + ";");

                //保存数据到数据池
                m_objContext.TransactionDataCache.Set(VTMDataDictionary.VtmCardTypeInfoForUi, cardTypeInfoForUi, GetType());

                #endregion

                //切用户界面
                SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

                //等待用户操作
                emWaitSignalResult_t emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();
                if (emWaitResult == emWaitSignalResult_t.Timeout) //等待超时
                {
                    m_objContext.ActionResult = emBusActivityResult_t.Timeout;
                    m_objContext.NextCondition = EventDictionary.s_EventTimeout;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
                m_objContext.NextCondition = EventDictionary.s_EventFail;
            }
            finally
            {
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            }

            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUi, UIEventArg argUiEvent)
        {
            //基类先处理该事件
            var result = base.InnerOnUIEvtHandle(iUi, argUiEvent);
            if (result != emBusiCallbackResult_t.Bypass)//如果基类处理完不继续传递事件
            {
                return result;
            }

            if (argUiEvent.EventName == UIPropertyKey.s_clickKey)
            {
                var strKey = argUiEvent.Key as string;
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    #region 保存选择结果到数据池

                    //保存选择的卡类型
                    var selectedType = strKey.TrimStart('O').TrimStart('n');
                    m_objContext.TransactionDataCache.Set(VTMDataDictionary.VtmSelectedNewCardType, selectedType, GetType());
                    Log.Action.LogDebug($"Selected new card type: [{selectedType}].");
                    //保存选择的卡类型对应的卡槽列表（For 'DispenseCard'）
                    var supplyCardUnit = _cardTypeInfos.Aggregate(string.Empty, (current, info) => current + info.CardUnitNo + ',');
                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_CARDSUPPLY_FLAG, supplyCardUnit, GetType());
                    Log.Action.LogDebug($"Card tray list of selected new card type: [{supplyCardUnit}].");

                    #endregion

                    m_objContext.NextCondition = strKey;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;//处理完毕，吞咽事件，不再传递
                }
            }
            return emBusiCallbackResult_t.Bypass;//不处理该事件，继续传递
        }

        #endregion
    }
}