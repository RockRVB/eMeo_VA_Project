using System;
using System.Collections.Generic;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using eCATBusinessServiceProtocol;
using RemoteTellerServiceProtocol;
using System.Threading;
using CameraDeviceProtocol;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using VTMModelLibrary;
using ResourceManagerProtocol;
using System.Runtime.Serialization.Json;

namespace VTMBusinessActivity
{
    /// <summary>
    /// get n latest details without datetime value
    /// </summary>
    [GrgActivity("{1FAA4779-0C59-41E1-97A1-1B438A21C5DD}",
                 Name = "FaceRecognition",
                 NodeNameOfConfiguration = "FaceRecognition")]
    public class FaceRecognition : BusinessActivityVTMBase
    {
        #region constructor
        protected FaceRecognition()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new FaceRecognition();
        }
        #endregion

        #region 属性
        //private bool NeedRecognizeFace = false;
        protected CameraWrapper m_CAMWrapper = null;
        private bool State = false;
        private byte[] LocalFaceBytes;
        string fileName = "";
        private string basePath = AppDomain.CurrentDomain.BaseDirectory;
        #endregion


        /// <summary>
        /// enter action
        /// </summary>
        /// <param name="argContext"></param>
        /// <returns></returns>
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0},InnerRun", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            //===============================
            //切UI界面
            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            //人脸识别的代码
            #region 开始人脸识别
            int count = 0;
            int ret;
            try
            {
                while (count < 2)
                {
                    Thread.Sleep(3000);
                    count++;
                    if (null == VTMContext.VideoService)
                    {
                        SwitchUIState(m_objContext.MainUI, "ScanCameraError", 3000);
                        WaitSignal();
                        VTMContext.LogJournalKey("IDS_ScanCameraFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        m_objContext.NextCondition = EventDictionary.s_EventFail;
                        VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                        Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                        return emBusActivityResult_t.Success;
                    }
                    fileName = basePath + "Temp\\" + Guid.NewGuid().ToString() + ".jpg";
                    ret = VTMContext.VideoService.CaptureImage(fileName);

                    m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_CamerImagePath, fileName, GetType());//保存数据
                    //转换二进制
                    Image localImage = Image.FromFile(fileName);
                    LocalFaceBytes = ImageToBytes(localImage);
                    localImage.Dispose();
                    //类型转换
                    string StrImage = Convert.ToBase64String(LocalFaceBytes);
                    //调用服务器人脸识别接口，验证成功，返回用户信息
                    string equipId = "72172129";//获取设备号
                    bool httpResult = HttpGetUserVIP(StrImage, equipId);
                    if (httpResult == true)
                    {
                        State = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("RecognizeFace is end by:" + ex.ToString());
            }
            #endregion

            if (State)
            {
                VTMContext.NextCondition = EventDictionary.s_EventConfirm;
            }
            else
            {
                VTMContext.NextCondition = EventDictionary.s_EventCancel;
            }

            //========================================================
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        
        #region 调用人脸识别接口
        /// <summary>
        /// 调用服务器人脸识别接口
        /// </summary>
        /// <param name="Image">图片</param>
        ///  <param name="equipId"></param>
        /// <returns></returns>
        public bool HttpGetUserVIP(string Image, string equipId)
        {
            bool state = false;
            try
            {
                Log.Action.LogDebug("Enter HttpGetUserVIP");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FaceUrl);
                request.Method = "Post";
                request.ContentType = "application/json";
                string JsonData = "{\"equipId\":" + "\"" + equipId + "\"" + "," + "\"image\":" + "\"" + Image + "\"" + "}";
                byte[] data = Encoding.Default.GetBytes(JsonData);
                request.ContentLength = data.Length;
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                Log.Action.LogDebug("Send http request success");
                reqStream.Close();

                using (WebResponse webResponse = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string retData = sr.ReadToEnd();
                        //把json反序列化为对象
                        using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(retData)))
                        {
                            DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(ResultRespose));
                            ResultRespose model = (ResultRespose)deseralizer.ReadObject(ms);
                            if (model.data.num > 0)
                            {
                                state = true;
                                string IdCard = null;
                                string uid = model.data.results[0].uid;
                                Log.Action.LogDebug("uid: " + uid);
                                HttpGetAppInfo(model.data.results[0].uid, IdCard);//调用手机端接口获取用户预填信息
                            }
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                Log.Action.LogError("HttpGetInfo error" + ex.ToString());
            }

            return state;
        }

        #endregion
        //定义人脸识别接口返回的实体
        public class ResultRespose
        {
            public int code { get; set; }
            public string msg { get; set; }
            public data data { get; set; }
        }
        public class data
        {
            public int num { get; set; }
            public string age { get; set; }
            public string gebder { get; set; }
            public string identifyImage { get; set; }
            public IList<result> results { get; set; }
        }
        public class result
        {
            public string uid { get; set; }
            public string name { get; set; }
            public string score { get; set; }
            public string registerImage { get; set; }
            public string mode { get; set; }
        }

        #region
        /// <summary>
        /// 调用手机端接口，返回用户填写信息
        /// </summary>
        /// <param name="IdCard">用户身份证信息</param>
        /// <returns></returns>
        public string HttpGetAppInfo(string VipNo, string IdCard)
        {
            string Url = MoblieUrl;
            //VipNo = "032";
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
                        VTMContext.TransactionDataCache.Set("model", model, GetType());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("Failed to get info from mobile: " + ex.ToString());
            }
            return retData;
        }
        #endregion


        /// <summary>
        /// 将图片转换成二进制流
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }


        private string m_FaceUrl = "http://10.252.105.1:9885/face/identify";
        [GrgBindTarget("FaceUrl", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string FaceUrl
        {
            get
            {
                return m_FaceUrl;
            }
            set
            {
                m_FaceUrl = value;
                OnPropertyChanged("FaceUrl");
            }
        }

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
    }
}
