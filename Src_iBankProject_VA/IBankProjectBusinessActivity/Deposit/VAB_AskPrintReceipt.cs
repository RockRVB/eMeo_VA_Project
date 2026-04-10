
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
using IBankProjectBusinessActivityBase;
using RemoteTellerServiceProtocol;
using UIServiceProtocol;
using VTMModelLibrary;
using System.Web.Script.Serialization;
using IBankProjectBusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    
[GrgActivity("{4E4E1BA0-5941-4B3D-8096-18E0C75F5C01}",
                     NodeNameOfConfiguration = "VAB_AskPrintReceipt",
                     Name = "VAB_AskPrintReceipt",
                     Author = "Louis")]
    public class VAB_AskPrintReceipt : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VAB_AskPrintReceipt() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected VAB_AskPrintReceipt()
        {

        }
        #endregion
        #region property

        private string m_CacheType = "1";
        [GrgBindTarget("CacheType", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CacheType
        {
            get
            {
                return m_CacheType;
            }
            set
            {
                m_CacheType = value;
                OnPropertyChanged("CacheType");
            }
        }
        private string m_InputVal = string.Empty;
        [GrgBindTarget("input_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string input_val
        {
            get
            {
                return m_InputVal;
            }
            set
            {
                m_InputVal = value;
                OnPropertyChanged("input_val");
            }
        }
        private string m_OutputVal = string.Empty;
        [GrgBindTarget("output_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string output_val
        {
            get
            {
                return m_OutputVal;
            }
            set
            {
                m_OutputVal = value;
                
                OnPropertyChanged("output_val");
            }
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
            FeeShowing feeShowing = new FeeShowing();
            Fee fee = new Fee();
            object objFee = null;
            List<Fee> lstFee = new List<Fee>();
            lstFee.Clear();
            ProjVTMContext.TransactionDataCache.Get("VAB_Fee", out objFee, GetType());
            if (objFee != null)
            {
                lstFee = objFee as List<Fee>;
                foreach (var item in lstFee)
                {
                    if (item.FeeCode == "RECEIPT_FEE")
                    {
                        feeShowing.fee = fee.FeeAmount;
                        break;
                    }
                }
            }
            input_val = new JavaScriptSerializer().Serialize(feeShowing);

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t waitResult = WaitSignal();
            if (waitResult == emWaitSignalResult_t.Timeout)
            {
                ProjVTMContext.NextCondition = EventDictionary.s_EventTimeout;
                return emBusActivityResult_t.Success;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
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
                    if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase) || key.Equals(EventDictionary.s_EventNext, StringComparison.OrdinalIgnoreCase))
                    {
                        m_objContext.NextCondition = EventDictionary.s_EventConfirm;
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
    public class FeeShowing
    {
        public string fee { get; set; }
    }
}

