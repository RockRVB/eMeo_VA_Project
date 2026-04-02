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
using DevServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{E42751C2-2D01-4f53-9718-4754A2BEA2B7}",
                 NodeNameOfConfiguration = "FaceCaptureImage",
                 Name = "FaceCaptureImage",
                 Author = "Raymond")]
    public class FaceCaptureImage : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FaceCaptureImage() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FaceCaptureImage()
        {

        }
        #endregion

        #region method
        //private Dualcamera dualCamera = null;
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

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;

            string imageFile = string.Empty;
            Log.Action.LogDebugFormat("FaceCaptureImage FaceRecognizeService.StartTakePic(out imageFile)");

            if(ProjVTMContext.SensorAndLight != null)
            {
                ProjVTMContext.SensorAndLight.SetGuidLight(GuidLight.DualCamera, GuidLightFlashMode.Continuous);
            }
            else
            {
                Log.Action.LogDebug("DualCamera SensorAndLight is null ->");
            }

            emTakePicResult takePicRst = ProjVTMContext.FaceRecognizeService.StartTakePic(out imageFile);
            //Log.Action.LogDebugFormat("takePicRst is:{0},fileName is:{1}", (emTakePicResult)takePicRst, imageFile);
            if(ProjVTMContext.SensorAndLight != null)
            {
                ProjVTMContext.SensorAndLight.SetGuidLight(GuidLight.DualCamera, GuidLightFlashMode.Off);
            }
            
            //SetBindData("iBank_FaceRecognitionImage_Current", imageFile);
            ProjVTMContext.TransactionDataCache.Set("iBank_FaceRecognitionImage_Current", imageFile,this.GetType());
            if (!string.IsNullOrEmpty(imageFile))
            {             
                string imgBase64String = ImgProc.ImgToBase64String(imageFile);
                ProjVTMContext.TransactionDataCache.Set("FB_FaceImgBase64String", imgBase64String, GetType());
            }

            if (takePicRst==emTakePicResult.Failure)
            {
                CloseCapture();
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                return emBusActivityResult_t.Success; 
            }
            else if (takePicRst==emTakePicResult.Timeout)
            {
                CloseCapture();
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
                return emBusActivityResult_t.Success; 
            }
            

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        private void CloseCapture()
        {
            ProjVTMContext.FaceRecognizeService.StopTakePic();
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                string strKey = argUIEvent.Key as string;

                Log.Action.LogDebugFormat("argUIEvent.Key is:{0}, argUIEvent.ElementName is:{1}", argUIEvent.Key, argUIEvent.ElementName);

                if (!string.IsNullOrWhiteSpace(strKey))
                {
                    if (strKey.Equals("OnCancel", StringComparison.OrdinalIgnoreCase))
                    {
                        CloseCapture();

                        m_objContext.NextCondition = strKey;
                        SignalCancel();
                        return emBusiCallbackResult_t.Swallowd;
                    }
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }
        
        #endregion

    }
}
