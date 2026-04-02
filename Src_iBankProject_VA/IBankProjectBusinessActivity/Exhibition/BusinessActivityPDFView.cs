using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Xml;
using System.IO;
using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using UIServiceProtocol;
using LogProcessorService;
using InputOrSelect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HelperService;

namespace VTMBusinessActivity
{
    [GrgActivity("{5C166038-C798-48C6-AABF-A33F3789236E}",
                Name = "PDFView",
                Description = "PDFView",
                NodeNameOfConfiguration = "PDFView",
                Catalog = "PDFView",
                ForwardTargets = new string[] { EventDictionary.s_EventCancel, "OnSaveSuccess", "OnSaveError" })]
    public class BusinessActivityPDFView : BusinessActivitySelectFunction
    {
        #region fields

        /// <summary>
        /// 保存成功的action出口
        /// </summary>
        protected String saveSuccess = "OnSaveSuccess";
        /// <summary>
        /// 保存失败的action出口
        /// </summary>
        protected String saveError = "OnSaveError";
        /// <summary>
        /// 保存的事件名（ui传上来的事件key）
        /// </summary>
        protected String saveEventKey = "OnSave";
        /// <summary>
        /// 签名确认的按钮事件（ui传上来的事件key）
        /// </summary>
        protected String signConfirmEventKey = "OnSignConfirm";
        /// <summary>
        /// 保存结果所在的html元素对应的ID值
        /// </summary>
        protected String FieldID_saveStatus = "saveStatus";
        /// <summary>
        /// pdf表单的值所在的html元素的ID值
        /// </summary>
        protected String FieldID_formFieldValue = "formFieldValue";
        /// <summary>
        /// pdf表单的值所在的html元素的ID值
        /// </summary>
        protected String FieldID_formFieldDataCacheKey = "FormFieldDataCacheKey";
        /// <summary>
        /// 签名图片base64字符串保存的域
        /// </summary>
        protected String formFieldsignImageBase64String = "signImageBase64String";

        #region "数据池项的key"

        /// <summary>
        /// 表单值保存到数据池的key值
        /// </summary>
        protected String s_formFieldDataCacheKey = "pdf_formFieldDataCacheKey";
        protected String s_pdf_CertificateFilePath = "pdf_CertificateFilePath";
        protected String s_pdf_EmptySignImageFile = "pdf_EmptySignImageFile";
        protected String s_pdf_CertificatePassword = "pdf_CertificatePassword";

        #endregion

        /// <summary>
        /// 总线对象
        /// </summary>
        private eCATContext m_eCATContext = null;

        #endregion

        #region method of creating

        [GrgCreateFunction("Create")]
        public new static IBusinessActivity Create()
        {
            return new BusinessActivityPDFView() as IBusinessActivity;
        }

        #endregion

        #region constructor

        public BusinessActivityPDFView()
        {

        }

        #endregion

        #region override methods of base

        protected override emBusActivityResult_t InnerPreRun(BusinessContext objContext)
        {
            SetDataCache(objContext);
            CountPdfSaveFilePath(objContext);
            CountPdfSaveFileName(objContext);
            CountPdfSaveFileFullPath(objContext);
            return base.InnerPreRun(objContext);
        }

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebug("Enter action: BusinessActivityPDFView");
            m_eCATContext = argContext as eCATContext;
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                m_objContext.LogJournal("Execute base inner run fail!", LogSymbol.Alert);
                Log.Action.LogDebug("Leave action: BusinessActivityPDFView");
                return emRet;
            }

