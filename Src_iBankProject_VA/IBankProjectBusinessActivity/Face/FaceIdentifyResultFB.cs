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
using RemoteTellerServiceProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{685F8280-CF05-4A40-BD3F-263E06993A8F}",
                 NodeNameOfConfiguration = "FaceIdentifyResultFB",
                 Name = "FaceIdentifyResultFB",
                 Author = "")]
    public class FaceIdentifyResultFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceIdentifyResultFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceIdentifyResultFB()
        {

        }
        #endregion

        private string m_CurPhoto = "";
        [GrgBindTarget("curPhoto", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string curPhoto
        {
            get
            {
                //m_CurPhoto = @"D:\GRG2019\VTM标准版\Trunk\VTMC\IBank2.1\Execute\TempForTest\pic3.jpg";
                //m_CurPhoto = m_CurPhoto.Replace("\\", "/");

                object obj = null;
                ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out obj, this.GetType());
                //Log.Action.LogDebugFormat("Get iBank_FaceRecognitionImage obj is:{0}", obj);

                if (obj != null)
                {
                    m_CurPhoto = obj.ToString().Replace("\\", "/");
                }

                //Log.Action.LogDebugFormat("Get m_CurPhoto is:{0}", m_CurPhoto);


                return m_CurPhoto;
            }
            set
            {
                m_CurPhoto = value;
                OnPropertyChanged("curPhoto");
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


            object obj = null;
            ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out obj, this.GetType());
            //Log.Action.LogDebugFormat("Get iBank_FaceRecognitionImage obj is:{0}", obj);


            VTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out obj, this.GetType());
            //Log.Action.LogDebugFormat("Get iBank_FaceRecognitionImage_Current obj is:{0}", obj);

            //Log.Action.LogDebugFormat("curPhoto file is:{0}.", curPhoto);
            string strData = string.Empty;
            JObject dataObj = null;

            VTMContext.TransactionDataCache.Get("FB_jsonrevdata", out obj);
            if (string.IsNullOrEmpty(obj as string))
            {
                Log.Action.LogError("FaceIdentifyResult data is null or empty. unpack fail.");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }

            strData = obj as string;
            //Log.Action.LogDebugFormat("FaceIdentifyResult data is:{0}", strData);
            dataObj = JObject.Parse(strData);

            string matchResult = dataObj["matchResult"]?.ToString();
            Log.Action.LogDebugFormat("matchResult is:{0}", matchResult);
            if (matchResult != "True")
            {             
                VTMContext.NextCondition = "NotMatch";
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }
                    
            JArray list = JArray.Parse(dataObj["result"]?.ToString());
            if(list!= null && list.Count > 0)
            {
                dataObj = (JObject)(list[0]);
                string customer_id = dataObj["customer_id"]?.ToString();
                //Log.Action.LogDebugFormat("FB customer_id = {0}", customer_id);
                VTMContext.TransactionDataCache.Set("FB_CustomerId", customer_id, GetType());
            }

            //curPhoto = @"D:\GRG2019\VTM标准版\Trunk\VTMC\IBank2.1\Execute\Resource\Common\HTML\images\pic3.jpg";
            //curPhoto = curPhoto.Replace("\\", "/");
            //Log.Action.LogDebugFormat("2  curPhoto file is:{0}.", curPhoto);
            SwitchUIState(m_objContext.MainUI, "FaceMatchSuccess", 2000);
            WaitSignal();

            VTMContext.NextCondition = EventDictionary.s_EventConfirm; 
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
                    Log.Action.LogDebugFormat("FaceIdentifyResultFB argUIEvent.Key is:{0}", strKey);
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
