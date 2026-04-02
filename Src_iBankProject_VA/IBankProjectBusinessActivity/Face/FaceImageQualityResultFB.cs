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
    [GrgActivity("{405B2C70-7E8F-4B12-A0D3-3DFF3C0C5F6F}",
                 NodeNameOfConfiguration = "FaceImageQualityResultFB",
                 Name = "FaceImageQualityResultFB",
                 Author = "")]
    public class FaceImageQualityResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceImageQualityResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceImageQualityResultFB()
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
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            object obj = null;
            string strData = string.Empty;
            JObject dataObj = null;

            m_objContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FaceImageQualityResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FaceImageQualityResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);
            dataObj = JObject.Parse(dataObj["data"]?.ToString());
            string error_no = dataObj["error_no"]?.ToString();
            if (error_no == "0")
            {
                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            }
            else
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }


            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }


        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {

            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                string strKey = argUIEvent.Key as string;
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    m_objContext.NextCondition = strKey;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }
        #endregion
    }
}
