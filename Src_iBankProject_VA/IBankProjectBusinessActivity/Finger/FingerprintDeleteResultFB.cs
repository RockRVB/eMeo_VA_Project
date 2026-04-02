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
    [GrgActivity("{102C77CE-B80F-4F14-823A-93B6F1296BBD}",
                 NodeNameOfConfiguration = "FingerprintDeleteResultFB",
                 Name = "FingerprintDeleteResultFB",
                 Author = "")]
    public class FingerprintDeleteResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerprintDeleteResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerprintDeleteResultFB()
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
                Log.Action.LogError("FingerprintDeleteResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FingerprintDeleteResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);
            dataObj = JObject.Parse(dataObj["header"]?.ToString());

            string responseCode = dataObj["responseCode"]?.ToString();
            if (responseCode == "0")
            {
                VTMContext.NextCondition = "OnConfirm";
            }
            else
            {
                VTMContext.NextCondition = "OnFail";
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
