using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using System.Configuration;
using eCATBusinessServiceProtocol;
using System.Threading;
using UIServiceProtocol;
using IBankProjectBusinessActivityBase;
using RemoteTellerServiceProtocol;
using Newtonsoft.Json;
using FingerServerRequestService;
using FingerveinServerRequestService;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{00DBE545-7371-4655-BF96-AD13AC6E94CE}",
            NodeNameOfConfiguration = "BAB_InputOTP",
            Name = "BAB_InputOTP",
            Author = "vnteam")]
    public class BAB_InputOTP : IBankProjectActivityBase
    {

        #region constructor
        private BAB_InputOTP()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new BAB_InputOTP();
        }
        #endregion

        #region property

        private string m_OTP = string.Empty;
        [GrgBindTarget("OTP", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string OTP
        {
            get
            {
                return m_OTP;
            }
            set
            {
                m_OTP = value;
                OnPropertyChanged("OTP");
            }
        }

        private string m_OtpError = string.Empty;
        [GrgBindTarget("OtpError", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string OtpError
        {
            get
            {
                return m_OtpError;
            }
            set
            {
                m_OtpError = value;
                OnPropertyChanged("OtpError");
            }
        }
        private string m_ShowOTP = "false";
        [GrgBindTarget("ShowOTP", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ShowOTP
        {
            get
            {
                return m_ShowOTP;
            }
            set
            {
                m_ShowOTP = value;
                OnPropertyChanged("ShowOTP");
            }
        }
        private string m_otpTryAgain = "0";
        [GrgBindTarget("otpTryAgain", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string otpTryAgain
        {
            get
            {
                return m_otpTryAgain;
            }
            set
            {
                m_otpTryAgain = value;
                OnPropertyChanged("otpTryAgain");
            }
        }
        private string m_ResendOTP = "1";
        [GrgBindTarget("ResendOTP", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ResendOTP
        {
            get { return m_ResendOTP; }
            set
            {
                m_ResendOTP = value;
                OnPropertyChanged("ResendOTP");
            }
        }
        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
         /*   BAB_STMConfig.LoadCashWithdrawConfig();
            int lifeTime = int.Parse(BAB_STMConfig.CashWithdrawConfig.Otp.Lifetime);
            ProjVTMContext.TimePoint2 = DateTime.Now;
            Log.Action.LogDebugFormat("TimePoint2:{0}", ProjVTMContext.TimePoint2.ToString());
            int interval = (ProjVTMContext.TimePoint2 - ProjVTMContext.TimePoint1).Seconds;
            Log.Action.LogDebugFormat("interval:{0}", interval.ToString());
            interval = interval < 3 ? 0 : interval;
            m_objContext.TransactionDataCache.Set("LeftTimeOTP", lifeTime - interval, GetType());*/
        //    BAB_STMConfig.LoadCashWithdrawConfig();
        //    string sTemp = BAB_STMConfig.CashWithdrawConfig.Otp.MaxRequest;
            int remain = 0;
        /*    if (!string.IsNullOrEmpty(sTemp))
            {
                int maxRetryTimes = 0;
                int.TryParse(sTemp, out maxRetryTimes);
      //          remain = maxRetryTimes - ProjVTMContext.ResendOTP;
                remain++;
                if (remain < 0)
                {
                    m_objContext.NextCondition = "OnEnd";
                    SignalCancel();
                    return emBusActivityResult_t.Success;
                }
                Log.Action.LogDebugFormat("Left {0} = {1} - {2} resend time(s)", remain, maxRetryTimes, ProjVTMContext.ResendOTP);
                m_objContext.TransactionDataCache.Set("resendOTP", remain, GetType());
            }*/
            
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            //otpTryAgain = ProjVTMContext.RetryCounting.ToString();
            object objRespCode = null;
            m_objContext.TransactionDataCache.Get("BAB_ErrorCode", out objRespCode, GetType());
            if (objRespCode != null && objRespCode?.ToString() == "82")
            {
                Log.Action.LogDebugFormat("show_otpsystem_error");
                m_objContext.MainUI.ExecuteScriptCommand("show_otpsystem_error", false);
            }
            else
            {
                object justSendOTP = null;
                ProjVTMContext.TransactionDataCache.Get("BAB_JustSendOTP", out justSendOTP, GetType());
                if (justSendOTP != null && justSendOTP.ToString() == "Y")
                {
                    ProjVTMContext.TransactionDataCache.Set("BAB_JustSendOTP", "N", GetType());

                    Log.Action.LogDebugFormat("show_otpfail");
                    m_objContext.MainUI.ExecuteScriptCommand("show_otpfail", false);
                    m_objContext.MainUI.TriggerEventSyn("CWD_OTPWrong");

                    //Thread.Sleep(4000);//wait voice complete
                }
            }
        //    if (ProjVTMContext.NeedShowResendOTPButton == true)
            {
                m_objContext.MainUI.ExecuteScriptCommand("showButton", false);
         //       ProjVTMContext.ShowedResendOTPButton = true;
         //       ProjVTMContext.NeedShowResendOTPButton = false;
                //ProjVTMContext.RetryCounting = 0;
            }
            emWaitSignalResult_t waitResult = WaitSignal();

            if (waitResult == emWaitSignalResult_t.Timeout)
            {
           //     if (ProjVTMContext.ShowedResendOTPButton == true)
                {
                    m_objContext.NextCondition = "OnEnd";
                    return emBusActivityResult_t.Success;
                }
              //  ProjVTMContext.ResendOTP++;
             //   ProjVTMContext.NeedShowResendOTPButton = true;
             //   ProjVTMContext.RetryCounting -= 1;
                ProjVTMContext.NextCondition = EventDictionary.s_EventTimeout;
             //   CommonClass.WriteEJLogForSignalResult(waitResult);
                return emBusActivityResult_t.Success;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);

            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string strMessage = string.Empty;
                    string key = (string)objArg.Key;
                    if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase) ||
                        key.Equals(EventDictionary.s_EventNext, StringComparison.OrdinalIgnoreCase))
                    {
                        ProjVTMContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);

                        Log.Action.LogDebugFormat("OTP Code is: {0}", OTP);

                        if (string.IsNullOrWhiteSpace(OTP) || OTP.Length < 6)
                        {
                            //m_objContext.CurrentUIResource.LoadString("IDS_InvalidOTP", TextCategory.s_UI, out strMessage);
                            //OtpError = strMessage;
                            Log.Action.LogDebugFormat("OtpCode error: {0}", OTP);
                            return emBusiCallbackResult_t.Swallowd;                         
                        }
                        else
                        {
                          //  m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableConfirmButtonFunctionName, false);
                            ProjVTMContext.NextCondition = EventDictionary.s_EventConfirm;
                          //  ProjVTMContext.CusInfoForm.otp = OTP;
                            SignalCancel();
                            return emBusiCallbackResult_t.Swallowd;
                        }
                        
                    }
                    else
                    {
                        if (key.Equals("OnBack"))
                        {
                      //      m_objContext.MainUI.ExecuteScriptCommand(CommonClass.DisableBackButtonFunctionName, false);
                        }
                        Log.Action.LogDebugFormat("Certification Key is: {0}", key);
                      //  CommonClass.WriteEJLog(key);
                        ProjVTMContext.NextCondition = key;
                        SignalCancel();
                    }
                }
            }
            return emBusiCallbackResult_t.Swallowd;
        }
    }
}
