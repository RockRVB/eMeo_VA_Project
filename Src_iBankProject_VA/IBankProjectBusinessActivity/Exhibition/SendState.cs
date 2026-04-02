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
    [GrgActivity("{4ED0A376-503E-49F5-80BC-D21B784F32C1}",
                 NodeNameOfConfiguration = "SendState",
                 Name = "SendState",
                 Author = "ltfei1")]
    public class SendState : BusinessActivityVTMBase
    {


        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new SendState() as IBusinessActivity;
        }
        private string _stateType = string.Empty;
        [GrgBindTarget("stateType", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string StateType
        {
            get
            {
                return _stateType;
            }
            set
            {
                _stateType = value;
                OnPropertyChanged("stateType");
            }
        }
        #endregion

        #region constructor
        protected SendState()
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
                        string port = XMLPackageClass.GetXmlAttValue("HTTPConfig.HTTP.[StatePort]");
                        IPAddress ipaddress = IPAddress.Parse(ip);
                        EndPoint point = new IPEndPoint(ipaddress, int.Parse(port));

                        tcpClient.Connect(point);

                        byte[] data = new byte[1024];
                        //int length = tcpClient.Receive(data);
                        //string message = Encoding.UTF8.GetString(data, 0, length);

                        //向服务器端发送消息
                        string message2 = m_objContext.TerminalConfig.Terminal.ATMNumber + "\u001F" + StateType;
                        tcpClient.Send(Encoding.UTF8.GetBytes(message2));//把字符串转化成字节数组，然后发送到服务器端 
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

        #endregion

    }
}
