using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{C8E13016-071B-93E2-EA5D-A4C25E61D181}",
                 NodeNameOfConfiguration = "FingerprintMatchResultFB20",
                 Name = "FingerprintMatchResultFB20",
                 Author = "alan.yu")]
    public class FingerprintMatchResultFB20 : IBankProjectActivityBase
    {
        #region creating

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerprintMatchResultFB20();
        }

        #endregion

        #region constructor

        protected FingerprintMatchResultFB20()
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
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FingerprintMatchResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var dataObj = JObject.Parse((string)obj);

            Log.Action.LogInfo($"score: [{dataObj["score"]}].");

            var templateId = dataObj["templateId"]?.ToString();
            ProjVTMContext.TransactionDataCache.Set("FB_FingerprintTemplateId", templateId, GetType());

            var status = dataObj["status"]?.ToString();
            if (status == "0")
            {
                VTMContext.NextCondition = "OnMatch";
            }
            else
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
