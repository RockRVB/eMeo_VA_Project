using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using Newtonsoft.Json;
using RemoteTellerServiceProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using VTMBusinessActivityBase;
using VTMBusinessServiceProtocol;
using VTMModelLibrary;
using VTMModelLibrary.Packmodels;

namespace VTMBusinessActivity
{
    [GrgActivity("{B4D352E3-5032-48EA-AFA4-CA5DDA9BB687}",
                  Name = "ParseQRCodeData4Exhibition",
                  NodeNameOfConfiguration = "ParseQRCodeData4Exhibition",
                  Author = "")]
    public class ParseQRCodeData4Exhibition : BusinessActivityVTMBase
    {
        #region constructor
        private ParseQRCodeData4Exhibition()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new ParseQRCodeData4Exhibition();
        }
        #endregion

        private string m_dataPool = VTMDataDictionary.proj_UkeyNoForActive;
        [GrgBindTarget("DataPool", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string DataPool
        {
            get
            {
                return m_dataPool;
            }
            set
            {
                m_dataPool = value;
                OnPropertyChanged("DataPool");
            }
        }

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            VTMContext.NextCondition = EventDictionary.s_EventContinue;
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            try
            {
                SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);

                object objData;
                m_objContext.TransactionDataCache.Get(DataPool, out objData, GetType());

                string strQRCodeData = string.Empty;
                if (objData != null)
                {
                    strQRCodeData = objData.ToString();
                }

                QRCodeData QRData = JsonConvert.DeserializeObject<QRCodeData>(strQRCodeData);

                int stringSize = 4;
                var sb = new StringBuilder();
                //卡号每4位增加空格
                var i = 0;
                while (QRData.cardNo.Length > i)
                {
                    var str = QRData.cardNo.Substring(i, Math.Min(stringSize, QRData.cardNo.Length - i));
                    sb.Append(str+" ");
                    i = i + stringSize;
                }

                string cardNumber = sb.ToString().TrimEnd(' ');
                //End 卡号每4位增加空格
                m_objContext.TransactionDataCache.Set("CardHolderToPrint", QRData.name, GetType());
                m_objContext.TransactionDataCache.Set("CardNumberToPrint", cardNumber, GetType());
                m_objContext.TransactionDataCache.Set("CustomerPhoneNumber", QRData.phone, GetType());
                //过期时间，页面显示用
                string ExpriyDate = "12 / 25";
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string month = dt.Month.ToString("D2");
                string year = dt.AddYears(5).Year.ToString();
                year = year.Substring(year.Length - 2);
                ExpriyDate = month + " / " + year;
                m_objContext.TransactionDataCache.Set("CardExpriyDate", ExpriyDate, GetType());
                //End 过期时间，页面显示用
                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
                return emBusActivityResult_t.Success;
            }
            catch(Exception e)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("ParseQRCodeData4Exhibition Error:" + e.Message);
                return emBusActivityResult_t.Failure;
            }
            
        }

        #endregion
    }

    public class QRCodeData
    {
        public string name;
        public string cardNo;
        public string phone;
    }
}