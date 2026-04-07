using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using RemoteTellerServiceProtocol;
using System;
using System.Linq;
using VTMBusinessActivityBase;
using UIServiceProtocol;
using IBankProjectBusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{4D432E05-09B9-4ACA-B0C8-C22698688A98}",
                     Name = "VAB_ShowQR",
                     NodeNameOfConfiguration = "VAB_ShowQR",
                     Author = "rocky",
                     ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]
    public class VAB_ShowQR : IBankProjectActivityBase
    {

        public VAB_ShowQR()
        {

        }

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_ShowQR();
        }
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                Log.Action.LogErrorFormat("execute {0} InnerRun failed", GetType());
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();

            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
			// Xử lý button khi click vào sự kiện
			// Ví dụ string strKeyOther = argUIEvent.Key as string; strKeyOther sẽ bằng OnBack hoặc OnConfirm dựa theo html định nghĩa. Mọi xử lý nhấn button đều đưa về hàm này xử ly
			
            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
				// ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData); nó có nhiệm vụ đồng bộ lại data binding giửa C# và html
				ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
				
                string strKeyOther = argUIEvent.Key as string;
                VTMContext.NextCondition = strKeyOther;
                SignalCancel();
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }
        protected override emWaitSignalResult_t HandlePageTimeout(string argSignals, ref int argTimeoutOrInterval)
        {

            Log.Action.LogDebug("HandlePageTimeout ->");
            emWaitSignalResult_t emResult = emWaitSignalResult_t.Timeout;
            return emResult;
        }
        #endregion
    }
}
