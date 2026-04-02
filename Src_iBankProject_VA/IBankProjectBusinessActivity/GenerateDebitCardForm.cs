using Attribute4ECAT;
using BusinessServiceProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTMBusinessActivity.common;
using VTMModelLibrary;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{3B1A70FD-4CA5-424C-B993-7DD4BC8B5A20}",
              Name = "GenerateDebitCardForm",
              NodeNameOfConfiguration = "GenerateDebitCardForm",
              Author = "ltfei1")]
    public class GenerateDebitCardForm : GenerateFormDataBase
    {
        [GrgCreateFunction("Create")]
        public static new IBusinessActivity Create()
        {
            return new GenerateDebitCardForm() as IBusinessActivity;
        }

        protected override void GenerateData(ref Dictionary<string, string> dicPrintData)
        {
            object obj = null;
            VTMContext.TransactionDataCache.Get("PassportFormInfo", out obj, GetType());
            if (null != obj)
            {
                Dictionary<string, string> formInfomation = obj as Dictionary<string, string>;
                if (formInfomation != null)
                {
                    dicPrintData.Add("txtName", formInfomation["name"] ?? string.Empty);
                    dicPrintData.Add("txtSex", formInfomation["gender"] ?? string.Empty);
                    dicPrintData.Add("txtPassportNo", formInfomation["passportNo"] ?? string.Empty);
                    dicPrintData.Add("txtCountry", formInfomation["nation"] ?? string.Empty);
                    dicPrintData.Add("txtBirthdate", formInfomation["birthDate"] ?? string.Empty);
                    dicPrintData.Add("txtExpire", formInfomation["expire"] ?? string.Empty);
                    dicPrintData.Add("txtMobilePhone", formInfomation["phoneNum"] ?? string.Empty);
                    dicPrintData.Add("txtMail", formInfomation["email"] ?? string.Empty);
                    dicPrintData.Add("txtAddress", formInfomation["address"] ?? string.Empty);
                }

                dicPrintData.Add("txtSignDate", DateTime.Now.ToString("yyyy-MM-dd"));
                VTMContext.TransactionDataCache.Get("PassportHeadImage", out obj, GetType());
                if (obj != null)
                    dicPrintData.Add("imgHead", obj.ToString());
                VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_QRCODE, out obj, GetType());
                if (obj != null)
                    dicPrintData.Add("imgQRCode", obj.ToString());
                VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_CustomSignPath, out obj, GetType());
                if (obj != null)
                    dicPrintData.Add("imgSignature", obj.ToString());
            }
        }
    }
}
