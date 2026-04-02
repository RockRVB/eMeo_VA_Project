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
using VTMModelLibrary;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{B7D83DD6-1EE5-4fda-89AD-729C325BAC91}",
                 NodeNameOfConfiguration = "ShowInfoToPage",
                 Name = "ShowInfoToPage",
                 Author = "Raymond")]
    public class ShowInfoToPage : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new ShowInfoToPage() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected ShowInfoToPage()
        {

        }
        #endregion

        private string m_info1 = "";
        [GrgBindTarget("info1", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string info1
        {
            get { return m_info1; }
            set
            {
                m_info1 = value;
                OnPropertyChanged("info1");
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
            object objInfo1 = null;
            VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_FINGER_USERNAME,out objInfo1);
            if (objInfo1 != null)
                info1 = objInfo1.ToString();
            else
                Log.Action.LogDebug("can not get username!");
            
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            emWaitSignalResult_t waitResult = WaitSignal();

            if (waitResult == emWaitSignalResult_t.Timeout)
            {
                m_objContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            else if (waitResult == emWaitSignalResult_t.Failure)
            {
                m_objContext.NextCondition = EventDictionary.s_EventFail;
            }


            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (result == emBusiCallbackResult_t.Swallowd)
            {
                return result;
            }

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    m_objContext.NextCondition = (string)objArg.Key;
                    SignalCancel();
                    return emBusiCallbackResult_t.Swallowd;
                }
            }

            return emBusiCallbackResult_t.Bypass;
        }

        #endregion

    }
   
}
