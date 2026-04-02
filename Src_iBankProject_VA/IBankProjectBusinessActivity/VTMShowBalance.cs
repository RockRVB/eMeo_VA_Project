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
using DevServiceProtocol;
using BusinessActivityBaseImp;
using InputOrSelect;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{75BA4231-6E5A-4DB6-8E3B-DDB50826AA28}",
                 Name = "VTMShowBalance",
                 NodeNameOfConfiguration = "VTMShowBalance",
                 Author = "yhqing")]
    public class VTMShowBalance : BusinessActivitySelectFunction
    {
        #region constructor
        private VTMShowBalance()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static new IBusinessActivity Create()
        {
            return new VTMShowBalance();
        }
        #endregion

        #region property
        private string m_withdrawAmount = "";
        [GrgBindTarget("withdrawAmount", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string withdrawAmount
        {
            get { return m_withdrawAmount; }
            set
            {
                m_withdrawAmount = value;
                OnPropertyChanged("withdrawAmount");
            }
        }

        private string m_newBalance = "";
        [GrgBindTarget("newBalance", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string newBalance
        {
            get { return m_newBalance; }
            set
            {
                m_newBalance = value;
                OnPropertyChanged("newBalance");
            }
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Project.LogDebug("Enter action: VTMShowBalance");

            object obj_newBalance = null;

            m_objContext.TransactionDataCache.Get("newBalance",out obj_newBalance,GetType());
            if (obj_newBalance != null)
            {
                Log.Project.LogDebug("newBalance:" + obj_newBalance.ToString());
                newBalance = obj_newBalance.ToString();
            }

            object obj_withdrawAmount = null;
            m_objContext.TransactionDataCache.Get("core_RPTRAmount", out obj_withdrawAmount);
            if (obj_withdrawAmount != null)
            {
                Log.Project.LogDebug("withdrawAmount:" + obj_withdrawAmount.ToString());
                withdrawAmount = obj_withdrawAmount.ToString();
            }

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                m_objContext.LogJournal("Execute base inner run fail!", LogSymbol.Alert);
                Log.Project.LogDebug("Leave action: VTMShowBalance");
                return emRet;
            }

            return emBusActivityResult_t.Success;
        }


        #endregion
    }
}
