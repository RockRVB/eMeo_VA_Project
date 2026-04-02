using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using LogProcessorService;

namespace IBankProjectBusinessActivityBase
{
    public class IBankProjectActivityBase : BusinessActivityVTMBase
    {
        #region constructor
        public IBankProjectActivityBase()
        {

        }
        #endregion

        private IBankProjectBusinessServiceContext m_ProjVTMContext = null;
        public virtual IBankProjectBusinessServiceContext ProjVTMContext
        {
            get
            {
                return m_ProjVTMContext;
            }
        }

        protected override emBusActivityResult_t InnerPreRun(BusinessContext objContext)
        {

            m_ProjVTMContext = (IBankProjectBusinessServiceContext)objContext;
            return base.InnerPreRun(objContext);
        }

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }

            return emBusActivityResult_t.Success;
        }
    }
}


