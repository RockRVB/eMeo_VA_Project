using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{325911C5-EEE7-5FEF-D643-7AB740292779}",
                 NodeNameOfConfiguration = "GetCustomerInfoResultFB20",
                 Name = "GetCustomerInfoResultFB20",
                 Author = "alan.yu")]
    public class GetCustomerInfoResultFB20 : IBankProjectActivityBase
    {
        #region creating

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new GetCustomerInfoResultFB20();
        }

        #endregion

        #region constructor

        protected GetCustomerInfoResultFB20()
        {

        }

        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            var result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            m_objContext.TransactionDataCache.Get("FB_jsonrevdata", out var obj);
            var strData = obj as string;
            if (string.IsNullOrEmpty(strData))
            {
                Log.Action.LogError("GetCustomerInfoResultFB data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var list = JArray.Parse(strData);
            if (list == null || list.Count != 1)
            {
                Log.Action.LogError(list == null ? "can not find customer id!" : $"[{list.Count}] customers found!");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var dataObj = (JObject)list[0];
            var customerId = dataObj["customerId"]?.ToString();
            VTMContext.TransactionDataCache.Set("FB_CustomerId", customerId, GetType());

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
