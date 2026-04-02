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
using eCATBusinessActivityXDCBase;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{A75132AC-0F07-4F3B-8782-29F54825338F}",
                 NodeNameOfConfiguration = "GoXDCMainMenu",
                 Name = "GoXDCMainMenu",
                 Author = "")]
    public class GoXDCMainMenu : BusinessActivityXDCBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new GoXDCMainMenu() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected GoXDCMainMenu()
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

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState,2000);
            WaitSignal();
            SetCurrentState("014");

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
    }
}
