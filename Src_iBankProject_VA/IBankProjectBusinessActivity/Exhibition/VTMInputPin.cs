using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using InputOrSelect;
using eCATBusinessServiceProtocol;
using BusinessServiceProtocol;
using LogProcessorService;
using VTMBusinessServiceProtocol;
using CardReaderDeviceProtocol;
using DevServiceProtocol;
using ResourceManagerProtocol;
using EPPKeypadDeviceProtocol;
using UIServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{AF8EBC26-54F6-4D82-9BF0-537D3B5C8A1A}",
               Name = "VTMInputPin",
               Description = "VTMInputPin",
               NodeNameOfConfiguration = "VTMInputPin",
               Catalog = "InputOrSelect",
               ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventTimeout, EventDictionary.s_EventCancel })]
    public class VTMInputPin : BusinessActivityInputPin
    {

        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VTMInputPin() as IBusinessActivity;
        }
        #endregion

        #region constructor
        public VTMInputPin()
        {
        }
        #endregion

        #region verride methods of base

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            SetLight(GuidLight.PinPad, GuidLightFlashMode.Continuous);

            emBusActivityResult_t result = base.InnerRun(argContext);

            #region add by lmjun2 20171212 显示设备异常提示界面
            if (argContext.NextCondition == EventDictionary.s_EventHardwareError)
            {
                SwitchUIState(m_objContext.MainUI, "HardwareError", 3000);
                WaitSignal();
            } 
            #endregion

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            SetLight(GuidLight.PinPad, GuidLightFlashMode.Off);
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        //protected override bool InnerCanTerminate(ref string strMsg)
        //{
        //    strMsg = "VTMInputPin can not Terminate";
        //    return false;
        //}

        protected override void InnerTerminate(bool argIsUserCancel)
        {
            ExecuteDevCommand(EPPKeypadCmds.s_CancelGetPinCmd);
            m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            base.InnerTerminate(argIsUserCancel);
        }
        #endregion
    }
}
