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
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{EAAE3F2D-276E-469D-968A-0CEFDCED20CF}",
                 NodeNameOfConfiguration = "FingerveinDeleteAllResultFB",
                 Name = "FingerveinDeleteAllResultFB",
                 Author = "")]
    public class FingerveinDeleteAllResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerveinDeleteAllResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerveinDeleteAllResultFB()
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
                Log.Action.LogError("FingerveinDeleteAllResultFB data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FingerveinDeleteAllResultFB data is:{0}", strData);
            dataObj = JObject.Parse(strData);
            dataObj = JObject.Parse(dataObj["header"]?.ToString());

            string responseCode = dataObj["responseCode"]?.ToString();
            if (responseCode == "0")
            {
                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            }
            else
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
            }           

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
