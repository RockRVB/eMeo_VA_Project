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
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{00608065-0511-4969-82EB-8A94685D6602}",
                 NodeNameOfConfiguration = "GetCustomerInfoResultFB",
                 Name = "GetCustomerInfoResultFB",
                 Author = "")]
    public class GetCustomerInfoResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new GetCustomerInfoResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected GetCustomerInfoResultFB()
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
                Log.Action.LogError("GetCustomerInfoResultFB data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("GetCustomerInfoResultFB data is:{0}", strData);
            JArray list = JArray.Parse(strData);
            if (list != null && list.Count > 0)
            {
                dataObj = (JObject)(list[0]);
                string customer_id = dataObj["customerId"]?.ToString();
                //Log.Action.LogDebugFormat("customerId = {0}", customer_id);
                VTMContext.TransactionDataCache.Set("FB_CustomerId", customer_id, GetType());
            }
            else
            {
                Log.Action.LogError("can not find customer id !!!");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
