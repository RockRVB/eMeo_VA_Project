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
using RemoteTellerServiceProtocol;
using UIServiceProtocol;
using FingerveinServerRequestService;
using FingerveinExDeviceProtocol;
using DevServiceProtocol;
using IBankProjectBusinessActivityBase;
using eCATBusinessActivityXDCBase;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{5CC296C0-4F71-4BE2-9D5F-5E82E50F23DF}",
                 NodeNameOfConfiguration = "FingerveinMatchResultFB",
                 Name = "FingerveinMatchResultFB",
                 Author = "")]
    public class FingerveinMatchResultFB : BusinessActivityXDCBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerveinMatchResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerveinMatchResultFB()
        {

        }
        #endregion

        public const string Fingervein_Success = "0";
        public const string Fingervein_Fail = "1";
        public const string Fingervein_NotExit = "3";
        public const string Fingervein_reject = "6";
        public const string Fingervein_NotFindSimilar = "7";
        public const string Fingervein_tampered = "8";

        private bool m_GoXDC = false;

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            m_GoXDC = false;
            object obj = null;
            string strData = string.Empty;
            JObject dataObj = null;

            m_objContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FingerveinMatchResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FingerveinMatchResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);
            dataObj = JObject.Parse(dataObj["header"]?.ToString());

            string responseCode = dataObj["responseCode"]?.ToString();
            if (responseCode == Fingervein_Success)
            {
                SwitchUIState(m_objContext.MainUI, "FingerMatchSuccess", 2000);
                WaitSignal();
                m_objContext.NextCondition = "OnMatch";
            }
            else if (responseCode == Fingervein_NotExit)
            {               
                SwitchUIState(m_objContext.MainUI, "FingerNotRegister");
                m_objContext.NextCondition = EventDictionary.s_EventNotFound;
            }
            else
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        public override bool SetNextState()
        {
            if (!m_GoXDC)
            {
                m_objContext.IgnoreConfig = false;
            }
            else
            {
                base.SetNextState();
            }
            return true;
        }

        #endregion
    }
}
