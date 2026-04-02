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
using FaceRecognizeServiceProtocol;
using IBankProjectBusinessActivityBase;
using RemoteTellerServiceProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{685F8280-CF05-4A40-BD3F-263E06993A8F}",
                 NodeNameOfConfiguration = "FaceCheckResultFB",
                 Name = "FaceCheckResultFB",
                 Author = "")]
    public class FaceCheckResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceCheckResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceCheckResultFB()
        {

        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base");
                return result;
            }


            object obj = null;
            string strData = string.Empty;
            JObject dataObj = null;

            VTMContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("return data is null or empty. unpack fail.");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            dataObj = JObject.Parse(strData);

            string matchResult = dataObj["matchResult"]?.ToString();
            Log.Action.LogDebugFormat("matchResult is:{0}", matchResult);
            if (matchResult.ToLower() != "true")
            {
                SwitchUIState(m_objContext.MainUI, "FaceNotMatch", 2000);
                WaitSignal();
                VTMContext.NextCondition = "NotMatch";
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }
            SwitchUIState(m_objContext.MainUI, "FaceMatchSuccess", 2000);
            WaitSignal();

            VTMContext.NextCondition = EventDictionary.s_EventConfirm; 
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}
