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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "InputFaceUserInfo",
                 Name = "InputFaceUserInfo",
                 Author = "Raymond")]
    public class InputFaceUserInfo : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new InputFaceUserInfo() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected InputFaceUserInfo()
        {

        }
        #endregion


        private string m_CurPhoto = "";
        [GrgBindTarget("curPhoto", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string curPhoto
        {
            get
            {
                object obj = null;
                ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out obj, this.GetType());
                //Log.Action.LogDebugFormat("InputFaceUserInfo Get iBank_FaceRecognitionImage obj is:{0}", obj);

                if (obj != null)
                {
                    m_CurPhoto = obj.ToString().Replace("\\", "/");
                }

                //Log.Action.LogDebugFormat("InputFaceUserInfo Get m_CurPhoto is:{0}", m_CurPhoto);
                
                return m_CurPhoto;
            }
            set
            {
                m_CurPhoto = value;
                OnPropertyChanged("curPhoto");
            }
        }


        private string m_FaceUserId = string.Empty;
        [GrgBindTarget("FaceUserId", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FaceUserId
        {
            get
            {
                return m_FaceUserId;
            }
            set
            {
                m_FaceUserId = value;
                OnPropertyChanged("FaceUserId");
            }
        }
        
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

            //FaceUserId = "1234567";

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            ProjVTMContext.NextCondition = EventDictionary.s_EventConfirm;

            emWaitSignalResult_t emWaitResult = WaitSignal();

            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                ProjVTMContext.ActionResult = emBusActivityResult_t.Timeout;
                ProjVTMContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }


        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {

            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                //Log.Action.LogDebugFormat("InputFaceUserInfo argUIEvent.Key is:{0}, argUIEvent.ElementName is:{1}", argUIEvent.Key, argUIEvent.ElementName);

                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                
                string strKey = argUIEvent.Key as string; 
                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    object obj = null;


                    VTMContext.MainUI.GetPropertyValueOfElement(null, "formData", UIPropertyKey.s_ContentKey, out obj);
                    //Log.Action.LogDebugFormat("2 formData s_ContentKey is:{0}", obj);
                    //2 formData s_ContentKey is:{"userid":"1234567999","username":""}
                    if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(obj.ToString());
                        if (jo != null)
                        {
                            string userid = "";
                            string userUserName = "";
                            if(jo["userid"] != null)
                            {
                                userid = jo["userid"].ToString();
                            }
                            if(jo["username"] != null)
                            {
                                userUserName = jo["username"].ToString();
                            }

                            //Log.Action.LogDebugFormat("InputFaceUserInfo userid is:{0}, userUserName is:{1}", userid, userUserName);

                            ProjVTMContext.TransactionDataCache.Set("iBank_FaceRecognitionAddUid", userid, this.GetType());
                            ProjVTMContext.TransactionDataCache.Set("iBank_FaceRecognitionAddUserName", userUserName, this.GetType());
                        } 
                    }

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
