using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using System.Diagnostics;
using PrintReceipt;
using System;
using System.Text;
using System.Collections.Generic;
using ReceiptPrinterDeviceProtocol;
using System.IO;
using System.Drawing;

namespace VTMBusinessActivity
{
    [GrgActivity("{2CCCB927-BDB7-4862-8695-794D608868BC}",
                Name = "CRDPrintReceiptImg",
                NodeNameOfConfiguration = "CRDPrintReceiptImg",
                Description = "CRDPrintReceiptImg",
                Catalog = "PrintReceiptEx",
                ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]

    public class BusinessActivityCRDPrintReceiptImg : BusinessActivityPrintReceiptEx
    {
        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityCRDPrintReceiptImg();
        }
        #endregion

        #region constructor
        public BusinessActivityCRDPrintReceiptImg()
        {
        }
        #endregion

        #region define variable
        // 打印默认显示3秒钟
        protected string m_showTime = "3";
        #endregion

        #region property
        [GrgBindTarget("isControlMedia", Type = TargetType.Int, Access = AccessRight.ReadAndWrite)]
        public int isControlMedia
        {
            get;
            set;
        }

        private string m_FrontImgPath = @"C:\xfs\Form\CardPrinter\Front.jpg";
        [GrgBindTarget("FrontImgPath", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FrontImgPath
        {
            get
            {
                return m_FrontImgPath;
            }
            set
            {
                m_FrontImgPath = value;
                OnPropertyChanged("FrontImgPath");
            }
        }

        private string m_BackImgPath = @"C:\xfs\Form\CardPrinter\Back.jpg";
        [GrgBindTarget("BackImgPath", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string BackImgPath
        {
            get
            {
                return m_BackImgPath;
            }
            set
            {
                m_BackImgPath = value;
                OnPropertyChanged("BackImgPath");
            }
        }

        private string m_FontName = "宋体";
        [GrgBindTarget("FontName", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FontName
        {
            get
            {
                return m_FontName;
            }
            set
            {
                m_FontName = value;
                OnPropertyChanged("FontName");
            }
        }

        private string m_FontSize = "44";
        [GrgBindTarget("FontSize", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FontSize
        {
            get
            {
                return m_FontSize;
            }
            set
            {
                m_FontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        private string m_CardNumberLocation = "80,342";
        [GrgBindTarget("CardNumberLocation", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CardNumberLocation
        {
            get
            {
                return m_CardNumberLocation;
            }
            set
            {
                m_CardNumberLocation = value;
                OnPropertyChanged("CardNumberLocation");
            }
        }

        private string m_ValidLocation = "260,442";
        [GrgBindTarget("ValidLocation", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ValidLocation
        {
            get
            {
                return m_ValidLocation;
            }
            set
            {
                m_ValidLocation = value;
                OnPropertyChanged("ValidLocation");
            }
        }

        private string m_ValidFontSize = "24";
        [GrgBindTarget("ValidFontSize", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string ValidFontSize
        {
            get
            {
                return m_ValidFontSize;
            }
            set
            {
                m_ValidFontSize = value;
                OnPropertyChanged("ValidFontSize");
            }
        }

        private string m_dataPoolForCardNumber = "CardNumberToPrint";
        [GrgBindTarget("DataPoolForCardNumber", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string DataPoolForCardNumber
        {
            get
            {
                return m_dataPoolForCardNumber;
            }
            set
            {
                m_dataPoolForCardNumber = value;
                OnPropertyChanged("DataPoolForCardNumber");
            }
        }
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebug("Enter action: CRDPrintReceiptImg");

            eCATContext context = (eCATContext)objContext;
            Debug.Assert(null != context);

            m_objContext.NextCondition = EventDictionary.s_EventConfirm;

            bool resultOfUI = true;

            try
            {
                //显示正在打印...
                if (multiLang == 1)
                {
                    object objValue = null;
                    m_objContext.CardHolderDataCache.Get("selectlangalready", out objValue, GetType());
                    if (objValue != null && objValue.ToString() == "1")
                    {
                        SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);
                    }
                    else
                    {
                        SwitchUIState(m_objContext.MainUI, "DoubleLanguagePrintingReceipt", -int.MaxValue);
                    }
                }
                else
                {
                    SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);
                }

                string strFormName = formName;
                if (null != isFormPrint && (isFormPrint.ToUpper() == "FALSE" || isFormPrint.ToUpper() == "0"))
                {
                    strFormName = string.Empty;
                }

                string dir = FrontImgPath;

                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                string savePath = Path.Combine(folderPath, "FrontCard.jpg");
                //有效期
                string ExpriyDate = "12 / 25";
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string month = dt.Month.ToString("D2");
                string year = dt.AddYears(5).Year.ToString();
                year = year.Substring(year.Length - 2);
                ExpriyDate = month + " / " + year;

                m_objContext.TransactionDataCache.Set("CardExpriyDate",ExpriyDate,GetType());
                //卡号
                string CardNumber = "6222 8888 8888 8888";
                object objCardNumber;
                m_objContext.TransactionDataCache.Get(DataPoolForCardNumber, out objCardNumber, GetType());
                if (!string.IsNullOrEmpty(objCardNumber?.ToString()))
                {
                    CardNumber = objCardNumber.ToString();
                }

                if (File.Exists(dir))
                {
                    FileStream fs = new FileStream(dir, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();
                    MemoryStream ms = new MemoryStream(bytes);
                    Bitmap img = new Bitmap(ms);

                    Graphics g = Graphics.FromImage(img);
                    Font font = new Font(FontName, float.Parse(FontSize));
                    SolidBrush sbrush = new SolidBrush(System.Drawing.Color.White);

                    float cardNumberX = 80;
                    float cardNumberY = 342;
                    float validX = 260;
                    float validY = 442;

                    var cardNumberLocation = CardNumberLocation.Split(',');
                    if (cardNumberLocation.Length >= 2)
                    {
                        cardNumberX = float.Parse(cardNumberLocation[0]);
                        cardNumberY = float.Parse(cardNumberLocation[1]);
                    }

                    var validLocation = ValidLocation.Split(',');
                    if (validLocation.Length >= 2)
                    {
                        validX = float.Parse(validLocation[0]);
                        validY = float.Parse(validLocation[1]);
                    }                    

                    g.DrawString(CardNumber, font, sbrush, new PointF(cardNumberX, cardNumberY));
                    g.DrawString("VALID " + ExpriyDate, new Font(FontName, float.Parse(ValidFontSize)), sbrush, new PointF(validX, validY));
                    img.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);
                    g.Dispose();
                }                

                //设置form的field字段
                m_objContext.TransactionDataCache.Set("FrontImgPath", savePath, GetType());
                m_objContext.TransactionDataCache.Set("BackImgPath", BackImgPath, GetType());
                m_objContext.TransactionDataCache.Set("CardNo", "", GetType());
                m_objContext.TransactionDataCache.Set("ExpriyDate", "", GetType());
                m_objContext.TransactionDataCache.Set("Name", "", GetType());
                m_objContext.TransactionDataCache.Set("CCV2", "", GetType());

                string strPrnData;
                bool bRet = BuildReceiptExData("", ref strFormName, out strPrnData, dataSource);
                if (bRet)
                {
                    DevResult devResult = null;
                    if (string.IsNullOrEmpty(strFormName))
                    {                    
                        devResult = ReceiptPrinter?.PrintRawData(strPrnData);
                    }
                    else
                    {
                        devResult = ReceiptPrinter?.PrintForm(strFormName, strPrnData, false);
                    }

                    Log.Action.LogDebugFormat("devResult.IsSuccess :{0}", devResult?.IsSuccess);
                    if (devResult?.IsSuccess==true)
                    {
                        resultOfUI = false;
                    }
                    else
                    {
                        m_objContext.NextCondition = EventDictionary.s_EventCancel;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }

            if (resultOfUI)
            {
                SwitchUIState(m_objContext.MainUI, "PrintReceiptFail", 3000);
                WaitSignal();
            }

            Log.Action.LogDebug("Leave action: CRDPrintReceiptImg");
            return emBusActivityResult_t.Success;
        }

        protected override bool BuildReceiptExData(string argTransType, ref string argFormName, out string argPrintData, string argDataSource = null)
        {
            argPrintData = "";

            string strAllFields = ReceiptPrinter?.GetFieldsInfo(argFormName);
            string[] strArrAllFields = strAllFields.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArrAllFields.Length <= 0)
            {
                Log.Action.LogWarnFormat("Receipt form [{0}] file error, can not find field value", formName);
                return false;
            }

            StringBuilder builder = new StringBuilder();
            string[] strArrFields = strArrAllFields[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string strField in strArrFields)
            {
                if (!string.IsNullOrWhiteSpace(strField))
                {
                    string fieldname = strField.Split('=')[0];
                    string value = m_objContext.GetBindData(fieldname) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.Replace("#", "\\#");
                        builder.AppendFormat("{0}={1}#", fieldname, value);
                    }
                    else
                    {
                        Log.Action.LogWarnFormat("Field [{0}] isn't exist", fieldname);
                    }
                }
            }

            builder.Append("#");
            argPrintData = builder.ToString();

            //if (HelperService.SpecialHandle.OpenAllLog)
            {
                Log.Action.LogInfoFormat("BuildPrintFormData is\r\n{0}", argPrintData);
            }

            return true;
        }

        public ReceiptPrinterWrapper ReceiptPrinter => m_objContext.ReceiptPrinterAlias(deviceName);
        #endregion
    }
}