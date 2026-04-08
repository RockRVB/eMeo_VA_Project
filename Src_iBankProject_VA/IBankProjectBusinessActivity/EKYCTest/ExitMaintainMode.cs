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
using XDCInstance;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{A43F9F60-9EED-4760-9148-2285FE98CDE0}",
                 NodeNameOfConfiguration = "ExitMaintainMode",
                 Name = "ExitMaintainMode",
                 Author = "Logan")]
    public class ExitMaintainMode : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new ExitMaintainMode() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected ExitMaintainMode()
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

            Log.Action.LogDebug("Start Set event");
            OXDCInst.xdcServer.oXDCStautsMgr.vSetEvent(XDCDataDefinitionProtocol.EDataEvent.eModeExitMaintenance, null);
            Log.Action.LogDebug("Set event exit maintainMode successfully");

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
