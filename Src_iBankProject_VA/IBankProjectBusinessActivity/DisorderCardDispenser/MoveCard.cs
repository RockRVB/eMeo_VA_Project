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
    [GrgActivity("{E718B14F-D0D3-4881-B517-4C368F06D9A0}",
                  Name = "MoveCard",
                  NodeNameOfConfiguration = "MoveCard")]
    public class MoveCard : BusinessActivityVTMBase
    {
        #region constructor
        protected MoveCard()
        {

        }
        #endregion
        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new MoveCard();
        }
        #endregion
        //1:从读卡器移动到非接位置 2:从非接移动到读卡器读卡位置 3:从读卡器移动到非接位置 4:从非接移动到读卡器读卡位置 

        private int m_moveCardWay = 3;
        [GrgBindTarget("MoveCardWay", Access = AccessRight.ReadAndWrite, Type = TargetType.Int)]
        public int MoveCardWay
        {
            get
            {
                return m_moveCardWay;
            }
            set
            {
                m_moveCardWay = value;
                OnPropertyChanged("MoveCardWay");
            }
        }
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
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);

            DevResult ret = VTMContext.CRDReader?.MoveCard(MoveCardWay);
            if (ret.IsSuccess)
            {
                Log.Action.LogInfoFormat("Move Card:{0}",MoveCardWay);
                VTMContext.NextCondition = EventDictionary.s_EventContinue;
            }
            else
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "DispenseCard can not Terminate";
            return false;
        }
        #endregion
    }
}
