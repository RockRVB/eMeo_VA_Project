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
using RemoteTellerServiceProtocol;
using UIServiceProtocol;
using FingerveinServerRequestService;
using FingerveinExDeviceProtocol;
using DevServiceProtocol;
using IBankProjectBusinessActivityBase;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{16B071BE-6482-45D8-93B0-DD6611049958}",
                 NodeNameOfConfiguration = "FingerveinCreateTemplateFB",
                 Name = "FingerveinCreateTemplateFB",
                 Author = "")]
    public class FingerveinCreateTemplateFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerveinCreateTemplateFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerveinCreateTemplateFB()
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
            object objFingerveinDataInfo = null;
            string Cif = string.Empty;
            VTMContext.CardHolderDataCache.Get("FingerveinDataInfo", out objFingerveinDataInfo, this.GetType());

            FingerveinDataInfo Fingerveindata = null;
            if (objFingerveinDataInfo is FingerveinDataInfo)
            {
                Log.Action.LogDebug("Fingervein CIF get from CIF query.");
                Fingerveindata = objFingerveinDataInfo as FingerveinDataInfo;
                Cif = Fingerveindata.CIF;
            }
            else
            {
                object customer_id = null;
                VTMContext.TransactionDataCache.Get("FB_CustomerId", out customer_id, GetType());
                if (customer_id != null)
                {
                    Cif = customer_id.ToString();
                }
                else
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    Log.Action.LogError("Fingervein Can not get CIF No.");
                    return emBusActivityResult_t.Success;
                }
            }
            FingerveinDevice service = new FingerveinDevice();
            string ImgBase64;
            string TemplateData;
            //int ret = CreateFingerveinTemplate(out ImgBase64, out TemplateData);
            int ret = service.CreateFingerveinTemplate(out ImgBase64, out TemplateData);
            if (ret != 0)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("CreateFingerveinTemplate Failed!");
                return emBusActivityResult_t.Success;
            }
            VTMContext.TransactionDataCache.Set("FB_FingerveinId", Cif, GetType());
            VTMContext.TransactionDataCache.Set("FB_FingerveinFeature", TemplateData, GetType());

            VTMContext.NextCondition = EventDictionary.s_EventContinue;

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        private DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        public long currentTime()
        {
            return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
        }       

        private byte[,] ToChangeByte(byte[][] text)
        {
            byte[,] array = new byte[3, 15000];
            for (int i = 0; i < text.Length && i < 120; i++)
            {
                for (int j = 0; j < text[i].Length; j++)
                {
                    //array[i, j] = charToByte(text[i][j])[1];
                    array[i, j] = text[i][j];
                }

            }
            return array;
        }

        #endregion
    }
}
