using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{4CDED0FC-3D30-1266-0FB5-6AFD89CFD79E}",
                 NodeNameOfConfiguration = "FaceCheckResultFB20",
                 Name = "FaceCheckResultFB20",
                 Author = "alan.yu")]
    public class FaceCheckResultFB20 : IBankProjectActivityBase
    {
        #region creating

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceCheckResultFB20();
        }

        #endregion

        #region constructor

        protected FaceCheckResultFB20()
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

            VTMContext.TransactionDataCache.Get("FB_jsonrevdata", out var obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("return data is null or empty. unpack fail.");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var strData = (string)obj;
            Log.Action.LogDebug($"CompareTwoImage result: [{strData}].");

            var dataObj = JObject.Parse(strData);

            var strScore = dataObj["score"]?.ToString();
            var strThreshold = dataObj["threshold"]?.ToString();
            bool matchResult;
            if (string.IsNullOrWhiteSpace(strScore) || string.IsNullOrWhiteSpace(strThreshold) ||
                !decimal.TryParse(strScore, out var score) || !decimal.TryParse(strThreshold, out var threshold))
            {
                matchResult = false;
            }
            else
            {
                matchResult = score >= threshold;
            }
            if (!matchResult)
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
