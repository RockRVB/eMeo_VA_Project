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
                 NodeNameOfConfiguration = "CheckFaceImageQuality",
                 Name = "CheckFaceImageQuality",
                 Author = "Raymond")]
    public class CheckFaceImageQuality : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new CheckFaceImageQuality() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected CheckFaceImageQuality()
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

            //emWaitSignalResult_t emWaitResult = WaitSignal();

            //if (emWaitResult == emWaitSignalResult_t.Timeout)
            //{
            //    VTMContext.ActionResult = emBusActivityResult_t.Timeout;
            //    VTMContext.NextCondition = EventDictionary.s_EventTimeout;
            //}

            object objImageFile = null;
            ProjVTMContext.TransactionDataCache.Get("iBank_FaceRecognitionImage_Current", out objImageFile, this.GetType());

            if (objImageFile!=null)
            {
                int checkImageResult= ProjVTMContext.FaceRecognizeService.CheckImageQuality(objImageFile.ToString());
                if (checkImageResult==0)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventConfirm;
                }
                else
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                }
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
