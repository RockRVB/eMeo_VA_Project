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
using DevServiceProtocol;
using IBankProjectBusinessActivityBase;
using eCATBusinessActivityXDCBase;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{E0457F2E-D026-443D-BE04-8346972E14FF}",
                 NodeNameOfConfiguration = "FingerprintRegisterResultFB",
                 Name = "FingerprintRegisterResultFB",
                 Author = "")]
    public class FingerprintRegisterResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerprintRegisterResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerprintRegisterResultFB()
        {

        }
        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            object obj = null;
            string strData = string.Empty;
            JObject dataObj = null;

            m_objContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FingerprintRegisterResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FingerprintRegisterResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);
            dataObj = JObject.Parse(dataObj["header"]?.ToString());

            string responseCode = dataObj["responseCode"]?.ToString();
            if (responseCode == "0")
            {
                VTMContext.NextCondition = "OnYes";
            }
            else
            {
                VTMContext.NextCondition = "OnNo";
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
