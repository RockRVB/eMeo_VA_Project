using IBankProjectBusinessServiceProtocol;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTMBusinessActivity.VTMBankInterface;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Rendering;

namespace IBankProjectBankInterface
{
    public partial class IBankProjectBankInterface : VTMBankInterface
    {
        private void WriteJournalLogAfter(string argTransType, string resultTransCode, JObject dataObj)
        {
            try
            {
                switch (argTransType)
                {
                    case "VerifyQR":
                        ProjVTMContext.LogJournal($"Customer Name: {dataObj["data"]["customerName"]?.ToString()}");
                        ProjVTMContext.LogJournal($"Customer CIF: {dataObj["data"]["customerId"]?.ToString()}");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.XdcTrace.LogFatal("Exception: WriteJournalLogAfter Error!" + ex.Message);
            }
        }

        private void UnpackQueryCustomerInfo(JObject dataObj)
        {
            try
            {
                CustomerInfo customerInfo = new CustomerInfo();
                customerInfo.Accounts.Clear();
                customerInfo.CIF = dataObj["cif_information"]["customerId"]?.ToString();
                customerInfo.FullName = dataObj["cif_information"]["fullName"]?.ToString();

                foreach (var item in dataObj["accounts"])
                {
                    Account account = new Account();

                    account.AccountName = item["accountName"]?.ToString();
                    account.AccountNumber = item["accountNumber"]?.ToString();
                    account.AvailableBalance = item["availableBalance"]?.ToString();
                    account.AccountStatus = item["accountStatus"]?.ToString();
                    account.Currency = item["currency"]?.ToString();

                    customerInfo.Accounts.Add(account);
                }
                ProjVTMContext.TransactionDataCache.Set("VAB_CustomerInfo", customerInfo, GetType());
            }
            catch
            {
                ProjVTMContext.TransactionDataCache.Set("VAB_CustomerInfo", null, GetType());
            }
        }
        private void UnpackGetQRString(JObject dataObj)
        {
            try
            {
                string str_qr = dataObj["data"]["qr_code"]?.ToString();
                ProjVTMContext.TransactionDataCache.Set("VAB_QRData", str_qr, GetType());
            }
            catch
            {
                Log.XdcTrace.LogFatal("Exception: UnpackGetQRString Error!");
            }
        }
        private void UnpackVerifyQR(JObject dataObj)
        {
            try
            {
                if (dataObj["resCode"]?.ToString() == "0")
                { 

                }
                CustomerInfo customerInfo = new CustomerInfo();
                customerInfo.Accounts.Clear();
                customerInfo.CIF = dataObj["data"]["customerId"]?.ToString();
                customerInfo.FullName = dataObj["data"]["customerName"]?.ToString();
                ProjVTMContext.TransactionDataCache.Set("VAB_CustomerInfo", customerInfo, GetType());
            }
            catch
            {
                Log.XdcTrace.LogFatal("Exception: UnpackVerifyQR Error!");
            }
        }
        private static void GenerateImageQRImage(string QRdata)
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + "QRImage\\QRimage.jpg";
            //string PathLogo = AppDomain.CurrentDomain.BaseDirectory + "QRImage\\logo_big.png";
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions() { Width = 300, Height = 300, Margin = 0, PureBarcode = false };
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            Bitmap bitmap = barcodeWriter.Write(QRdata);
            //Bitmap logo = new Bitmap(PathLogo);
            //Graphics g = Graphics.FromImage(bitmap);
            //g.DrawImage(logo, new Point((bitmap.Width - logo.Width) / 2, (bitmap.Height - logo.Height) / 2));
            bitmap.Save(Path, ImageFormat.Jpeg);
        }
    }
}
