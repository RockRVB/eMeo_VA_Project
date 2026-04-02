using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using LogProcessorService;
using UIServiceProtocol;
using BusinessServiceProtocol;
using Attribute4ECAT;
using eCATBusinessServiceProtocol;
using eCATBusinessActivityBase;
using System.Threading;
using System.IO;
using VTMModelLibrary;
using VTMBusinessActivityBase;
using VTMBusinessActivity.common;
using VTMHelperService.common;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.Serialization.Json;

namespace VTMBusinessActivity
{
    [GrgActivity("{CA8EC870-DBCD-47A8-942E-D3D500AE7AA1}",
             NodeNameOfConfiguration = "GetInfoByID4Exhibition",
             Name = "GetInfoByID4Exhibition",
             Author = "wjw")]
    public class GetInfoByID4Exhibition : BusinessActivityVTMBase
    {
        #region property      

        private string m_MoblieUrl = "http://122.13.76.151:7080/grgweb/openaccount/getInfo.do";
        [GrgBindTarget("MoblieUrl", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string MoblieUrl
        {
            get
            {
                return m_MoblieUrl;
            }
            set
            {
                m_MoblieUrl = value;
                OnPropertyChanged("MoblieUrl");
            }
        }

        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new GetInfoByID4Exhibition() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected GetInfoByID4Exhibition()
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
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }
            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            object obj = null;
            string IdCard = "";
            m_objContext.TransactionDataCache.Get("ExhibitionIDNum", out obj, GetType());
            if (obj != null)
            {
                IdCard = obj.ToString();
            }
            string VipNo = string.Empty;
            string ret = HttpGetAppInfo(VipNo, IdCard);
            
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());

            return emBusActivityResult_t.Success;
        }


        #region 属性
        protected BusinessContext m_context;
        private eCATContext Context
        {
            get
            {
                return m_context as eCATContext;
            }
        }
        #endregion
        #region
        /// <summary>
        /// 调用手机端接口，返回用户填写信息
        /// </summary>
        /// <param name="IdCard">用户身份证信息</param>
        /// <returns></returns>
        private string HttpGetAppInfo(string VipNo, string IdCard)
        {
            string Url = MoblieUrl;
            string retData = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(Url + (VipNo == "" ? "" : "?vipno=") + VipNo + "&" + (IdCard == "" ? "" : "idcard=") + IdCard);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    retData = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(retData)))
                    {
                        DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(CreadCardInfo));
                        CreadCardInfo model = (CreadCardInfo)deseralizer.ReadObject(ms);
                        m_objContext.TransactionDataCache.Set("model", model, GetType());
                    }
                    Console.ReadKey();
                }     
            }
            catch (Exception ex)
            {
                Log.Action.LogError("Failed to get info from mobile: " + ex.ToString());
            }
            return retData;
        }
        #endregion

        #endregion
    }
}
