using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using IDCardReaderProtocol;
using DevServiceProtocol;
using eCATBusinessServiceProtocol;
using VTMBusinessServiceProtocol;
using UIServiceProtocol;
using RemoteTellerServiceProtocol;
using VTMModelLibrary;
using System.IO;
using System.Globalization;

namespace VTMBusinessActivity.common
{
    [GrgActivity("{799B74D2-29FE-41BD-AD3C-432FBBEE1BF8}",
                  Name = "GenerateDitCarForm4Exhibition",
                  NodeNameOfConfiguration = "GenerateDitCarForm4Exhibition",
                  Author = "ltfei1")]
    public class GenerateDitCarForm4Exhibition : GenerateFormDataBase
    {

        [GrgCreateFunction("Create")]
        public static new IBusinessActivity Create()
        {
            return new GenerateDitCarForm4Exhibition() as IBusinessActivity;
        }

        private string GetCheckboxSrc(bool key)
        {

            if (key)
                return System.AppDomain.CurrentDomain.BaseDirectory + @"\Resource\Common\HTML\Print\EN\images\check.png";
            else
                return System.AppDomain.CurrentDomain.BaseDirectory + @"\Resource\Common\HTML\Print\EN\images\uncheck.png";
        }

        protected override void GenerateData(ref Dictionary<string, string> dicPrintData)
        {
            object obj;
            m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out obj, GetType());
            var idCardInfo = obj as ScanIDCardInfo;
            object obj1 = null;
            m_objContext.TransactionDataCache.Get("model", out obj1, GetType());
            // CreadCardInfo crdInfo = new CreadCardInfo();
            var crdInfo = (CreadCardInfo)obj1;
            //m_objContext.TransactionDataCache.Get("Print_Info", out obj2, GetType());
            // OtherService4ExhibitionClass OS4EInfo = new OtherService4ExhibitionClass();
            //  var OS4EInfo = (OtherService4ExhibitionClass)obj2;
            // if (null != idCardInfo && crdInfo != null && OS4EInfo != null)
            if (null != idCardInfo && crdInfo != null)
            {
                dicPrintData.Add("txtName", idCardInfo.IdCard_Name);
                dicPrintData.Add("txtSex", idCardInfo.IdCard_Sex);
                dicPrintData.Add("txtbirthday", idCardInfo.IdCard_Birthday);
                dicPrintData.Add("txtNationality", "Chinese");//hardcorde
                dicPrintData.Add("txtDateofIDIssue", idCardInfo.IdCard_BeginDate);
                dicPrintData.Add("txtPlaceofIDIssue", idCardInfo.IdCard_IDOrg);
                dicPrintData.Add("txtIDCardNumber", idCardInfo.IdCard_IDNo);
                dicPrintData.Add("txtMobilePhone", crdInfo.mobile);
                dicPrintData.Add("txtEmailAddress", "grgbanking@grgbanking.com");
                dicPrintData.Add("txtAddress", idCardInfo.IdCard_Address);
                dicPrintData.Add("txtContactAddress", idCardInfo.IdCard_Address);
                dicPrintData.Add("checkImg3", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg4", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg5", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg6", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg7", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg9", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg10", GetCheckboxSrc(false));
                dicPrintData.Add("checkNonRegistry", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg11", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg12", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg13", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg14", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg15", GetCheckboxSrc(false));
                //dicPrintData.Add("checkImg16", GetCheckboxSrc(OS4EInfo.Tanswer1Yes));
                //dicPrintData.Add("checkImg17", GetCheckboxSrc(OS4EInfo.Tanswer1No));
                //dicPrintData.Add("checkImg18", GetCheckboxSrc(OS4EInfo.Tanswer2Yes));
                //dicPrintData.Add("checkImg19", GetCheckboxSrc(OS4EInfo.Tanswer2No));
                //dicPrintData.Add("checkImg20", GetCheckboxSrc(OS4EInfo.Tanswer3Yes));
                //dicPrintData.Add("checkImg21", GetCheckboxSrc(OS4EInfo.Tanswer3No));
                //dicPrintData.Add("checkImg22", GetCheckboxSrc(OS4EInfo.Tanswer4Yes));
                //dicPrintData.Add("checkImg23", GetCheckboxSrc(OS4EInfo.Tanswer4No));
                dicPrintData.Add("checkImg16", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg17", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg18", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg19", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg20", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg21", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg22", GetCheckboxSrc(false));
                dicPrintData.Add("checkImg23", GetCheckboxSrc(false));
                dicPrintData.Add("imgRenewBoth", GetCheckboxSrc(false));
                dicPrintData.Add("imgRenewPrincipal", GetCheckboxSrc(false));
                dicPrintData.Add("imgRenewNothing", GetCheckboxSrc(false));
                dicPrintData.Add("txtOpenDate", DateTime.Now.ToString("dd/MM/yyyy"));

                string dateStr = DateTime.Now.ToString("yyyy-MM-dd");
                string timeStr = DateTime.Now.ToString("HH:mm:ss");

                object objSerial = null;
                VTMContext.TransactionDataCache.Get("core_SerialNumber", out objSerial);

                dicPrintData.Add("txtTime", timeStr);
                dicPrintData.Add("txtVTMNumber", VTMContext.TerminalConfig.Terminal.ATMNumber);
                if (objSerial != null)
                {
                    string serialNumber = objSerial.ToString();
                    dicPrintData.Add("txtSerialNumber", serialNumber);
                }
            }
            else
            {
                Log.Action.LogError("Data is null.");
            }
        }

    }

}

