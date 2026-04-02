using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using VTMBusinessActivityBase;
using VTMBusinessServiceProtocol;
using VTMModelLibrary;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{7FF640D1-DC93-4889-B902-9CC594217B6F}",
        Name = "RestoreSavedAction",
        NodeNameOfConfiguration = "RestoreSavedAction",
        Author = "yslin3")]
    public class RestoreSavedAction : BusinessActivityVTMBase
    {
        #region constructor

        private RestoreSavedAction()
        {
        }

        #endregion

        #region create

        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new RestoreSavedAction();
        }

        #endregion

        #region Properties

        private string _actionBuffer;
        [GrgBindTarget("actionBuffer", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ActionBuffer
        {
            get { return _actionBuffer; }
            set
            {
                if (value != _actionBuffer)
                {
                    _actionBuffer = value;
                    OnPropertyChanged(nameof(ActionBuffer));
                }
            }
        }

        private string _workflowBuffer;
        [GrgBindTarget("workflowBuffer", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string WorkflowBuffer
        {
            get { return _workflowBuffer; }
            set
            {
                if (value != _workflowBuffer)
                {
                    _workflowBuffer = value;
                    OnPropertyChanged(nameof(WorkflowBuffer));
                }
            }
        }

        #endregion

        #region Methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            try
            {
                Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
                var emRet = base.InnerRun(argContext);
                if (emRet != emBusActivityResult_t.Success)
                {
                    Log.Action.LogError("Execute base InnerRun failed");
                    VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                    VTMContext.ActionResult = emBusActivityResult_t.Failure;
                    m_objContext.NextCondition = EventDictionary.s_EventFail;
                    return emRet;
                }
                SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);

                var bizCtx = argContext as VTMBusinessContext;
                if (bizCtx != null)
                {
                    var actionBuffer = string.IsNullOrWhiteSpace(ActionBuffer)
                        ? DataCacheKey.VtmDefaultActionBuffer
                        : ActionBuffer;
                    var workflowBuffer = string.IsNullOrWhiteSpace(WorkflowBuffer)
                        ? DataCacheKey.VtmDefaultWorkflowBuffer
                        : WorkflowBuffer;
                    object objSavedActionId;
                    string savedActionId;

                    if (bizCtx.TransactionDataCache.Get(actionBuffer, out objSavedActionId) &&
                        !string.IsNullOrWhiteSpace(savedActionId = objSavedActionId as string))
                    {
                        //Log.Action.LogDebug($"Succeed to get saved action's ID [{savedActionId}] from datacache [{actionBuffer}]!");
                        object objSavedWorkflowName;
                        string savedWorkflowName;
                        if (bizCtx.TransactionDataCache.Get(workflowBuffer, out objSavedWorkflowName) &&
                            !string.IsNullOrWhiteSpace(savedWorkflowName = objSavedWorkflowName as string))
                        {
                            //Log.Action.LogDebug($"Succeed to get saved workflow's name [{savedWorkflowName}] from datacache [{workflowBuffer}]!");

                            bizCtx.BusinessService.CurrentBusinessEngine.CancelWorkflow(bizCtx.CurrentWorkflowName);
                            bizCtx.BusinessService.CurrentBusinessEngine.PostWorkflow(savedWorkflowName, savedActionId, m_objContext);

                            bizCtx.NextCondition = EventDictionary.s_EventConfirm;
                        }
                        else
                        {
                            Log.Action.LogError($"Failed to get saved workflow's name from datacache [{workflowBuffer}]!");
                            argContext.NextCondition = EventDictionary.s_EventFail;
                        }
                    }
                    else
                    {
                        Log.Action.LogError($"Failed to get saved action's ID from datacache [{actionBuffer}]!");
                        argContext.NextCondition = EventDictionary.s_EventFail;
                    }
                }
                else
                {
                    Log.Action.LogError($"[{nameof(argContext)}] is not type [{nameof(VTMBusinessContext)}]!");
                    argContext.NextCondition = EventDictionary.s_EventFail;
                }

                return emBusActivityResult_t.Success;
            }
            finally
            {
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            }
        }

        #endregion
    }
}