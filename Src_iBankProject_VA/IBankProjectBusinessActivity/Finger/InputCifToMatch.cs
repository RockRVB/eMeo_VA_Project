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
using FingerveinServerRequestService;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "InputCifToMatch",
                 Name = "InputCifToMatch",
                 Author = "Raymond")]
    public class InputCifToMatch : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new InputCifToMatch() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected InputCifToMatch()
        {

        }
        #endregion

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
                    Log.Action.LogDebugFormat("InputCifToMatch argUIEvent.Key is:{0}", strKey);
                    m_objContext.NextCondition = strKey;
                    m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
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
                    VTMContext.CardHolderDataCache.Set("FingerDataInfo", Fingerdata, this.GetType());

                    FingerveinDataInfo Fingerveindata = null;
                    object objFingerveinDataInfo = null;
                    VTMContext.CardHolderDataCache.Get("FingerveinDataInfo", out objFingerveinDataInfo, this.GetType());
                    if (objFingerveinDataInfo != null)
                    {
                        Fingerveindata = objFingerveinDataInfo as FingerveinDataInfo;
                    }
                    else
                    {
                        Fingerveindata = new FingerveinDataInfo();
                    }
                    Fingerveindata.CIF = cif;
                    VTMContext.CardHolderDataCache.Set("FingerveinDataInfo", Fingerveindata, this.GetType());
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }

        #endregion

    }
   
}
