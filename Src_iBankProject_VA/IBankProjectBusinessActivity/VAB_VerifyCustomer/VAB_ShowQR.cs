using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using IBankProjectBusinessServiceProtocol;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using RemoteTellerServiceProtocol;
using RestfulServiceProtocol;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIServiceProtocol;
using VTMBusinessActivityBase;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{4D432E05-09B9-4ACA-B0C8-C22698688A98}",
                     Name = "VAB_ShowQR",
                     NodeNameOfConfiguration = "VAB_ShowQR",
                     Author = "rocky",
                     ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]
    public class VAB_ShowQR : IBankProjectActivityBase
    {
        private string m_input_val = string.Empty;
        [GrgBindTarget("input_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string input_val
        {
            get { return m_input_val; }
            set
            {
                m_input_val = value;
                OnPropertyChanged("input_val");
            }
        }
        private string m_TimerResentVerifyQRl = String.Empty;
        [GrgBindTarget("TimerResentVerifyQR", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string TimerResentVerifyQR
        {
            get
            {
                return m_TimerResentVerifyQRl;
            }
            set
            {
                m_TimerResentVerifyQRl = value;
            }
        }
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
        private static volatile bool _stopVerifyQRPolling = false;

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
            //
            object value = null;
            ProjVTMContext.TransactionDataCache.Get("VAB_QRData", out value, GetType());
            if (value != null)
            {
                var HTMLJson_input = new
                {
                    QR_Code = value.ToString()
                };
                input_val = HTMLJson_input.ToJSON();
            }
            else goto outActivity;

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            _stopVerifyQRPolling = false;

            // Check verify QR song song trong lúc màn hình đang chờ thao tác.
            int resentQR = Int32.Parse(TimerResentVerifyQR);
            Task.Run(() =>
            {
                try
                {
                    int countReset = 0;
                    int timeoutReset = 0;

                    while (!_stopVerifyQRPolling)
                    {
                        string responseCode = string.Empty;
                        emRestfulServiceResult sendResult = ProjVTMContext.RestfulService.SendMessage("VerifyQR", out responseCode, MessageFormat.JSON);

                        if (sendResult == emRestfulServiceResult.Success && responseCode == "0")
                        {
                            Log.Project.LogDebug("VerifyQR success. Stop polling and continue workflow.");
                            _stopVerifyQRPolling = true;
                            VTMContext.NextCondition = "OnConfirm";
                            SignalCancel();
                            break;
                        }

                        countReset++;
                        Log.Project.LogDebug($"VerifyQR not success. resend time: {countReset}, sendResult: {sendResult}, responseCode: {responseCode}");

                        Thread.Sleep(resentQR);
                        timeoutReset += resentQR;

                        if (timeoutReset >= CountDown)
                        {
                            Log.Project.LogDebug("VerifyQR polling timeout by page timeout. Stop polling.");
                            _stopVerifyQRPolling = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Project.LogDebug($"RestfulService sent verify QR failed: {ex}");
                }
            });

            emWaitSignalResult_t emWaitResult = WaitPopu == 1 ? VTMWaitSignal() : WaitSignal();
            _stopVerifyQRPolling = true;


            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Cancel;
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        outActivity:
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
                _stopVerifyQRPolling = true;
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