            Log.Action.LogDebug("Leave action: BusinessActivityPDFView");
            return emBusActivityResult_t.Success;
        }

        /// <summary>
        /// UI事件
        /// 由于Action的UI事件必须是真实Click操作触发的事件才能捕获，没办法通过代码触发ecat的UI事件，所以action根据用户触发的按钮事件进行截获过滤处理。
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="argUIEvent"></param>
        /// <returns></returns>
        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            //保存事件
            String eventKey = Convert.ToString(argUIEvent.Key);
            //if (eventKey == saveEventKey)
            if (argUIEvent.ElementName == "btnConfirm")
            {
                //保存pdf
                //捕获到页面触发了保存事件，新起一个线程轮询pdf保存结果
                //（因为ecat事件抛上来，流程处理了下一步，并不能确定pdf已经保存成功，所以需要等待UI，确定pdf保存成功后再走下一步流程）
                StartTimer(iUI, argUIEvent);
                return emBusiCallbackResult_t.Swallowd;
            }
            //if (signConfirmEventKey == eventKey)
            if (argUIEvent.ElementName == "_pdfsign_dialog_confirmBtn")
            {
                //确定签名
                CreateSignImage(iUI);
                return emBusiCallbackResult_t.Swallowd;
            }
            else
            {
                return base.InnerOnUIEvtHandle(iUI, argUIEvent);
            }
        }

        #endregion

        #region append methods

        /// <summary>
        /// 设置数据池
        /// </summary>
        protected virtual void SetDataCache(BusinessContext objContext)
        {
            eCATContext eCATContext = objContext as eCATContext;
            Config conf = GetConfig();
            //eCATContext.TransactionDataCache.Set("pdf_CertificateFilePath", conf.CertificateFilePath, GetType());
            //eCATContext.TransactionDataCache.Set("pdf_EmptySignImageFile", conf.EmptySignImageFile, GetType());
            //eCATContext.TransactionDataCache.Set("pdf_CertificatePassword", conf.CertificatePassword, GetType());
            eCATContext.TransactionDataCache.Set(s_pdf_CertificateFilePath, conf.CertificateFilePath, GetType());
            eCATContext.TransactionDataCache.Set(s_pdf_EmptySignImageFile, conf.EmptySignImageFile, GetType());
            eCATContext.TransactionDataCache.Set(s_pdf_CertificatePassword, conf.CertificatePassword, GetType());
        }

        /// <summary>
        /// 计算pdf文件保存的目录
        /// </summary>
        protected virtual void CountPdfSaveFilePath(BusinessContext objContext)
        {
            eCATContext eCATContext = objContext as eCATContext;
            //设置pdf文件保存目录
            String pdfSaveFilePath = Convert.ToString(eCATContext.GetBindData("pdf_SaveFilePath"));
            if (String.IsNullOrEmpty(pdfSaveFilePath))
            {
                Config conf = GetConfig();
                Directory.CreateDirectory(conf.PdfFileSavePath);
                eCATContext.TransactionDataCache.Set("pdf_SaveFilePath", conf.PdfFileSavePath, GetType());
            }
            else
            {
                Directory.CreateDirectory(pdfSaveFilePath);
            }
        }

        /// <summary>
        /// 计算pdf文件保存的文件名
        /// </summary>
        protected virtual void CountPdfSaveFileName(BusinessContext objContext)
        {
            eCATContext eCATContext = objContext as eCATContext;
            Config conf = GetConfig();
            String fileName = String.Empty;
            if (String.IsNullOrEmpty(conf.PdfFileSaveName))
            {
                //设置pdf文件保存文件名
                String pdf_OpenFileUrlOrPath = Convert.ToString(eCATContext.GetBindData("pdf_OpenFileUrlOrPath"));
                if (!String.IsNullOrEmpty(pdf_OpenFileUrlOrPath))
                {
                    pdf_OpenFileUrlOrPath = pdf_OpenFileUrlOrPath.Replace("\\", "/");
                    fileName = pdf_OpenFileUrlOrPath.Substring(pdf_OpenFileUrlOrPath.LastIndexOf("/") + 1);
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                fileName = conf.PdfFileSaveName.Replace("{yyyy}", now.ToString("yyyy"))
                                                .Replace("{MM}", now.ToString("MM"))
                                                .Replace("{dd}", now.ToString("dd"))
                                                .Replace("{HH}", now.ToString("HH"))
                                                .Replace("{mm}", now.ToString("mm"))
                                                .Replace("{ss}", now.ToString("ss"));
            }
            fileName = Path.Combine(conf.PdfFileSavePath, fileName);
            eCATContext.TransactionDataCache.Set("pdf_SaveFileName", fileName, GetType());
        }

        /// <summary>
        /// 计算pdf文件保存的完整文件名
        /// </summary>
        /// <param name="objContext"></param>
        protected virtual void CountPdfSaveFileFullPath(BusinessContext objContext)
        {
            eCATContext eCATContext = objContext as eCATContext;
            String fileName = Convert.ToString(eCATContext.GetBindData("pdf_SaveFileName"));
            String saveFilePath = Convert.ToString(eCATContext.GetBindData("pdf_SaveFilePath"));
            eCATContext.TransactionDataCache.Set("pdf_SaveFileFullPath", Path.Combine(saveFilePath, fileName), GetType());
        }

        /// <summary>
        /// 创建签名图片
        /// </summary>
        /// <param name="iUI"></param>
        /// <returns></returns>
        protected virtual void CreateSignImage(IUIService iUI)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((inputUI) =>
            {
                IUIService ui = inputUI as IUIService;
                DoCreateSignImage(ui);
            }), iUI);
        }

        /// <summary>
        /// 创建签名图片
        /// </summary>
        /// <param name="iUI"></param>
        protected virtual void DoCreateSignImage(IUIService iUI)
        {
            Object val = GetUIFieldValue(iUI, formFieldsignImageBase64String, 5);
            //if (iUI.GetPropertyValueOfElement(null, formFieldsignImageBase64String, "value", out val))
            {
                String valStr = Convert.ToString(val);
                Config config = GetConfig();
                String signImageFilePath = config.SignImageSavePath;
                signImageFilePath = Base64ToImage(valStr, signImageFilePath);
                String certificateFilePath = config.CertificateFilePath;
                String certificatePassword = config.CertificatePassword;
                //sCommand, sFileUrl, sPfxFilePath, sPassword
                iUI.ExecuteCommand(UICommandType.Script, "ExcuteBackgroundCommand",
                    new Object[] { "InsertSignImage", signImageFilePath, certificateFilePath, certificatePassword });
            }
        }

        /// <summary>
        /// 把base64字符串转换为图片
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="signFilePath">图片文件保存的目录</param>
        public static String Base64ToImage(String base64String, String signFilePath)
        {
            try
            {
                //首先创建目录
                Directory.CreateDirectory(signFilePath);
                String imageFileName = DateTime.Now.ToString("yyyy-mm-dd HH-mm-ss") + ".png";
                String imageFileFullPath = Path.Combine(signFilePath, imageFileName);
                Byte[] data = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(data);
                Bitmap bitmap = new Bitmap(ms);
                bitmap.Save(imageFileFullPath);
                return imageFileFullPath;
            }
            catch (Exception ex)
            {
                Log.Action.LogError("Base64ToImage()：" + ex.Message);
            }
            return String.Empty;
        }

        /// <summary>
        /// 获取表单值保存到数据池的key值
        /// </summary>
        /// <param name="iUI"></param>
        /// <returns></returns>
        protected virtual String GetPDFFormFieldValueCacheKey(IUIService iUI)
        {
            Object val = null;
            if (iUI.GetPropertyValueOfElement(null, FieldID_formFieldDataCacheKey, "value", out val))
            {
                String valStr = Convert.ToString(val);
                return valStr;
            }
            return s_formFieldDataCacheKey;
        }

        /// <summary>
        /// 把pdf表单填写的值保存到数据池中
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="fieldValue"></param>
        protected virtual void SetPDFFormFieldValueToCache(IUIService iUI, String fieldValue)
        {
            if (m_eCATContext == null) return;
            String cacheKey = GetPDFFormFieldValueCacheKey(iUI);
            if (String.IsNullOrEmpty(fieldValue))
            {
                m_eCATContext.TransactionDataCache.Set(cacheKey, fieldValue, GetType());
            }
            else
            {
                Object jo = JsonConvert.DeserializeObject(fieldValue);
                Dictionary<String, Object> fieldValues = ParsetoDictionary(jo) as Dictionary<String, Object>;
                m_eCATContext.TransactionDataCache.Set(cacheKey, fieldValues, GetType());
            }
        }

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer = null;
        private Int32 tryTimes = 3;

        /// <summary>
        /// 执行检查保存状态
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="argUIEvent"></param>
        protected virtual void DoCheckSaveResult(IUIService iUI, UIEventArg argUIEvent)
        {
            //重新起一个新的线程来执行（只有释放UI线程才能获取到最新的页面元素的值）
            Object val = null;
            iUI.ExecuteCommand(UICommandType.Custom, UIServiceCommands.s_updateData);
            if (iUI.GetPropertyValueOfElement(null, FieldID_saveStatus, "value", out val) && tryTimes >= 1)
            {
                //更新数据
                iUI.ExecuteCommand(UICommandType.Custom, UIServiceCommands.s_updateData);
                String saveResult = Convert.ToString(val);
                if (!String.IsNullOrEmpty(saveResult))
                {
                    UIEventArg uiEventArg = new UIEventArg()
                    {
                        ElementName = argUIEvent.ElementName,
                        EventName = argUIEvent.EventName,
                        IsImpersonated = argUIEvent.IsImpersonated,
                        Param = argUIEvent.Param,
                        ScreenName = argUIEvent.ScreenName,
                        Source = argUIEvent.Source
                    };
                    //判断结果值
                    if (saveResult == "0") //0表示保存成功
                    {
                        //保存成功之后，还需要获取所有表单域的值
                        Object pdfFormFieldValue = null;
                        iUI.GetPropertyValueOfElement(null, FieldID_formFieldValue, "value", out pdfFormFieldValue);
                        //Log.Action.LogDebug("pdf formfieldValue: " + Convert.ToString(pdfFormFieldValue));
                        SetPDFFormFieldValueToCache(iUI, Convert.ToString(pdfFormFieldValue));
                        //让action走保存成功的出口
                        uiEventArg.Key = saveSuccess;
                    }
                    else
                    {
                        //让action走保存失败的出口
                        uiEventArg.Key = saveError;
                    }
                    base.InnerOnUIEvtHandle(iUI, uiEventArg);
                    return;
                }
                //重新改变定时器，进行下一次的执行
                ReStartTimer(iUI, argUIEvent);
                tryTimes--;
            }
        }

        /// <summary>
        /// 检查保存状态
        /// </summary>
        /// <param name="inputVal"></param>
        protected virtual void CheckSaveResult(Object inputVal)
        {
            //线程内部方法一定要捕获异常，否则出现线程未处理异常会导致程序崩溃
            try
            {
                Object[] inputParams = inputVal as Object[];
                IUIService iUI = inputParams[0] as IUIService;
                UIEventArg argUIEvent = inputParams[1] as UIEventArg;
                DoCheckSaveResult(iUI, argUIEvent);
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 启动定时器
        /// 必须要用异步去轮询获取UI元素的值，否则UI线程被卡死，没办法获取最新的值
        /// </summary>
        protected virtual void StartTimer(IUIService iUI, UIEventArg argUIEvent)
        {
            if (timer == null)
            {
                timer = new Timer(new TimerCallback(CheckSaveResult), new Object[] { iUI, argUIEvent }, 100, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 重新启动timer
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="argUIEvent"></param>
        protected virtual void ReStartTimer(IUIService iUI, UIEventArg argUIEvent)
        {
            if (timer != null)
                timer.Change(tryTimes * 100, Timeout.Infinite);
        }

        #endregion

        #region "get ui fieldValue Methods"

        /// <summary>
        /// 获取UI元素值的委托
        /// </summary>
        private Func<Object[], Object> getUIFieldValueFunc = null;

        private AutoResetEvent getValueResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 获取UI的元素值
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="fieldName"></param>
        /// <param name="tryTimes">获取不到内容的时候，重复获取的次数（会进行异步的遍历次数）</param>
        /// <returns></returns>
        protected virtual String GetUIFieldValue(IUIService iUI, String fieldName, Int32 tryTimes = 3)
        {
            getValueResetEvent.Reset();
            getUIFieldValueFunc = new Func<object[], object>(GetUIFieldValueFuncCallback);
            Object fieldValue = null;
            IAsyncResult getFieldValueResult = getUIFieldValueFunc.BeginInvoke(new Object[] { iUI, fieldName, tryTimes }, new AsyncCallback((result) =>
            {
                fieldValue = getUIFieldValueFunc.EndInvoke(result);
                getValueResetEvent.Set();
            }), null);
            getValueResetEvent.WaitOne();
            return Convert.ToString(fieldValue);
        }

        /// <summary>
        /// 执行获取值的方法
        /// </summary>
        /// <param name="inputVal"></param>
        /// <returns></returns>
        protected virtual Object GetUIFieldValueFuncCallback(Object[] inputVal)
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);
            Object[] inputParams = inputVal as Object[];
            Int32 tryTimes = Convert.ToInt32(inputVal[2]);//尝试次数
            Int32 currentTimes = 0;
            Object fieldValue = null;

            using (Timer timer = new Timer(new TimerCallback((obj) =>
            {
                IUIService iUI = inputParams[0] as IUIService;
                String fieldName = Convert.ToString(inputParams[1]);
                fieldValue = DoGetUIFieldValue(iUI, fieldName);
                //Log.Action.LogDebug("fieldValue：" + Convert.ToString(fieldValue));
                resetEvent.Set();
            }), null, 0, Timeout.Infinite))
            {
                while (currentTimes++ <= tryTimes && String.IsNullOrEmpty(Convert.ToString(fieldValue)))
                {
                    //Thread.Sleep(1000 * currentTimes);
                    resetEvent.WaitOne();
                    resetEvent.Reset();
                    //内部需要判断字符串是否获取到不为空了，不为空才需要重新执行获取代码。
                    if (String.IsNullOrEmpty(Convert.ToString(fieldValue)))
                    {
                        Thread.Sleep(1000 * currentTimes);
                        timer.Change(0, Timeout.Infinite);
                    }
                }
                return fieldValue;
            } 
        }

        /// <summary>
        /// 执行获取UI的元素值的方法
        /// </summary>
        /// <param name="iUI"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        protected virtual Object DoGetUIFieldValue(IUIService iUI, String fieldName)
        {
            //重新起一个新的线程来执行（只有释放UI线程才能获取到最新的页面元素的值）
            Object val = null;
            iUI.ExecuteCommand(UICommandType.Custom, UIServiceCommands.s_updateData);
            iUI.GetPropertyValueOfElement(null, fieldName, "value", out val);
            //Log.Action.LogDebug("图片base64编码数据为：" + val);
            //Log.Action.LogDebug("The image base64 encode data is：" + val);
            return val;
        }

        #endregion

        #region static methods(utility)

        /// <summary>
        /// 把转换出来的JObject复制到Dictionary中
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static Object ParsetoDictionary(Object o)
        {
            if (o == null) return null;
            if (o.GetType() == typeof(String))
            {
                //判断是否符合2010-09-02T10:00:00的格式
                String s = o.ToString();
                if (s.Length == 19 && s[10] == 'T' && s[4] == '-' && s[13] == ':')
                    o = System.Convert.ToDateTime(o);
            }
            else if (o is JObject)
            {
                JObject jo = o as JObject;
                Dictionary<String, Object> result = new Dictionary<String, Object>();
                foreach (KeyValuePair<String, JToken> entry in jo)
                    result[entry.Key] = ParsetoDictionary(entry.Value);
                o = result;
            }
            else if (o is IList)
            {
                ArrayList list = new ArrayList();
                list.AddRange((o as IList));
                int i = 0, l = list.Count;
                for (; i < l; i++)
                    list[i] = ParsetoDictionary(list[i]);
                o = list;
            }
            else if (typeof(JValue) == o.GetType())
            {
                JValue v = (JValue)o;
                o = ParsetoDictionary(v.Value);
            }
            else
            {
            }
            return o;
        }

        #endregion

        #region config

        public class Config
        {
            /// <summary>
            /// pdf文件保存的文件名
            /// </summary>
            public string PdfFileSaveName
            {
                get;
                set;
            }

            /// <summary>
            /// pdf文件保存的目录
            /// </summary>
            public string PdfFileSavePath
            {
                get;
                set;
            }

            /// <summary>
            /// 签名图片保存目录
            /// </summary>
            public string SignImageSavePath
            {
                get;
                set;
            }

            /// <summary>
            /// 签名图片保存策略，默认退出pdfviewAction就会删除所有签名图片
            /// </summary>
            public Boolean SignImageDeleteWhenExitAction
            {
                get;
                set;
            }

            /// <summary>
            /// 数字证书目录
            /// </summary>
            public String CertificateFilePath
            {
                get;
                set;
            }

            /// <summary>
            /// 数字证书口令
            /// </summary>
            public String CertificatePassword
            {
                get;
                set;
            }

            /// <summary>
            /// 空白签名图片路径
            /// </summary>
            public String EmptySignImageFile
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 配置对象
        /// </summary>
        private static Config currentConfig = null;
        public static Config GetConfig()
        {
            if (currentConfig == null || SpecialHandle.Offline)
            {
                try
                {
                    currentConfig = new Config();
                    XmlDocument doc = new XmlDocument();
                    String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\PdfFormEdit.xml");
                    doc.Load(filePath);
                    currentConfig.PdfFileSavePath = FormatPath(doc.SelectSingleNode("/Config/PdfFileSavePath").InnerText);
                    currentConfig.EmptySignImageFile = FormatPath(doc.SelectSingleNode("/Config/Sign/EmptySignImageFile").InnerText);
                    currentConfig.PdfFileSaveName = doc.SelectSingleNode("/Config/PdfFileSaveName").InnerText;
                    currentConfig.CertificateFilePath = FormatPath(doc.SelectSingleNode("/Config/Sign/CertificateFilePath").InnerText);
                    currentConfig.CertificatePassword = doc.SelectSingleNode("/Config/Sign/CertificatePassword").InnerText;
                    currentConfig.SignImageDeleteWhenExitAction = Convert.ToBoolean(doc.SelectSingleNode("/Config/Sign/SignImageDeleteWhenExitAction").InnerText);
                    currentConfig.SignImageSavePath = doc.SelectSingleNode("/Config/Sign/SignImageSavePath").InnerText;
                }
                catch (Exception ex)
                {
                    //Log.Action.LogError("读取pdfviewconfig.xml文件出错：" + ex.Message);
                    Log.Action.LogError("Reading pdfviewconfig.xml file Error：" + ex.Message);
                }
            }
            return currentConfig;
        }

        public static String FormatPath(String path)
        {
            if (String.IsNullOrEmpty(path)) return path;
            //绝对路径
            if (path.IndexOf(":") > 0)
            {
                return path;
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }
        }

        ///// <summary>
        ///// 获取签名图片保存的路径
        ///// </summary>
        ///// <returns></returns>
        //public static String GetSignImageFileSavePath()
        //{
        //    String generalConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\GeneralConfig.xml");
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(generalConfigFilePath);
        //    XmlNode node = doc.SelectSingleNode("/Config/SignImageFileSavePath");
        //    if (node != null)
        //    {
        //        XmlAttribute attr = node.Attributes["value"];
        //        return attr == null ? attr.Value : String.Empty;
        //    }
        //    return String.Empty;
        //}

        #endregion
    }
}
