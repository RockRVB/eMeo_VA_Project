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
    [GrgActivity("{32464291-DF0A-4F20-ABD8-06EACCCB83DB}",
                 NodeNameOfConfiguration = "FBLoginResult",
                 Name = "FBLoginResult",
                 Author = "")]
    public class FBLoginResult : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FBLoginResult() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FBLoginResult()
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
                Log.Action.LogError("FBLoginResult data is null or empty. unpack fail.");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FBLoginResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);

            ProjVTMContext.CardHolderDataCache.Set("FB_accessToken", dataObj["accessToken"]?.ToString(), GetType());
            ProjVTMContext.CardHolderDataCache.Set("FB_refreshToken", dataObj["refreshToken"]?.ToString(), GetType());
            ProjVTMContext.CardHolderDataCache.Set("FB_sceneKey", dataObj["sceneKey"]?.ToString(), GetType());
            ProjVTMContext.CardHolderDataCache.Set("FB_username", dataObj["username"]?.ToString(), GetType());
            ProjVTMContext.CardHolderDataCache.Set("FB_userid", dataObj["id"]?.ToString(), GetType());

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
