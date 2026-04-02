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
using VTMModelLibrary;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{CE8743A7-3E48-4BE5-B1D2-738E25C6B8B1}",
                 NodeNameOfConfiguration = "UpdateFilebyFTP",
                 Name = "UpdateFilebyFTP",
                 Author = "ltfei1")]
    public class UpdateFilebyFTP: BusinessActivityVTMBase
    {
        string FTPAddress; //ftp服务器地址
        string FTPUsername;   //用户名
        string FTPPwd;        //密码

        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new UpdateFilebyFTP() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected UpdateFilebyFTP()
        {

        }
        #endregion

        [GrgBindTarget("fileDataCache", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FileDataCache
        {
            get;
            set;
        }


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
                FTPAddress = @"ftp://" + XMLPackageClass.GetXmlAttValue("FTPConfig.FTP.[ServerIP]") + ":" + XMLPackageClass.GetXmlAttValue("FTPConfig.FTP.[ServerPort]");
                FTPUsername = XMLPackageClass.GetXmlAttValue("FTPConfig.FTP.[UserName]");
                FTPPwd = XMLPackageClass.GetXmlAttValue("FTPConfig.FTP.[UserPassword]");
                string LocalPath = string.Empty;

                object objPath = null;

                if (string.IsNullOrEmpty(FileDataCache))
                {
                    Log.Action.LogError("Update File by failed, error : FileDataCache is null or empty");
                    m_objContext.NextCondition = "OnCancel";
                    Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                    return emBusActivityResult_t.Success;
                }

                objPath = m_objContext.GetBindData(FileDataCache);
                if (objPath != null)
                {
                    LocalPath = objPath as string;
                }
                else
                {
                    Log.Action.LogError("Update File by failed, error : objPath is null or empty");
                    m_objContext.NextCondition = "OnCancel";
                    Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                    return emBusActivityResult_t.Success;
                }

                FileInfo file = new FileInfo(LocalPath);
                string FileName = file.Name;
                //Path = Path.Replace("\\", "/");
                string ftpRemotePath = XMLPackageClass.GetXmlAttValue("FTPConfig.FTP.[FTPPath]");
                string FTPPath = FTPAddress + ftpRemotePath + FileName; //上传到ftp路径,                                   

                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(FTPPath));
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(FTPUsername, FTPPwd); //设置通信凭据
                reqFtp.KeepAlive = false; //请求完成后关闭ftp连接
                reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
                reqFtp.ContentLength = file.Length;
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                try
                {
                    //读本地文件数据并上传
                    using (FileStream fs = file.OpenRead())
                    {
                        //Log.Action.LogDebug("Update File: " + FTPPath + FTPUsername + FTPPwd);
                        Stream strm = reqFtp.GetRequestStream();
                        contentLen = fs.Read(buff, 0, buffLength);
                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                        strm.Close();
                        fs.Close();
                    }   
                }
                catch (Exception ex)
                {
                    Log.Action.LogError("Update  File by failed, error : {0}", ex);
                    m_objContext.NextCondition = "OnCancel";
                    Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                    return emBusActivityResult_t.Success;
                }
            }
            m_objContext.NextCondition = "OnConfirm";
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion

    }
}
