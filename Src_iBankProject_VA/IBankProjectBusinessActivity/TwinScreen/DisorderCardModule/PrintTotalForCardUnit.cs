using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessMaintenanceActivity;
using eCATBusinessServiceProtocol;
using FlashDeviceProtocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VTMBusinessServiceProtocol;

namespace TwinScreen
{
    [GrgActivity("{60F65345-4852-455F-8327-8DCBF17EBB91}",
                Name = "PrintTotalForCardUnit",
                Description = "PrintTotalForCardUnit Des",
                NodeNameOfConfiguration = "PrintTotalForCardUnit",
                Catalog = "PrintTotalForCardUnit",
                Author = "  ",
                ForwardTargets = new string[] { EventDictionary.s_EventContinue })]
    public class BusinessActivityPrintTotalForCardUnit : eCATBusinessTwinScreenActivityBase
    {

        #region method of creating
        /// <summary>
        /// 实现Create接口
        /// </summary>
        /// <returns></returns>
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityPrintTotalForCardUnit() as IBusinessActivity;
        }
        #endregion

        #region constructor
        /// <summary>
        /// 构造函数,防止外部调用
        /// </summary>
        protected BusinessActivityPrintTotalForCardUnit()
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
            TwinUI = "121";
            return base.InnerInit();
        }
        #endregion

        #region page initialize

        /// <summary>
        /// 初始化界面
        /// </summary>
        protected override void HandleInitPage()
        {

            if (VTMContext.CardDispenser != null)
            {
                var devResult = VTMContext.CardDispenser.GetCapabilitiesInfo(out var argCap);
                IsDisorderCardDispenser = devResult.IsSuccess && argCap.IsDisorderCardDispenser == 1;
            }
            //change ui
            Context.TwinScreen.ChangeUI(TwinUI);
        }

        /// <summary>
        /// 设置界面元素
        /// </summary>
        protected override void HandleSetPageValue()
        {
            Context.TwinScreen.PromptBox(GetResValue("Loading"));
            base.HandleSetPageValue();
            if (VTMContext.ReceiptPrinter == null)
            {
                Context.TwinScreen.SetValue("FUN_1,", "0,0" + TwinSeparator.FieldSeparator, true);
            }
            if (VTMContext.CardDispenser == null)
            {
                Context.TwinScreen.SetValue("STATIC_TXTBOX,", GetResValue("NODEVICE"));
            }
            else {
                ShowCardUnitInfo();
            }
            Context.TwinScreen.UnPromptBox();
        }
        private List<CardDispenserDeviceProtocol.CardUnitInfo> ListCardUnitInfo = null;
        List<string> list = new List<string>();
        private void ShowCardUnitInfo()
        {
            //取卡箱信息
            VTMContext.CardDispenser.GetCardUnitInfo(out ListCardUnitInfo, m_devTimeout);
            list.Add("Number".PadRight(7, ' ') + "TYPE".PadRight(5, ' ') + "InitialCount".PadRight(13, ' ') + "Count".PadRight(6, ' ') + "RetainCount".PadRight(12, ' ') + "Status");
            foreach (CardUnitInfo cardUnitInfo in ListCardUnitInfo)
            {
                list.Add(cardUnitInfo.Number.ToString().PadRight(2, ' ') + (cardUnitInfo.Type== emCRDType.RetainBin? "RetainBin": "SupplyBin").PadRight(10, ' ') + cardUnitInfo.InitialCount.ToString().PadRight(4, ' ') + cardUnitInfo.Count.ToString().PadRight(5, ' ') + cardUnitInfo.RetainCount.ToString().PadRight(5, ' ') + cardUnitInfo.Status);
            }
            //乱序发卡机信息
            if (IsDisorderCardDispenser)
            {
                list.Add("----------------------------------------");
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
                        else {
                            present++;
                        }
                    }
                    list.Add(string.Format(GetResValue("DisorderCardModuleEmptySlot"), emptySlot)); 
                    list.Add(string.Format(GetResValue("DisorderCardModuleNoEmptySlot"), present));
                }
                var listCardTypeInfo = VTMContext.VTMDataGateway.QueryDisorderCardInfo();
                if (listCardTypeInfo != null && listCardTypeInfo.Count > 0)
                {
                    list.Add(string.Format(GetResValue("DispenseCardSuccessNumer"), listCardTypeInfo.FindAll(m=>m.status.Equals(0)).Count));
                    list.Add(string.Format(GetResValue("DispenseCardFailedNumer"), listCardTypeInfo.FindAll(m => m.status.Equals(1)|| m.status.Equals(2)).Count));
                }
            }
            if (list.Count > 0)
            {
                Context.TwinScreen.SetValue("STATIC_TXTBOX,", string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss"),
                              GetResValue("DisorderCardSlotInfo")
                              ));
                foreach (string data in list)
                {
                    VTMContext.LogJournal(data, LogSymbol.DeviceFailure);
                    Context.TwinScreen.SetValue("STATIC_TXTBOX,",data);
                }
            }
        }
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
                //set ui cache for next flow
                SetUICache(arg.Key);
                //flash press event handle
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
                return emBusiCallbackResult_t.Bypass;
            }

            return emBusiCallbackResult_t.Bypass;
        }
        /// <summary>
        /// 打印凭条
        /// </summary>
        protected virtual void HandleFun1()
        {
            Context.TwinScreen.PromptBox(GetResValue("Processing"));
            Context.ReceiptPrinter?.PrintData(list);
            Context.TwinScreen.UnPromptBox();
        }
        protected override void InnerExit()
        {
            base.InnerExit();
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
