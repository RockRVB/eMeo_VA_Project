using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using IBankProjectBusinessActivityBase;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{5DF552B3-D159-290A-9FC5-CBDF07F6AB2F}",
                 NodeNameOfConfiguration = "FaceIdentifyResultFB20",
                 Name = "FaceIdentifyResultFB20",
                 Author = "alan.yu")]
    public class FaceIdentifyResultFB20 : IBankProjectActivityBase
    {
        #region creating

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceIdentifyResultFB20();
        }

        #endregion

        #region constructor

        protected FaceIdentifyResultFB20()
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
                return result;
            }

            object obj;
            VTMContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FaceIdentifyResult data is null or empty. unpack fail.");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            var strData = (string)obj;
            var dataObj = JObject.Parse(strData);

            var matchResult = dataObj["threshold"];
            Log.Action.LogDebug($"threshold is:{matchResult}");


            var list = JArray.FromObject(dataObj["result"]);
            if (list != null && list.Count > 0)
            {
                dataObj = JObject.FromObject(list.OrderByDescending(o => decimal.Parse((o["highPrecisionScore"] ?? o["score"]).ToString())).First());
                Log.Action.LogDebug($"score is:{dataObj["highPrecisionScore"] ?? dataObj["score"]}");
                VTMContext.TransactionDataCache.Set("FB_CustomerId", dataObj["userId"], GetType());
                VTMContext.TransactionDataCache.Set("FB_ImageToken", dataObj["imageToken"], GetType());
            }
            else
            {
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
