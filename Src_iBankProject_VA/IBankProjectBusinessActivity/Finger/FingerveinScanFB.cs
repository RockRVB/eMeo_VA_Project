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
    [GrgActivity("{6043E11B-0D52-4823-9838-04ABB101ECD0}",
                 NodeNameOfConfiguration = "FingerveinScanFB",
                 Name = "FingerveinScanFB",
                 Author = "")]
    public class FingerveinScanFB : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new FingerveinScanFB() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected FingerveinScanFB()
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
                if(customer_id != null)
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

           
            FingerveinDevice fingerveinDev = new FingerveinDevice();
            string featureData = string.Empty;
            int emResult = fingerveinDev.GetFingerveinFeature(out featureData);
            if (emResult != 0)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("GetFingerveinFeature Failed!");
                return emBusActivityResult_t.Success;
            }
            if (string.IsNullOrEmpty(featureData))
            {
                Log.Action.LogInfoFormat("HanlderGetFingerveinFeature() featureData is null or empty");
            }
            VTMContext.TransactionDataCache.Set("FB_FingerveinId", Cif, GetType());
            VTMContext.TransactionDataCache.Set("FB_FingerveinFeature", featureData, GetType());

            VTMContext.NextCondition = EventDictionary.s_EventContinue;

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}
