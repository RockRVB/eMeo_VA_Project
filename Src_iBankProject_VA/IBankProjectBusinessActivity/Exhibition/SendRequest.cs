using System;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using eCATBusinessServiceProtocol;
using DevServiceProtocol;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using VTMModelLibrary.common;
using CardReaderDeviceProtocol;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using VTMModelLibrary;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{550D2347-CD1D-491F-8EED-6CD4B079D6B0}",
                 NodeNameOfConfiguration = "SendRequest",
                 Name = "SendRequest",
                 Author = "ltfei1")]
    public class SendRequest : BusinessActivityVTMBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new SendRequest() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected SendRequest()
        {

        }
        #endregion

        private string m_msgType = string.Empty;
        [GrgBindTarget("msgType", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string MsgType
        {
            get
            {
                return m_msgType;
            }
            set
            {
                m_msgType = value;
                OnPropertyChanged("msgType");
            }
        }


        string separator = "\u001F";
        CreadCardInputInfo FormInfo = null;
        ScanIDCardInfo sidc = null;

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
            else
            {
                try
                {
                    using (System.Net.Sockets.Socket tcpClient = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        string ip = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[ServerIP]");
                        string port = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[RequestPort]");

                        IPAddress ipaddress = IPAddress.Parse(ip);
                        EndPoint point = new IPEndPoint(ipaddress, int.Parse(port));

                        tcpClient.Connect(point);

                        byte[] data = new byte[1024];
                        // int length = tcpClient.Receive(data);
                        //string message = Encoding.UTF8.GetString(data, 0, length);
                        object objtemp = null;
                        string userName = String.Empty;
                        m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out objtemp, GetType());
                        if (null != objtemp)
                        {
                            sidc = objtemp as ScanIDCardInfo;
                            userName = sidc.IdCard_Name;
                        }

                        object objFormInfo = null;
                        m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_CARDFORM, out objFormInfo, GetType());
                        if (null != objFormInfo)
                        {
                            FormInfo = objFormInfo as CreadCardInputInfo;
                        }

                        string transType = string.Empty;
                        object value = null;
                        m_objContext.TransactionDataCache.Get("proj_TransType", out value, GetType());
                        if (null != value)
                        {
                            transType = value as string;
                        }

                        string fileType = string.Empty;
                        object objType = null;
                        m_objContext.TransactionDataCache.Get("proj_FileType", out objType, GetType());
                        if (null != objType)
                        {
                            fileType = objType as string;
                        }
                        else
                        {
                            Log.Action.LogWarn("proj_FileType is null");
                            fileType = "jpg";
                        }

                        if (string.IsNullOrWhiteSpace(fileType))
                        {
                            Log.Action.LogWarn("proj_FileType is null or empty");
                            fileType = "jpg";
                        }

                        StringBuilder msgBuilder = new StringBuilder();

                        msgBuilder.Append(MsgType).Append(separator);//审核类型
                        msgBuilder.Append(m_objContext.TerminalConfig.Terminal.ATMNumber).Append(separator);//terminal id

                        if (string.IsNullOrEmpty(transType)) { transType = "null"; }
                        msgBuilder.Append(transType).Append(separator);//TransType

                        if (string.IsNullOrEmpty(userName)) { userName = "null"; }
                        msgBuilder.Append(userName).Append(separator);//customer name

                        if (null != FormInfo)
                        {
                            if (string.IsNullOrEmpty(FormInfo.PhoneNo))
                            {
                                msgBuilder.Append("null").Append(separator);
                            }
                            else
                            {
                                msgBuilder.Append(FormInfo.PhoneNo).Append(separator);
                            }

                            if (string.IsNullOrEmpty(FormInfo.CustomAddress))
                            {
                                msgBuilder.Append("null").Append(separator);
                            }
                            else
                            {
                                msgBuilder.Append(FormInfo.CustomAddress).Append(separator);
                            }

                            msgBuilder.Append(fileType).Append(separator).Append(GetOtherInfo());
                        }
                        else
                        {
                            msgBuilder.Append("null").Append(separator).Append("null").Append(separator).Append(fileType).Append(separator).Append("null");
                        }

                        Log.Action.LogDebug("Request string: " + msgBuilder.ToString());

                        tcpClient.Send(Encoding.UTF8.GetBytes(msgBuilder.ToString()));//把字符串转化成字节数组，然后发送到服务器端 
                    }
                }
                catch (Exception ex)
                {
                    m_objContext.NextCondition = "OnCancel";
                    Log.Action.LogError("Send Request failed, error : {0}", ex);
                }
            }
            m_objContext.NextCondition = "OnConfirm";
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        string GetOtherInfo()
        {
            string info = "null";
            string sep = "|";
            StringBuilder builder = new StringBuilder();
            if (null != FormInfo)
            {
                builder.Append(FormInfo.IdCard_Name).Append(sep);
                builder.Append(FormInfo.IdCard_Sex).Append(sep);
                builder.Append(FormInfo.IdCard_Nation).Append(sep);
                builder.Append(FormInfo.IdCard_Birthday).Append(sep);
                builder.Append(FormInfo.IdCard_Address).Append(sep);
                builder.Append(FormInfo.IdCard_IDNo).Append(sep);
                builder.Append(FormInfo.IdCard_IDOrg).Append(sep);
                builder.Append(FormInfo.IdCard_ValidateDate).Append(sep);
                builder.Append(FormInfo.CustomAddress).Append(sep);
                builder.Append(FormInfo.PhoneNo).Append(sep);
                builder.Append(FormInfo.FamilyNo).Append(sep);
                builder.Append(FormInfo.EmailAddress);

                info = builder.ToString();
            }
            return info;
        }

        #endregion

    }
}
