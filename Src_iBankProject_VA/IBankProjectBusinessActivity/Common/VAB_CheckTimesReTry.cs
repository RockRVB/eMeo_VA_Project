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
using System.Xml;

namespace IBankProjectBusinessActivity
{   
[GrgActivity("{43C72C08-9BC9-448D-8F86-DE79679613FD}",
                 NodeNameOfConfiguration = "BAB_CheckTimesReTry",
                 Name = "BAB_CheckTimesReTry",
                 Author = "Louis")]
    public class BAB_CheckTimesReTry : IBankProjectActivityBase
    {
        int SendTimeout = 0;
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BAB_CheckTimesReTry() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected BAB_CheckTimesReTry()
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
        

        private string m_TypeReTry = string.Empty;
        [GrgBindTarget("TypeReTry", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string TypeReTry
        {
            get
            {
                return m_TypeReTry;
            }
            set
            {
                m_TypeReTry = value;
                OnPropertyChanged("TypeReTry");
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

 /*           ProjVTMContext.RetryCounting ++;
            ProjVTMContext.MaxRetry = LoadConfig();
            Log.Action.LogDebugFormat("Retried {0} of {1} maximum times", ProjVTMContext.RetryCounting.ToString(), ProjVTMContext.MaxRetry.ToString());
            Log.Action.LogDebugFormat("Retried resend {0} of {1} maximum times", ProjVTMContext.ResendOTP.ToString(), SendTimeout.ToString());
            
            if (ProjVTMContext.ResendOTP <= SendTimeout)
            {
                if (ProjVTMContext.RetryCounting < ProjVTMContext.MaxRetry)
                {
                    m_objContext.NextCondition = "OnTryAgain";
                }
                else
                {
                    m_objContext.NextCondition = "OnTimesOverLimit";
                }
            }
            else
            {
                m_objContext.NextCondition = "OnTimesOverResendLimit";
            }*/
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        public int LoadConfig()
        {
            int maxRetryTimes = 3;
            try
            {
                string sTemp = string.Empty;
                string sTemp2 = string.Empty;
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + @"Config\BAB_STMConfig.xml");
                switch (TypeReTry)
                {
                    case "CWC":
                        sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["CWC"].Value;
                        break;
                    case "TFR":
                        sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["TFC"].Value;
                        break;
                    case "DEP":
                        sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["DEP"].Value;
                        break;
                    case "OTP":
                     /*   BAB_STMConfig.LoadCashWithdrawConfig();
                        sTemp = BAB_STMConfig.CashWithdrawConfig.Otp.MaxRetry;
                        sTemp2 = BAB_STMConfig.CashWithdrawConfig.Otp.MaxRequest;*/
                       // sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["OTP"].Value;
                       // sTemp2 = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["SendTimeout"].Value;
                        int.TryParse(sTemp2, out SendTimeout);
                        break;
                    case "Finger":
                        sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["Fingervein"].Value;
                        break;
                    case "QRScan":
                        sTemp = XmlDoc.SelectSingleNode(@"/Config/MaxRety").Attributes["QRScan"].Value;
                        break;
                }
                if (!string.IsNullOrEmpty(sTemp))
                {
                    int.TryParse(sTemp, out maxRetryTimes);
                }
            }
            catch (System.Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
            return maxRetryTimes;
        }
        #endregion

    }
}
