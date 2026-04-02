using BusinessServiceProtocol;
using FaceRecognitionService4SmartKioskProtocol;
using LogProcessorService;
using SessionRecordServiceProtocol.Interfaces;
using VTMBusinessServiceProtocol;

namespace IBankProjectBusinessServiceProtocol
{
    /// <summary>
    /// 业务数据上下文，俗称数据总线
    /// </summary>
    public class IBankProjectBusinessServiceContext : VTMBusinessContext
    {
        #region constructor
        public IBankProjectBusinessServiceContext()
        {

        }
        #endregion
        private ISessionRecordService _sessionRecordService;
        private CustomerInfo m_CustomerInfo;
        public ISessionRecordService SessionRecordService
        {
            get
            {
                const string serviceName = "SessionRecordService";
                if (_sessionRecordService == null)
                {
                    IStandardFeatureService iResult;
                    if (BusinessService.QueryStandardFeature(serviceName, out iResult))
                    {
                        _sessionRecordService = (ISessionRecordService)iResult;
                        //Log.BusinessService.LogDebugFormat("Succeed to query standard feature [{0}]!", serviceName);
                    }
                    else
                    {
                        //Log.BusinessService.LogErrorFormat("Failed to query standard feature [{0}]!", serviceName);
                    }
                }
                return _sessionRecordService;
            }
        }
        public new CustomerInfo CustomerInfo
        {
            get
            {
                if (m_CustomerInfo == null)
                {
                    m_CustomerInfo = new CustomerInfo();
                }
                return m_CustomerInfo;
            }
            set
            {
                m_CustomerInfo = value;
            }
        }
    }
}
