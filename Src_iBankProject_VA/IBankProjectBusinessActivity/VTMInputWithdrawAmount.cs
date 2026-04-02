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
using UIServiceProtocol;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{C9596375-5173-40FE-B534-C0D83D6FE04E}",
                 Name = "VTMInputWithdrawAmount",
                 NodeNameOfConfiguration = "VTMInputWithdrawAmount",
                 Author = "yhqing")]
    public class VTMInputWithdrawAmount : BusinessActivityInputWithdrawalAmount
    {
        #region constructor
        private VTMInputWithdrawAmount()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static new IBusinessActivity Create()
        {
            return new VTMInputWithdrawAmount();
        }
        #endregion

        private string m_testInput = "";
        [GrgBindTarget("testInput", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string testInput
        {
            get { return m_testInput; }
            set
            {
                m_testInput = value;
                OnPropertyChanged("testInput");
            }
        }

        public override void HandleSideKey(string argEvent, string argKey)
        {
            if(argKey == "OnConfirm")
            {
                //解决页面值回传给后端的问题
                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            }
            base.HandleSideKey(argEvent, argKey);
            
        }

        public override bool HandlePinPadKey(string argKey)
        {
            if ("ENTER" == argKey)
            {
                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            }
            return base.HandlePinPadKey(argKey);
        }
    }
}
