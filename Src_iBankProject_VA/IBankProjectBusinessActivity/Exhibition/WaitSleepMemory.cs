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
using System.Diagnostics;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{98C10025-A13C-45E1-80C5-230CB92A2273}",
                 NodeNameOfConfiguration = "WaitSleepMemory",
                 Name = "WaitSleepMemory",
                 Author = "ltfei1")]
    public class WaitSleepMemory : BusinessActivityVTMBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new WaitSleepMemory() as IBusinessActivity;
        }
        #endregion
        #region constructor
        protected WaitSleepMemory()
        {

        }
        #endregion
        Process proc;
        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            // Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
           
            proc = Process.GetCurrentProcess();
            long usedMemory = proc.PrivateMemorySize64/1024;
            object obj;
            m_objContext.TransactionDataCache.Get("vtm_PreMemory", out obj, GetType());
            long pre=0;
            if (null != obj)
            {
                pre = (long)obj;
            }
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            //获得当前语言
            Thread.Sleep(m_objContext.ActionTimeout - 1000);
            proc = Process.GetCurrentProcess();
            long usedMemory1 = proc.PrivateMemorySize64/1024;
            Log.Action.LogDebugFormat("当前actionId:{0}, 当前UIName:{1},切换界面前内存:{2},切换界面后内存:{3},内存差:{4},与上一action内存差:{5}", this.m_objContext.ActionID, this.m_objContext.ActionUIName, usedMemory, usedMemory1, (usedMemory1 - usedMemory), usedMemory1 - pre);
            m_objContext.TransactionDataCache.Set("vtm_PreMemory", usedMemory1,GetType());
            m_objContext.NextCondition = EventDictionary.s_EventContinue;
            // Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        #endregion

    }
}
