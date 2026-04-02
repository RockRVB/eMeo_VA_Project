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
using IBankProjectBusinessActivityBase;
using RemoteTellerServiceProtocol;
using Newtonsoft.Json;
using FingerServerRequestService;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "InputFingerInfoToRegister",
                 Name = "InputFingerInfoToRegister",
                 Author = "Raymond")]
    public class InputFingerInfoToRegister : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new InputFingerInfoToRegister() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected InputFingerInfoToRegister()
        {

        }
        #endregion

        private string m_RegisterName = "";
        [GrgBindTarget("registerName", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string registerName
        {
            get { return m_RegisterName; }
            set
            {
                m_RegisterName = value;
                OnPropertyChanged("registerName");
            }
        }

        private string m_Cif = "";
        [GrgBindTarget("cif", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string cif
        {
            get { return m_Cif; }
            set
            {
                m_Cif = value;
                OnPropertyChanged("cif");
            }
        }

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

            emWaitSignalResult_t emWaitResult = WaitSignal();

            if (emWaitResult == emWaitSignalResult_t.Timeout)
            {
                VTMContext.ActionResult = emBusActivityResult_t.Timeout;
                VTMContext.NextCondition = EventDictionary.s_EventTimeout;
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
                    m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                    Log.Action.LogDebugFormat("InputFingerInfoToRegister argUIEvent.Key is:{0}", strKey);
                    m_objContext.TransactionDataCache.Set("FingerRegisterName", m_RegisterName, GetType());
                    m_objContext.TransactionDataCache.Set("FingerRegisterCif", m_Cif, GetType());
                    m_objContext.NextCondition = strKey;

                    FingerDataInfo Fingerdata = null;
                    object objFingerDataInfo = null;
                    VTMContext.CardHolderDataCache.Get("FingerDataInfo", out objFingerDataInfo, this.GetType());
                    if (objFingerDataInfo != null)
                    {
                        Fingerdata = objFingerDataInfo as FingerDataInfo;
                    }
                    else
                    {
                        Fingerdata = new FingerDataInfo();
                    }
                    Fingerdata.Cif = cif;
                    Fingerdata.username = registerName;
                    VTMContext.CardHolderDataCache.Set("FingerDataInfo", Fingerdata, this.GetType());
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        #endregion

    }
   
}
