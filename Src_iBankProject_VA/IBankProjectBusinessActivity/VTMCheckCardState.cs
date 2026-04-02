using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using eCATBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using eCATBusinessServiceProtocol;
using VTMBusinessServiceProtocol;
using System.Threading;
using System.Collections.ObjectModel;
using IBankProjectBusinessActivityBase;
using VTMBusinessActivityBase;
using RemoteTellerServiceProtocol;
using CardReaderDeviceProtocol;
using DevServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{5A911BB6-007B-433D-B919-1A8094CC7448}",
                 Name = "VTMCheckCardState",
                 NodeNameOfConfiguration = "VTMCheckCardState",
                 Author = "wychao")]
    public class VTMCheckCardState : IBankProjectActivityBase
    {

        #region constructor
        private VTMCheckCardState()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VTMCheckCardState();
        }
        #endregion

        #region property
       
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {

            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                ProjVTMContext.CurrentTransactionResult = TransactionResult.Fail;
                ProjVTMContext.ActionResult = emBusActivityResult_t.Failure;
                ProjVTMContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            m_objContext.NextCondition = EventDictionary.s_EventFail;

            object outValue = null;
            if (ExecuteDevCommand(CARDREADERDEVICECMD.s_GetStatusInfoCmd, out outValue))
            {
                GrgCmdIDCStatusInfo IDCStatus = outValue as GrgCmdIDCStatusInfo;
                if ((HardwareState.Online == IDCStatus.DeviceState || HardwareState.Busy == IDCStatus.DeviceState) && (MediaState.Present == IDCStatus.MediaState))
                {
                    m_objContext.NextCondition = EventDictionary.s_EventYes;
                }
                else
                {
                    m_objContext.NextCondition = EventDictionary.s_EventNo;
                }
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}
