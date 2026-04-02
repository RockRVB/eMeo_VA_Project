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

namespace VTMBusinessActivity.common
{
    [GrgActivity("{FD39FCF9-325C-46EF-AC93-26CBE5F9A3CA}",
                 NodeNameOfConfiguration = "WaitSleep",
                 Name = "WaitSleep",
                 Author = "ltfei1")]
    public class WaitSleep : BusinessActivityVTMBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new WaitSleep() as IBusinessActivity;
        }
        #endregion
        #region constructor
        protected WaitSleep()
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
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState); 
            //获得当前语言
            Thread.Sleep(m_objContext.ActionTimeout-1000);

            m_objContext.NextCondition = EventDictionary.s_EventContinue;
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        #endregion

    }
}
