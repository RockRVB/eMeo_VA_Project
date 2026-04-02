using Attribute4ECAT;
using BusinessServiceProtocol;
using CardDispenserDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMBusinessServiceProtocol;
using VTMModelLibrary;

namespace VTMBusinessActivity
{
    [GrgActivity("{2B70767B-D529-449C-941F-F131DEEAE096}",
                  Name = "SetAddCardFailedTimes4Exhibition",
                  NodeNameOfConfiguration = "SetAddCardFailedTimes4Exhibition")]
    public class SetAddCardFailedTimes4Exhibition : BusinessActivityVTMBase
    {
        #region constructor
        private SetAddCardFailedTimes4Exhibition()
        {

        }
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new SetAddCardFailedTimes4Exhibition();
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            if (Way == "1")
            {
                VTMContext.AddCardFailedTimes += 1;
                Log.Action.LogInfo($"AddCardFailedTimes is {VTMContext.AddCardFailedTimes}");
            }
            else if(Way == "2")
            {
                Log.Action.LogInfo("AddCardFailedTimes is 0");
                VTMContext.AddCardFailedTimes = 0;
            }

            VTMContext.NextCondition = EventDictionary.s_EventConfirm;

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCard can not Terminate";
            return false;
        }
        //1:增加 2:清零
        private string m_way = "1";
        [GrgBindTarget("Way", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string Way
        {
            get
            {
                return m_way;
            }
            set
            {
                m_way = value;
                OnPropertyChanged("Way");
            }
        }
        #endregion
    }
}
