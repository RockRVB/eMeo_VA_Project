using System;
using LogProcessorService;
using UIServiceProtocol;
using BusinessServiceProtocol;
using Attribute4ECAT;
using eCATBusinessServiceProtocol;
using VTMBusinessActivityBase;
using RemoteTellerServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{CA8EC870-DBCD-47A8-942E-D3D500AE7AA1}",
             NodeNameOfConfiguration = "InputIDNum4Exhibition",
             Name = "InputIDNum4Exhibition",
             Author = "wjw")]
    public class InputIDNum4Exhibition : BusinessActivityVTMBase
    {
        #region property
        private string m_CustomerIDNumber = "";
        [GrgBindTarget("CustomerIDNumber", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CustomerIDNumber
        {
            get
            {
                return m_CustomerIDNumber;
            }
            set
            {
                m_CustomerIDNumber = value;
                OnPropertyChanged("CustomerIDNumber");
            }
        }

        private string m_CustomerIDNumber_Error = string.Empty;
        [GrgBindTarget("CustomerIDNumber_Error", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CustomerIDNumber_Error
        {
            get
            {
                return m_CustomerIDNumber_Error;
            }
            set
            {
                m_CustomerIDNumber_Error = value;
                OnPropertyChanged("CustomerIDNumber_Error");
            }
        }

        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new InputIDNum4Exhibition() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected InputIDNum4Exhibition()
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
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            emWaitSignalResult_t waitResult = VTMWaitSignal();

            if (waitResult == emWaitSignalResult_t.Timeout)
            {
                m_objContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            //string IdCard = CustomerIDNumber;
            //string VipNo = null;
            //HttpGetAppInfo(VipNo, IdCard);
            return emBusActivityResult_t.Success;
        }


        #region 属性
        protected BusinessContext m_context;
        private eCATContext Context
        {
            get
            {
                return m_context as eCATContext;
            }
        }
        #endregion

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);

            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }
            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                    {
                        string strMessage = string.Empty;
                        CustomerIDNumber_Error = string.Empty;
                        m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                        if (CustomerIDNumber != null)
                        {
                            m_objContext.NextCondition = EventDictionary.s_EventConfirm;
                            m_objContext.TransactionDataCache.Set("ExhibitionIDNum", CustomerIDNumber, GetType());
                        }
                        //   VTMContext.IDCardInfo.IDNumber = CustomerIDNumber;
                        SignalCancel();
                        return emBusiCallbackResult_t.Swallowd;
                    }
                    else
                    {
                        m_objContext.NextCondition = key;
                        SignalCancel();
                    }
                }
            }
            return emBusiCallbackResult_t.Swallowd;
        }
        #endregion
    }
}
