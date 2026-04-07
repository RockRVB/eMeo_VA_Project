using BusinessServiceProtocol;
using FaceRecognitionService4SmartKioskProtocol;
using LogProcessorService;
using SessionRecordServiceProtocol.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VTMBusinessServiceProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessServiceProtocol
{
    /// <summary>
    /// 业务数据上下文，俗称数据总线
    /// </summary>
    public class IBankProjectBusinessServiceContext : VTMBusinessContext
    {
        #region constructor
        public IBankProjectBusinessServiceContext()
        {

        }
        #endregion
        private ISessionRecordService _sessionRecordService;
        private CustomerInfo m_CustomerInfo;
        public ISessionRecordService SessionRecordService
        {
            get
            {
                const string serviceName = "SessionRecordService";
                if (_sessionRecordService == null)
                {
                    IStandardFeatureService iResult;
                    if (BusinessService.QueryStandardFeature(serviceName, out iResult))
                    {
                        _sessionRecordService = (ISessionRecordService)iResult;
                        //Log.BusinessService.LogDebugFormat("Succeed to query standard feature [{0}]!", serviceName);
                    }
                    else
                    {
                        //Log.BusinessService.LogErrorFormat("Failed to query standard feature [{0}]!", serviceName);
                    }
                }
                return _sessionRecordService;
            }
        }
        public new CustomerInfo CustomerInfo
        {
            get
            {
                if (m_CustomerInfo == null)
                {
                    m_CustomerInfo = new CustomerInfo();
                }
                return m_CustomerInfo;
            }
            set
            {
                m_CustomerInfo = value;
            }
        }
    }
    public static class JsonExtension
    {
        /// <summary>
        /// 把对象转换为JSON字符串
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>JSON字符串</returns>
        public static string ToJSON(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 把对象转换为JSON字符串,忽略空值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSONIgnoreNullValue(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(obj, jSetting);
        }


        /// <summary>
        /// 把Json文本转为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T FromJSON<T>(this string input)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string ConvertToEngUpChar(this object obj)
        {
            string stFormD = obj.ToString().Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD).ToUpper());
        }

        public static string GetNameFromTrack1(this object track1)
        {
            string str_out = "";
            try
            {
                int a1 = track1.ToString().IndexOf('^');
                int b1 = track1.ToString().IndexOf('/');
                int a2 = track1.ToString().LastIndexOf('^');
                if (a1 == a2) str_out = "";
                else if (b1 == -1) str_out = track1.ToString().Substring(a1 + 1, a2 - a1 - 1).Trim();
                else
                {
                    str_out = track1.ToString().Substring(a1 + 1, b1 - a1 - 1).Trim();
                    str_out = str_out + " " + track1.ToString().Substring(b1 + 1, a2 - b1 - 1).Trim();
                }
                return str_out;
            }
            catch (System.Exception ex)
            {
                return str_out;
            }
        }
        public static string ProcessMaskData(this object Data)
        {
            string str_data = Data.ToString();
            try
            {
                if (str_data.Length > 6)
                {
                    str_data = str_data.Substring(0, 3).PadRight(str_data.Length - 3, '*') + str_data.Substring(str_data.Length - 3, 3);
                }
                else if (str_data.Length > 3)
                    str_data = str_data.Substring(0, 3).PadRight(str_data.Length, '*');
                return str_data;
            }
            catch (System.Exception ex)
            {
                return str_data;
            }
        }
        public static string CleanStringOfNonDigits(this object obj)
        {
            if (string.IsNullOrEmpty(obj.ToString())) return obj.ToString();
            string cleaned = new string(obj.ToString().Where(char.IsDigit).ToArray());
            return cleaned;
        }
        public static string Base64Encode(this object plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(this object base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData.ToString());
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

}
