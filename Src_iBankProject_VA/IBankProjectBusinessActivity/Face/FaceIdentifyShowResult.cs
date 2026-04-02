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

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "FaceIdentifyShowResult",
                 Name = "FaceIdentifyShowResult",
                 Author = "Raymond")]
    public class FaceIdentifyShowResult : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceIdentifyShowResult() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceIdentifyShowResult()
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
                Log.Action.LogDebugFormat("Get iBank_FaceRecognitionImage obj is:{0}", obj);

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



        private string m_CompareDataShow = "";
        [GrgBindTarget("compareData", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string compareData
        {
            get
            {
                object objDataList = null;
                List<IdentifyFaceResult.ResponseBody> dataList = null;
                ProjVTMContext.TransactionDataCache.Get("iBank_IdentifyFaceResultDataList", out objDataList, this.GetType());
                //Log.Action.LogDebugFormat("attribute iBank_IdentifyFaceResult is:{0}", objDataList);
                if (objDataList != null)
                {
                    dataList = objDataList as List<IdentifyFaceResult.ResponseBody>;

                    if (dataList.Count > 0)
                    {
                        List<ShowFaceIdentifyResultEntity> showDataList = new List<ShowFaceIdentifyResultEntity>();
                        ShowFaceIdentifyResultEntity entity = null;
                        foreach (var item in dataList)
                        {
                            //Log.Action.LogDebugFormat("UserId is:{0},rate is:{1}, Similarity is:{2}", "*", item.Similarity, item.ImageFileName);

                            entity = new ShowFaceIdentifyResultEntity();
                            entity.pic = item.ImageFileName.Replace("\\", "/");
                            //Log.Action.LogDebugFormat("entity.pic is:{0}", entity.pic);
                            entity.rate = item.Similarity.ToString() + "%";
                            entity.userid = item.UserId;
                            entity.username = item.UserName;
                            showDataList.Add(entity);
                        }

                        if (showDataList.Count > 0)
                        {
                            ShowFaceIdentifyResultEntity[] showArray = showDataList.ToArray();
                            m_CompareDataShow = JsonExtension.ToJSON(showArray);
                            //Log.Action.LogDebugFormat("m_CompareDataShow Json is:{0}", m_CompareDataShow);
                        }
                    }
                }
                return m_CompareDataShow;
            }
            set
            {
                m_CompareDataShow = value;
                OnPropertyChanged("compareData");
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

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            //curPhoto = @"D:\GRG2019\VTM标准版\Trunk\VTMC\IBank2.1\Execute\Resource\Common\HTML\images\pic3.jpg";
            //curPhoto = curPhoto.Replace("\\", "/");
            //Log.Action.LogDebugFormat("2  curPhoto file is:{0}.", curPhoto);

            object objDataList = null;
            List<IdentifyFaceResult.ResponseBody> dataList = null;
            ProjVTMContext.TransactionDataCache.Get("iBank_IdentifyFaceResultDataList", out objDataList, this.GetType());
            //Log.Action.LogDebugFormat(" iBank_IdentifyFaceResult is:{0}", objDataList);
            if (objDataList != null)
            {
                dataList = objDataList as List<IdentifyFaceResult.ResponseBody>;

                string userId1 = dataList[0].UserId;
                string imageFile1 = dataList[0].ImageFileName;
                string similarity1 = dataList[0].Similarity.ToString();

                //Log.Action.LogDebugFormat("userId1 is:{0}, imageFile1 is:{1}, similarity1 is:{2}", userId1, imageFile1, similarity1);

                if (dataList.Count > 1)
                {
                    string userId2 = dataList[1].UserId;
                    string imageFile2 = dataList[1].ImageFileName;
                    string similarity2 = dataList[1].Similarity.ToString();

                    //Log.Action.LogDebugFormat("userId2 is:{0}, imageFile2 is:{1}, similarity2 is:{2}", userId2, imageFile2, similarity2);
                }

                if (dataList.Count > 2)
                {
                    string userId3 = dataList[2].UserId;
                    string imageFile3 = dataList[2].ImageFileName;
                    string similarity3 = dataList[2].Similarity.ToString();

                    //Log.Action.LogDebugFormat("userId3 is:{0}, imageFile3 is:{1}, similarity3 is:{2}", userId3, imageFile3, similarity3);
                }
            }
            else
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;                
            }


            emWaitSignalResult_t emWaitResult = WaitSignal();

            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
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
                    Log.Action.LogDebugFormat("FaceIdentifyShowResult argUIEvent.Key is:{0}", strKey);
                    m_objContext.NextCondition = strKey;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        #endregion

    }

    public class ShowFaceIdentifyResultEntity
    {
        public string pic;
        public string rate;
        public string userid;
        public string username;
    }
}
