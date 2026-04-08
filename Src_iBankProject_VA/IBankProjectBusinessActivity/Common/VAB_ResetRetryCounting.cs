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
using UIServiceProtocol;
using FaceRecognizeServiceProtocol;
using IBankProjectBusinessActivityBase;

namespace IBankProjectBusinessActivity
{

    [GrgActivity("{CA15DCB6-0E9F-43E5-9760-8C6AB1895C14}",
                 NodeNameOfConfiguration = "BAB_ResetRetryCounting",
                 Name = "BAB_ResetRetryCounting",
                 Author = "Louis")]
    public class BAB_ResetRetryCounting : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BAB_ResetRetryCounting() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected BAB_ResetRetryCounting()
        {

        }
        #endregion

        #region property
        

        private string m_CacheType = "1";
        [GrgBindTarget("CacheType", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CacheType
        {
            get
            {
                return m_CacheType;
            }
            set
            {
                m_CacheType = value;
                OnPropertyChanged("CacheType");
            }
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
      /*      ProjVTMContext.RetryCounting = 0;
            ProjVTMContext.ResendOTP = 0;
            ProjVTMContext.NeedShowResendOTPButton = false;
            ProjVTMContext.ShowedResendOTPButton = false;
            */
            Log.Action.LogDebugFormat("Reset retry counting to 0");
            ProjVTMContext.NextCondition = EventDictionary.s_EventConfirm;
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion

    }
}
