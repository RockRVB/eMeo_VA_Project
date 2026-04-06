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
using DevServiceProtocol;
using OCRServiceProtocol;

namespace IBankProjectBusinessActivity
{

    [GrgActivity("{ED07C2E9-70EB-4CCF-8AC8-B883B0D8DF64}",
                 NodeNameOfConfiguration = "VAB_CWDCheckRemain",
                 Name = "VAB_CWDCheckRemain")]
    public class VAB_CWDCheckRemain : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VAB_CWDCheckRemain() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected VAB_CWDCheckRemain()
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
       //     m_objContext.OCRService.SaveOCRInfo(6, OCRCommandType.Dispense);
            int totalremain = 0;
            List<CashUnitInfo> listCashUnitInfo = GetCashUnitInfo(2);
            if (listCashUnitInfo.Count == 0)
            {
                m_objContext.NextCondition = EventDictionary.s_EventCancel;
                Log.Project.LogDebug("Leave action: VAB_CWDCheckRemain because no cassette");
                return emBusActivityResult_t.Success;
            }
            else
            {
                foreach (CashUnitInfo item in listCashUnitInfo)
                {
                    if (item.DenoValue == 500000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;
                        continue;
                    }
                    if (item.DenoValue == 200000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;
                        continue;
                    }
                    if (item.DenoValue == 100000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;
                        continue;
                    }
                    if (item.DenoValue == 50000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;

                        continue;
                    }
                    if (item.DenoValue == 20000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;

                        continue;
                    }
                    if (item.DenoValue == 10000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                    {
                        totalremain = totalremain + item.Count * item.DenoValue;
                        continue;
                    }
                }
                Log.Project.LogDebug("total remain = " + totalremain.ToString());
            }
            if (totalremain >= 50000)
            {
                m_objContext.NextCondition = EventDictionary.s_EventContinue;
                Log.Project.LogDebug("Leave action: VAB_CWDCheckRemain because total remain >= 50000");
                return emBusActivityResult_t.Success;
            }
            else
            {
                ShowAndWait("CRMNotAvailable");
                m_objContext.NextCondition = EventDictionary.s_EventCancel;
                Log.Project.LogDebug("Leave action: VAB_CWDCheckRemain because total remain = 0");
                return emBusActivityResult_t.Success;
            }
            return emBusActivityResult_t.Success;
        }
        protected override emBusActivityResult_t InnerPreRun(BusinessContext argContext)
        {

            base.InnerPreRun(argContext);
            m_objContext = (eCATContext)argContext;



            Log.Project.LogDebug("Leave action: InnerPreRun VAB_CWDCheckRemain");
            return emBusActivityResult_t.Success;
        }

        public virtual void ShowAndWait(string argUIName)
        {
            if (m_objContext.UIState.ContainsKey(argUIName))
            {
                if (SwitchUIState(m_objContext.MainUI, argUIName, 4000))
                    WaitSignal();
            }
        }
        #endregion

    }
}
