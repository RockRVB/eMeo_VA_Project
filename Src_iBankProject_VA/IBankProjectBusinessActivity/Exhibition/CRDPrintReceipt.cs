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

namespace VTMBusinessActivity
{
    [GrgActivity("{5B6714F7-1CB7-4D3A-ABB5-DD6EDDCB6474}",
                Name = "CRDPrintReceipt",
                NodeNameOfConfiguration = "CRDPrintReceipt",
                Description = "CRDPrintReceipt",
                Catalog = "PrintReceiptEx",
                ForwardTargets = new string[] { EventDictionary.s_EventConfirm, EventDictionary.s_EventFail, EventDictionary.s_EventCancel })]

    public class BusinessActivityCRDPrintReceipt : BusinessActivityPrintReceiptEx
    {
        #region method of creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new BusinessActivityCRDPrintReceipt();
        }
        #endregion

        #region constructor
        public BusinessActivityCRDPrintReceipt()
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

        private string m_frontImgPath = @"C:\xfs\Form\CardPrinter\Front.jpg";
        [GrgBindTarget("FrontImgPath", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string FrontImgPath
        {
            get
            {
                return m_frontImgPath;
            }
            set
            {
                m_frontImgPath = value;
                OnPropertyChanged("FrontImgPath");
            }
        }

        private string m_backImgPath = @"C:\xfs\Form\CardPrinter\Back.jpg";
        [GrgBindTarget("BackImgPath", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string BackImgPath
        {
            get
            {
                return m_backImgPath;
            }
            set
            {
                m_backImgPath = value;
                OnPropertyChanged("BackImgPath");
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

        private string m_dataPoolForCardHolder = "CardHolderToPrint";
        [GrgBindTarget("DataPoolForCardHolder", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string DataPoolForCardHolder
        {
            get
            {
                return m_dataPoolForCardHolder;
            }
            set
            {
                m_dataPoolForCardHolder = value;
                OnPropertyChanged("DataPoolForCardHolder");
            }
        }

        private string m_dataPoolForCCV2 = "CCV2ToPrint";
        [GrgBindTarget("DataPoolForCCV2", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string DataPoolForCCV2
        {
            get
            {
                return m_dataPoolForCCV2;
            }
            set
            {
                m_dataPoolForCCV2 = value;
                OnPropertyChanged("DataPoolForCCV2");
            }
        }
        #endregion

        #region override methods of base
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebug("Enter action: Print Receipt Ex");

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

                string CardNumber = "6888 8888 8888 8888";
                object objCardNumber;
                m_objContext.TransactionDataCache.Get(DataPoolForCardNumber,out objCardNumber,GetType());
                if (!string.IsNullOrEmpty(objCardNumber?.ToString()))
                {
                    CardNumber = objCardNumber.ToString();
                }

                string CardHolder = "BILL GATES";
                object objCardHolder;
                m_objContext.TransactionDataCache.Get(DataPoolForCardHolder, out objCardHolder, GetType());
                if (!string.IsNullOrEmpty(objCardHolder?.ToString()))
                {
                    CardHolder = objCardHolder.ToString();
                }

                string ExpriyDate = "12 / 25";
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string month = dt.Month.ToString("D2");
                string year = dt.AddYears(5).Year.ToString();
                year = year.Substring(year.Length - 2);
                ExpriyDate = month + " / " + year;
                m_objContext.TransactionDataCache.Set("CardExpriyDate", ExpriyDate, GetType());

                string CCV2 = "553 2469";
                object objCCV2;
                m_objContext.TransactionDataCache.Get(DataPoolForCCV2, out objCCV2, GetType());
                if (!string.IsNullOrEmpty(objCCV2?.ToString()))
                {
                    CCV2 = objCCV2.ToString();
                }

                //设置form的field字段
                m_objContext.TransactionDataCache.Set("FrontImgPath", FrontImgPath, GetType());
                m_objContext.TransactionDataCache.Set("BackImgPath", BackImgPath, GetType());
                m_objContext.TransactionDataCache.Set("CardNo", CardNumber, GetType());
                m_objContext.TransactionDataCache.Set("ExpriyDate", ExpriyDate, GetType());
                m_objContext.TransactionDataCache.Set("Name", CardHolder, GetType());
                m_objContext.TransactionDataCache.Set("CCV2", CCV2, GetType());

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
                    if (null!=devResult&&devResult.IsSuccess)
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

            Log.Action.LogDebug("Leave action: Print Receipt Ex");
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