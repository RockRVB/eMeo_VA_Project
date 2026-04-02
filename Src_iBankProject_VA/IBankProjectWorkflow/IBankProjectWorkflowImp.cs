using Attribute4ECAT;
using BusinessServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using LogProcessorService;
using System;
using VTMWorkflow;

namespace IBankProjectWorkflow
{
    [GrgComponent("{068A302D-5268-441C-8F2A-933DE97E7484}",
                   Name = "IBankProjectWorkflow",
                   Catalog = "IBankProjectWorkflow",
                   Author = "  ")]
    public class IBankProjectWorkflowImp : VTMWorkflowImp
    {
        #region constructor
        protected IBankProjectWorkflowImp()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static new IWorkflow Create()
        {
            return new IBankProjectWorkflowImp();
        }
        #endregion

        #region property
        public IBankProjectBusinessServiceContext ProjVTMContext
        {
            get
            {
                if (null == m_objContext)
                {
                    return null;
                }

                return (IBankProjectBusinessServiceContext)m_objContext;
            }
        }
        #endregion
        protected override emBusiCallbackResult_t InnerRunAction(BusinessContext objContext)
        {
            SetSessionRecordService();
            var result = base.InnerRunAction(objContext);
            return result;
        }
        private void SetSessionRecordService()
        {
            try
            {
                if (ProjVTMContext.SessionRecordService == null)
                {
                    return;
                }

                ProjVTMContext.SessionRecordService.UpdateSession();

                //var startTime = DateTime.Now;
                if (ProjVTMContext.CurrentWorkflowName == "Main" && ProjVTMContext.ActionID == "ID_ZeroState" && ProjVTMContext.LastCondition == "OnStart")
                {
                    ProjVTMContext.SessionRecordService.StartNewSession();
                }
                else if (ProjVTMContext.CurrentWorkflowName == "CloseTrans" && ProjVTMContext.ActionID == "0")
                {
                    ProjVTMContext.SessionRecordService.StopSession();
                }
                //var endTime = DateTime.Now;
                //Log.Workflow.LogDebug($"SetSessionRecordService() costed time: {(endTime - startTime).TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Log.Workflow.LogError(ex.ToString());
            }
        }
    }
}
