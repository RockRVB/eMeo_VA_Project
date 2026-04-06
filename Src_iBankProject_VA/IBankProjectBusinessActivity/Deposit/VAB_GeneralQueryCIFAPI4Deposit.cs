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
    [GrgActivity("{CDA8ADDC-4517-4356-BBDB-F697011F9367}",
                 NodeNameOfConfiguration = "VAB_GeneralQueryCIFAPI4Deposit",
                 Name = "VAB_GeneralQueryCIFAPI4Deposit",
                 Author = "Louis")]
    public class VAB_GeneralQueryCIFAPI4Deposit : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VAB_GeneralQueryCIFAPI4Deposit() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected VAB_GeneralQueryCIFAPI4Deposit()
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

            if (GeneralQueryCIFAPI4Deposit())
            {
                VTMContext.NextCondition = EventDictionary.s_EventNext;
            }
            else
            {
                Log.Action.LogError("Get cif response data is empty");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        public bool GeneralQueryCIFAPI4Deposit()
        {
            bool result = false;
            try
            {
                ProjVTMContext.TransactionDataCache.Set("proj_TransType", "CASH_DEPOSIT", GetType());
                result = true;
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
            return result;
        }
    }
}
