using System;
using System.Collections.Generic;
using System.Threading;
using Attribute4ECAT;
using BusinessServiceProtocol;
using CardReaderDeviceProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using ResourceManagerProtocol;
using UIServiceProtocol;
using System.IO;
using eCATBusinessActivityBase;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Drawing;
using System.Net.Sockets;
using VTMModelLibrary;

namespace BankInterface
{
    [GrgActivity("{AC0E2E1A-F82B-43ED-BDF0-2177F5988CB0}",
                  Name = "HttpGet",
                  NodeNameOfConfiguration = "HttpGet",
                  Author = "")]
    public class BusinessActivityHttpGet : BusinessActivityeCATBase
    {
        #region properties
        private string m_Url = string.Empty;
        private bool bgetResult = true;
        [GrgBindTarget("Url", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string Url
        {
            get
            {
                return m_Url;
            }
            set
            {
                m_Url = value;
                OnPropertyChanged("Url");
            }
        }
        public class Result
        {
            public string result { get; set; }

        }
        #endregion

        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityHttpGet();
        }
        #endregion

        #region constructor
        public BusinessActivityHttpGet()
        {
        }
        #endregion

        #region methods
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogError("execute base InnerRun failed");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }

            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

            string ip = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[ServerIP]");
            string port = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[CheckPort]");
            string path = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[CheckPath]");

            string Url = @"http://" + ip + ":" + port + path + m_objContext.TerminalConfig.Terminal.ATMNumber;

            string getResult = string.Empty;

            TimeSpan ts = new TimeSpan();
            if (string.IsNullOrEmpty(Url))
            {
                Log.Action.LogError("The url is null or empty!");
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
               
                do
                {
                    string data = HttpGet(Url);
                    //Log.Action.LogDebugFormat("HttpGet from {0}, result: {1}", Url, data);
                    m_objContext.TransactionDataCache.Set("proj_HttpGetResult", data, GetType());
                    Result rt = JsonConvert.DeserializeObject<Result>(data);
                    getResult = rt.result;
                    if (getResult == "0" || getResult == "1" || getResult == "2")
                    {
                        bgetResult = false;
                    }
                    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
                    ts = ts1.Subtract(ts2).Duration();
                } while(bgetResult && ts.TotalMinutes <30) ;
            }
            catch (Exception ex)
            {
                Log.Action.LogDebugFormat("Action HttpGet exception: {0}", ex.Message);
                //SendTransactionDoneeState();
                m_objContext.NextCondition = EventDictionary.s_EventFail;
                return emRet;
            }
            if(bgetResult)
            {
                Log.Action.LogDebugFormat("Action HttpGet TimeOut");
                //SendTransactionDoneeState();
                m_objContext.NextCondition = EventDictionary.s_EventTimeout;
            }
            else
            {
                //Log.Action.LogDebug("getResult: " + getResult);
                //SendTransactionDoneeState();
                switch (getResult)
                {
                    case "0":
                        m_objContext.NextCondition = EventDictionary.s_EventContinue;
                        break;
                    case "1":
                        m_objContext.NextCondition = EventDictionary.s_EventFail;
                        break;
                    case "2":
                        m_objContext.NextCondition = EventDictionary.s_EventFallback;
                        break;
                    default:
                        m_objContext.NextCondition = EventDictionary.s_EventFail;
                        break;

                }            
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        #endregion

        public static string HttpGet(string url)
        {
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

