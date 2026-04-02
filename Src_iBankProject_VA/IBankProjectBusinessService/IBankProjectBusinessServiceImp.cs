using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessService;
using eCATProtocol;
using IBankProjectBusinessServiceProtocol;
using VTMBusinessService;

namespace IBankProjectBusinessServiceImp
{
    [GrgComponent("{FCF3EBDD-423C-4B75-AAE0-6A71E52480F5}",
                    Name = "IBankProjectBusinessService",
                    Catalog = "IBankProjectBusinessService")]
    public class IBankProjectBusinessServiceImp : VTMBusinessServiceImp
    {
        #region function for creating
        [GrgCreateFunction("Create the business service for Project")]
        public static new IBusinessService Create()
        {
            return new IBankProjectBusinessServiceImp() as IBusinessService;
        }
        #endregion

        #region constructor
        protected IBankProjectBusinessServiceImp()
        {

        }
        #endregion

        #region overide method of the base class
        protected override void InitContext()
        {
            m_objConfig = new Configuration4ECATBusinessService();
            m_objContext = new IBankProjectBusinessServiceContext();
        }

        protected override void InnerClose()
        {
            base.InnerClose();
        }

        protected override bool InnerOpen(string strCfg, IECATPlatform iECATPlatform)
        {
            return base.InnerOpen(strCfg, iECATPlatform);
        }
        #endregion
    }
}