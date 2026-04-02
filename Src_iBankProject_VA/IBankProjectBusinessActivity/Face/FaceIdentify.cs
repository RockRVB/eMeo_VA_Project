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

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "FaceIdentify",
                 Name = "FaceIdentify",
                 Author = "Raymond")]
    public class FaceIdentify : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceIdentify() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceIdentify()
        {

        }
        #endregion


        private int m_LimitSimilarity = 0;
        [GrgBindTarget("LimitSimilarity", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int LimitSimilarity
        {
            get
            {
                return m_LimitSimilarity;
            }
            set
            {
                m_LimitSimilarity = value;
                OnPropertyChanged("LimitSimilarity");
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
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            //Log.Action.LogDebugFormat("FaceIdentify LimitSimilarity is:{0}", LimitSimilarity);


            object objImageFile = null;
            string imageFile = string.Empty;
            ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out objImageFile, this.GetType());


            //objImageFile = string.Format("{0}{1}\\pic.jpg", AppDomain.CurrentDomain.BaseDirectory, "TempForTest", Guid.NewGuid().ToString());//ForTest
            //Log.Action.LogDebugFormat("FaceIdentify objImageFile is:{0}", objImageFile);


            if (objImageFile!=null)
            {
                imageFile = objImageFile.ToString();

                IdentifyFaceResult identify = ProjVTMContext.FaceRecognizeService.IdentifyFace(imageFile, "test"); 
                Log.Action.LogDebugFormat("Response code is:{0}", identify.ResponseCode);
                Log.Action.LogDebugFormat("Response Msg is:{0}", identify.ErrorMsg);

                if (identify != null && identify.ResponseCode == 10003)//用户不存在
                {
                    ProjVTMContext.NextCondition = "NotMatch";
                    Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                    return emBusActivityResult_t.Success;
                }

                if (identify?.ResponseCode == 0 && identify?.DataList.Count > 0)
                {
                    ProjVTMContext.NextCondition = EventDictionary.s_EventConfirm;
                   // ProjVTMContext.TransactionDataCache.Set("iBank_IdentifyFaceResultDataList", identify.DataList, this.GetType());


                    bool isMatch = false;
                    List<IdentifyFaceResult.ResponseBody> datalist = identify.DataList;
                    foreach (var item in datalist)
                    {
                        //Log.Action.LogDebugFormat("IdentifyFace User Id is:{0}, simulity is:{1}, fileName is:{2}", item.UserId, item.Similarity, item.ImageFileName);

                        if (item.Similarity > LimitSimilarity)
                            isMatch = true;
                    }

                    Log.Action.LogDebugFormat("iBank_IdentifyFaceResultDataList count is:{0}", datalist.Count);

                    datalist.RemoveAll(m => m.Similarity < LimitSimilarity);


                    //for (int i = 0; i < datalist.Count; i++)
                    //{
                    //    string uid= datalist[i].UserId;
                    //    string lastThree = uid.Substring(uid.Length-4);
                    //    string maskStr = new string('*', uid.Length - 3);
                    //}

                    ProjVTMContext.TransactionDataCache.Set("iBank_IdentifyFaceResultDataList", datalist, this.GetType());
                    Log.Action.LogDebugFormat("After remove by LimitSimilarity iBank_IdentifyFaceResultDataList count is:{0}", datalist.Count);

                    if (!isMatch|| datalist.Count<1)//不匹配
                    {
                        //SwitchUIState(m_objContext.MainUI, "NotMatch", 3000);
                        ProjVTMContext.NextCondition = "NotMatch";
                        Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                        return emBusActivityResult_t.Success;
                    }
                }
                else
                {
                    ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                }
            }
            else
            {
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion

    }
}
