using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{1507041C-5697-C9BC-A479-BE491AE5E604}",
                 NodeNameOfConfiguration = "FingerveinMatchResultFB20",
                 Name = "FingerveinMatchResultFB20",
                 Author = "alan.yu")]
    public class FingerveinMatchResultFB20 : IBankProjectActivityBase
    {
        #region creating

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerveinMatchResultFB20();
        }

        #endregion

        #region constructor

        protected FingerveinMatchResultFB20()
        {

        }

        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            var result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return result;
            }

            m_objContext.TransactionDataCache.Get("FB_jsonrevdata", out var obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FingerveinMatchResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var dataObj = JObject.Parse((string)obj);

            Log.Action.LogInfo($"Status: [{dataObj["status"]}].");
            Log.Action.LogInfo($"Score: [{dataObj["score"]}].");

            var status = dataObj["status"]?.ToString();
            var templateId = dataObj["templateId"]?.ToString();
            ProjVTMContext.TransactionDataCache.Set("FB_FingerveinTemplateId", templateId, GetType());
            if (status == "0")
            {
                SwitchUIState(m_objContext.MainUI, "FingerMatchSuccess", 2000);
                WaitSignal();
                m_objContext.NextCondition = "OnMatch";
            }
            else
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}
