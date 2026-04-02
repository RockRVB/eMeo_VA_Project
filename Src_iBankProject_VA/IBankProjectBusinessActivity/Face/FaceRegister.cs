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
using System.IO;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{400D9590-3212-4906-8C75-4B839BA947C5}",
                 NodeNameOfConfiguration = "FaceRegister",
                 Name = "FaceRegister",
                 Author = "Raymond")]
    public class FaceRegister : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceRegister() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceRegister()
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
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            object objImageFile = null;
            ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out objImageFile, this.GetType());


            //objImageFile = string.Format("{0}{1}\\pic.jpg", AppDomain.CurrentDomain.BaseDirectory, "TempForTest", Guid.NewGuid().ToString());//ForTest
            //Log.Action.LogDebugFormat("InputFaceUserInfo objImageFile is:{0}", objImageFile);


            if (objImageFile != null)
            {
                object obj;
                ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionAddUid", out obj, this.GetType());
                //Log.Action.LogDebugFormat("InputFaceUserInfo iBank_FaceRecognitionAddUid is:{0}", obj);

                RegisterFaceRequestEntity registerRequest = new RegisterFaceRequestEntity();
                if (obj!=null)
                {
                    registerRequest.UserId = obj.ToString();
                }
                //registerRequest.UserId = "123456";

                ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionAddUserName", out obj, this.GetType());
                //Log.Action.LogDebugFormat("InputFaceUserInfo iBank_FaceRecognitionAddUserName is:{0}", obj);
                if (obj != null)
                    registerRequest.Name = obj.ToString();

                registerRequest.ImageFileName = objImageFile.ToString();
                registerRequest.Groups = new string[] { "test" };


                QueryUserInfoResult queryResult= ProjVTMContext.FaceRecognizeService.QueryUserInfo(registerRequest.UserId);
                if (queryResult != null && queryResult.ResponseCode == 0)
                {
                    int delResult = ProjVTMContext.FaceRecognizeService.DeleteUserInfo(registerRequest.UserId);

                    //Log.Action.LogDebugFormat("InputFaceUserInfo DeleteUserInfo result is:{0}", delResult);
                }


                RegisterFaceResult addUserInfoResult = ProjVTMContext.FaceRecognizeService.AddUser(registerRequest);
                if (addUserInfoResult.ResponseCode == 0)
                {
                    Log.Action.LogDebugFormat("FaceRegister AddUser() successful.");

                    VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                }
                else
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            else
                VTMContext.NextCondition = EventDictionary.s_EventFail;


            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion

    }
}
