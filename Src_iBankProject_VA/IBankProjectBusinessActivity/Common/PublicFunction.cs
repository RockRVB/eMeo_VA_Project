using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;//读取Ini文件的引用
using DataCacheServiceProtocol;
using LogProcessorService;
using eCATBusinessServiceProtocol;
using System.IO;
using System.Xml;
using System.Globalization;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Tamir.SharpSsh.jsch;
using System.Collections;
using System.Diagnostics;
using OCRServiceProtocol;
using IBankProjectBusinessActivityBase;
using BusinessServiceProtocol;

namespace IBankProjectBusinessActivity
{
    static public class CommonClass
    {
        public static void WriteEJLog (string strKey, string str = "   CUSTOMER CANCELED TRANSACTION")
        {
            if (!string.IsNullOrWhiteSpace(strKey) && (strKey.ToUpper() == "ONYES" || strKey.ToUpper() == "ONCANCEL"))
            {
                Log.ElectricJournal.LogInfo(str);
            }
        }
        public static void WriteEJLogForSignalResult(emWaitSignalResult_t waitResult, string str = "   CUSTOMER TIMEOUT")
        {
            if (waitResult == emWaitSignalResult_t.Timeout)
            {
                Log.ElectricJournal.LogInfo(str);
            }
        }
        public static int CompareDatetime(string date)
        {
            if (string.IsNullOrEmpty(date)) return -2;
            int res = 0;

            DateTime t1 = DateTime.ParseExact(date, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string now = DateTime.Now.ToString("d/M/yyyy");
            DateTime t2 = DateTime.ParseExact(DateTime.Now.ToString("d/M/yyyy"), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (t1 == t2)
            {
                res = 0;
            }
            else
            {
                if (t1 > t2)
                    res = 1;
                else res = -1;
            }
            return res;
        }
        public static int CompareDatetime(string date, int interval)
        {
            if (string.IsNullOrEmpty(date)) return -2;
            int res = 0;

            DateTime t1 = DateTime.ParseExact(date, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            t1 = t1.AddYears(interval);
            string now = DateTime.Now.ToString("d/M/yyyy");
            DateTime t2 = DateTime.ParseExact(DateTime.Now.ToString("d/M/yyyy"), "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (t1 == t2)
            {
                res = 0;
            }
            else
            {
                if (t1 > t2)
                    res = 1;
                else res = -1;
            }
            return res;
        }
        public static int CompareDatetime(string date1, string date2)
        {
            if (string.IsNullOrEmpty(date1) || string.IsNullOrEmpty(date2)) return -2;
            int res = 0;

            DateTime t1 = DateTime.ParseExact(date1, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime t2 = DateTime.ParseExact(date2, "d/M/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            if (t1 == t2)
            {
                res = 0;
            }
            else
            {
                if (t1 > t2)
                    res = 1;
                else res = -1;
            }
            return res;
        }
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith(".") || !trimmedEmail.Contains("."))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidFullname(string fullname)
        {   
            try
            {
                string trimmedFullname = string.Empty;
                if (!string.IsNullOrEmpty(fullname))
                {
                    trimmedFullname = fullname.Trim().Replace(" ", "");
                }
                else return false;
                string regexPatten = "^[a-zA-Z]+$";
                if (!Regex.IsMatch(trimmedFullname, regexPatten))
                    return false;
                else return true;
            }
            catch
            {
                return false;
            }
        }
        public static string ConverFormatMoney(string strMoney, string currency = "VND")
        {
            // moneyType = 0 => VN, 1 => USD
            string str = string.Empty;
            if (string.IsNullOrEmpty(strMoney))
                return str;
            string comma1 = ".";
            string comma2 = ",";
            string strTemp = string.Empty;
            string part1 = string.Empty;
            string part2 = string.Empty;
            if (currency != "VND")
            {
                comma1 = ",";
                comma2 = ".";
            }
            if (strMoney[0] == '-')
            {
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strTemp = "-";
            }
            string[] arr = strMoney.Split('.');
            part1 = arr[0];
            if (arr.Length == 2)
            {
                part2 = arr[1];
            }
            int len = part1.Length;
            int count = 0;
            for (int i = len - 1; i > 0; i--)
            {
                count++;
                if (count == 3)
                {
                    part1 = part1.Insert(i, comma1);
                    count = 0;
                }
            }
            str = part1;
            if (part2 != string.Empty) str += comma2 + part2;
            return strTemp + str;
        }
        public static void CustomernameFormat(string name, out string part1, out string part2)
        {
            part1 = string.Empty;
            part2 = string.Empty;
            if (name != null && name.Length > 17)
            {
                string[] temp = name.Split(' ');
                part1 = temp[0] + " " + temp[1] + " " + temp[2];
                for (int i = 3; i < temp.Length; i++)
                {
                    if (part2 != string.Empty)
                    {
                        part2 += " " + temp[i];
                    }
                    else
                    {
                        part2 += temp[i];
                    }
                }
            }
            else
            {
                part1 = name;
            }
        }
        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }

            int idxSpace = result.IndexOf(' ');
            TextInfo txtInfo = CultureInfo.CurrentCulture.TextInfo;
            result = (txtInfo.ToTitleCase(result.Substring(0, idxSpace)) + result.Substring(idxSpace)).Trim();

            if (isNegative) result = "Am " + result;
            return result + (suffix ? " Việt Nam đồng" : "");
        }

        public static string UppercaseFirstLetter(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return char.ToUpper(input[0]) + input.Substring(1);
            }
            return input;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string FormatCardNumber(string cardNumber)
        {
            if (cardNumber.Length != 16)
            {
                return "Số thẻ không hợp lệ.";
            }

            char[] formattedNumber = new char[16];

            Array.Copy(cardNumber.ToCharArray(), formattedNumber, 6);
            Array.Copy(cardNumber.ToCharArray(), 12, formattedNumber, 12, 4);

            for (int i = 6; i < 12; i++)
            {
                formattedNumber[i] = 'X';
            }

            return new string(formattedNumber, 0, formattedNumber.Length)
                .Insert(4, " ")
                .Insert(9, " ")
                .Insert(14, " ");
        }

        public static string MaskCardNumber(string cardNumber)
        {
            if (cardNumber.Length < 16)
            {
                throw new ArgumentException("Card number must be at least 16 characters long.");
            }

            string part1 = cardNumber.Substring(0, 6);
            string part2 = "xxxxxx";
            string part3 = cardNumber.Substring(cardNumber.Length - 4);
            string maskedCardNumber = $"{part1}-{part2}-{part3}";

            return maskedCardNumber;
        }

        public static string ConverFormatMoney1(string strMoney, string currency = "VND")
        {
            // moneyType = 0 => VN, 1 => USD
            string str = string.Empty;
            if (string.IsNullOrEmpty(strMoney))
                return str;
            string comma1 = ".";
            string comma2 = ",";
            string strTemp = string.Empty;
            string part1 = string.Empty;
            string part2 = string.Empty;
            if (currency != "VND")
            {
                comma1 = ",";
                comma2 = ".";
            }
            if (strMoney[0] == '-')
            {
                strMoney = strMoney.Substring(1, strMoney.Length - 1);
                strTemp = "-";
            }
            string[] arr = strMoney.Split('.');
            part1 = arr[0];
            if (arr.Length == 2)
            {
                part2 = arr[1];
            }
            int len = part1.Length;
            int count = 0;
            for (int i = len - 1; i > 0; i--)
            {
                count++;
                if (count == 3)
                {
                    part1 = part1.Insert(i, comma2);
                    count = 0;
                }
            }
            str = part1;
            if (part2 != string.Empty) str += comma2 + part2;
            return strTemp + str;
        }
        public static string ConvertMoney2(string strMoney, string type = "0")
        {
            string res = strMoney;
            
            try
            {
                if (type == "0") // 20000 => 20 000
                {
                    string cleanStr = string.Empty;
                    if (strMoney != null)
                    {
                        cleanStr = strMoney.Replace(" ", "");
                        if (long.TryParse(cleanStr, out long number))
                        {
                            res = number.ToString("#,##0").Replace(',', ',');
                        }
                    }
                }
                else if (type == "1")// 20,000 => 20000
                {
                    if (strMoney != null)
                    {
                        res = strMoney.Replace(",", "");
                    }
                }
                
            }
            catch { }
            return res;
        }
    }

    public class OCRNumberSerial : IBankProjectActivityBase
    {

        INIClass iniClass;
        private string GetOcrPrintInfo(List<OCRInfoDataStruct> ocrList)
        {
            try
            {
                Log.Action.LogDebugFormat("Being GetOcrPrintInfo");
                if (ocrList == null || ocrList.Count == 0)
                    return string.Empty;

                string denomination = "   Others: ";
                Log.Action.LogDebugFormat("ocrList[0].NoteID:{0}", ocrList[0].NoteID);
                if (ocrList[0].NoteID.Equals("1425")) denomination = "   10000";
                else if (ocrList[0].NoteID.Equals("1426")) denomination = "   20000";
                else if (ocrList[0].NoteID.Equals("1427")) denomination = "   50000";
                else if (ocrList[0].NoteID.Equals("1428")) denomination = "   100000";
                else if (ocrList[0].NoteID.Equals("1429")) denomination = "   200000";
                else if (ocrList[0].NoteID.Equals("1430")) denomination = "   500000";

                StringBuilder sb = new StringBuilder();
                sb.Append(denomination).Append(":");
                foreach (OCRInfoDataStruct ocrResult in ocrList)
                    sb.Append(ocrResult.SerialNumber + "*");
                Log.Action.LogDebugFormat("sb:{0}", sb);
                return sb.ToString().Remove(sb.Length - 1);
            }
            catch (Exception ex) {
                Log.Action.LogDebugFormat("GetOcrPrintInfo err:{0}", ex.ToString());
                return string.Empty;
            }
        }

        public void PrintOcrInfo()
        {
            try
            {
                HelperService.SolidXmlDocument m_adoc = new HelperService.SolidXmlDocument();
                string m_apath = AppDomain.CurrentDomain.BaseDirectory + @"Config\Services\OCRService.xml";

                m_adoc.Load(m_apath);
                string path = m_adoc.DocumentElement.SelectSingleNode(@"Format4Ini/@iniFileFolder").Value;
                path += @"\SNRInfo.ini";
                Log.Action.LogDebug("iniFileFolder:" + path);

                iniClass = new INIClass(path);

                List<OCRInfoDataStruct> ocrInfoList = new List<OCRInfoDataStruct>();
                //m_objContext.OCRService.GetLastOCRInfo(ref ocrInfoList);
                GetOCRInfo(ref ocrInfoList);
                if (ocrInfoList == null || ocrInfoList.Count == 0) return;

                string tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1430")).ToList());

                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);
                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1429")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1428")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1427")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1426")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => q.NoteID.Equals("1425")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

                tmpStr = string.Empty;
                tmpStr = GetOcrPrintInfo(ocrInfoList.Where(q => !q.NoteID.Equals("1425") && !q.NoteID.Equals("1426") && !q.NoteID.Equals("1427") && !q.NoteID.Equals("1428") && !q.NoteID.Equals("1429") && !q.NoteID.Equals("1430")).ToList());
                if (tmpStr != string.Empty) Log.ElectricJournal.LogInfo(tmpStr);

            }
            catch (Exception ex) { Log.Action.LogDebugFormat("PrintOcrInfo err:{0}", ex.ToString()); }
        }

        public void GetOCRInfo(ref List<OCRInfoDataStruct> ocrInfoList)
        {
            string LEVEL2_COUNT = iniClass.IniReadValue("Cash_Info", "LEVEL2_COUNT");
            string LEVEL3_COUNT = iniClass.IniReadValue("Cash_Info", "LEVEL3_COUNT");
            string LEVEL4_COUNT = iniClass.IniReadValue("Cash_Info", "LEVEL4_COUNT");
            int level2Count = int.Parse(LEVEL2_COUNT);
            int level3Count = int.Parse(LEVEL3_COUNT);
            int level4Count = int.Parse(LEVEL4_COUNT);

            LoadDataLevel(4, level4Count, ref ocrInfoList);
            LoadDataLevel(3, level3Count, ref ocrInfoList);
            LoadDataLevel(2, level2Count, ref ocrInfoList);
        }
        public void LoadDataLevel(int level, int count, ref List<OCRInfoDataStruct> ocrInfoList)
        {
            string levelIdx = string.Empty;
            string levelName = string.Empty;
            for (int i = 0; i < count; i++)
            {
                OCRInfoDataStruct item = new OCRInfoDataStruct();
                levelIdx = (i + 1).ToString().PadLeft(3, '0');
                levelName = "LEVEL" + level.ToString() + "_" + levelIdx.ToString();
                int deno = 0;
                string strValue = iniClass.IniReadValue(levelName, "Value");
                int.TryParse(strValue, out deno);
                item.Denomination = deno;

                int index = 0;
                string strIndex = iniClass.IniReadValue(levelName, "Index");
                int.TryParse(strValue, out index);
                item.Index = index;
                item.LevelType = level;

                item.NoteID = iniClass.IniReadValue(levelName, "NoteID");
                item.Release = iniClass.IniReadValue(levelName, "Release");
                item.SerialNumber = iniClass.IniReadValue(levelName, "SerialNumber");
                item.ImageFile = iniClass.IniReadValue(levelName, "ImageFile");
                //item.Currency = iniClass.IniReadValue(levelName, "Currency");

                ocrInfoList.Add(item);
            }
        }
    }
    public class INIClass
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        /// <summary> 
        /// 构造方法 
        /// </summary> 
        /// <param name="INIPath">文件路径</param> 
        public INIClass(string INIPath)
        {
            inipath = INIPath;
        }
        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        /// <param name="Value">值</param> 
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }
        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="Section">项目名称(如 [TypeName] )</param> 
        /// <param name="Key">键</param> 
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }
        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }
    }
    public class FtpConfig
    {
        #region constructor
        public FtpConfig()
        {
            InitializeFtpConfig();
        }
        #endregion
        private void InitializeFtpConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();//载入xml文档
            string strPath = AppDomain.CurrentDomain.BaseDirectory + @"Config\FTPConfig.xml";

            try
            {
                xmlDoc.Load(strPath);
                _uri = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["uri"].Value;
                _user = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["user"].Value;
                _port = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["port"].Value;
                _pwd = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["pwd"].Value;
                _remotepath = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["remotepath"].Value;
                _keyfile = xmlDoc.SelectSingleNode(@"Config/FTP").Attributes["keyfile"].Value;
            }
            catch (System.Exception ex)
            {
                Log.Root.LogFatal("load FTPConfig.xml fatal error", ex);
            }
        }

        private string _uri;
        public string Uri
        {
            get
            {
                return _uri;
            }
        }

        private string _user;
        public string User
        {
            get
            {
                return _user;
            }
        }

        private string _pwd;
        public string Pwd
        {
            get
            {
                return _pwd;
            }
        }

        private string _keyfile;
        public string Keyfile
        {
            get
            {
                return _keyfile;
            }
        }

        private string _remotepath;
        public string Remotepath
        {
            get
            {
                return _remotepath;
            }
        }
        private string _port;
        public string Port
        {
            get
            {
                return _port;
            }
        }
    }
    public class upSftpLoadFile
    {
        public static bool upLoadFile(string argFilePath, string argFileGargetPath, string originalName)
        {
            Log.Project.LogDebug("argFilePath : " + argFilePath);
            Log.Project.LogDebug("argFileGargetPath : " + argFileGargetPath);
            Log.Project.LogDebug("originalName : " + originalName);
            FtpConfig ftpconfig = new FtpConfig();
            sftpHelper sftp = new sftpHelper(ftpconfig.Uri, ftpconfig.User, ftpconfig.Pwd, ftpconfig.Keyfile, 22);
            try
            {
                if (sftp.Connect() && originalName != string.Empty)
                {
                    //Log.Project.LogDebug("UPLOAD FILE:" + zipFilePath.ToString() + " CONNECT SUCCESS!");
                    //Log.Project.LogDebug("target path :" + ftpconfig.Remotepath + terminalID + "_TEMPORARY");

                    if (sftp.Put(argFilePath, argFileGargetPath))
                    {
                        Log.Project.LogDebug("put file succeed go to next move file:" + originalName);
                        Thread.Sleep(5000);
                        if (sftp.Move(argFileGargetPath, originalName))
                        {
                            Log.Project.LogDebug("upload succeed :" + originalName);
                            sftp.Disconnect();
                            Thread.Sleep(3000);
                            return true;
                            //m_objContext.LogJournal("UPLOAD FILE:" + zipFilePath.ToString() + " SUCCESS!");
                            //Log.Project.LogDebug("UPLOAD MOVE:" + zipFilePath.ToString() + " MOVE SUCCESS!");
                        }
                        else
                        {
                            Log.Project.LogDebug("move file failed :" + originalName);
                            sftp.Disconnect();
                            Thread.Sleep(3000);
                            return false;
                        }
                    }
                    else
                    {
                        Log.Project.LogDebug("put file failed :" + originalName);
                        sftp.Disconnect();
                        Thread.Sleep(3000);
                        return false;
                    }
                }
                else
                {
                    Log.Project.LogDebug("connect failed :" + originalName);
                    sftp.Disconnect();
                    Thread.Sleep(3000);
                    return false;
                }

                // sftp.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("connect failed :" + ex.Message);
            }
            sftp.Disconnect();
            Thread.Sleep(3000);
            return false;
        }
    }
    public class sftpHelper
    {
        private Session m_session;
        private Channel m_channel;
        private ChannelSftp m_sftp;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">sftp地址</param>
        /// <param name="user">sftp用户名</param>
        /// <param name="pwd">sftp密码</param>
        /// <param name="port">端口，默认20</param>
        //public sftpHelper(string ip, string user, string pwd, string port = "2088")
        //{
        //    int serverport = Int32.Parse(port);

        //    JSch jsch = new JSch();
        //    m_session = jsch.getSession(user, ip, serverport);

        //    MyUserInfo ui = new MyUserInfo();
        //    ui.setPassword(pwd);
        //    m_session.setUserInfo(ui);
        //}
        //
        public sftpHelper(string ip, string user, string pwd, string keypatch, int port)
        {
            Log.Project.LogDebugFormat("Begin to connect the sftp server,ip is :{0},user is :{1},keypatch is :{2},port is {3}.", ip, user, keypatch, port);
            int serverport = port;

            JSch jsch = new JSch();
            Log.Project.LogDebug("begin to load keypatch ");
            jsch.addIdentity(keypatch);
            Log.Project.LogDebug("load keypatch success");
            m_session = jsch.getSession(user, ip, serverport);
            Log.Project.LogDebug("getSession success");
            MyUserInfo ui = new MyUserInfo();
            ui.setPassword(pwd);
            m_session.setUserInfo(ui);
            Log.Project.LogDebug("setUserInfo success");
        }

        /// <summary>
        /// 连接状态
        /// </summary>     
        public bool Connected { get { return m_session.isConnected(); } }

        /// <summary>
        /// 连接SFTP
        /// </summary>     
        public bool Connect()
        {
            try
            {
                if (!Connected)
                {
                    m_session.connect();
                    m_channel = m_session.openChannel("sftp");
                    m_channel.connect();
                    m_sftp = (ChannelSftp)m_channel;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] Connect  failed! " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 断开SFTP 
        /// </summary> 
        public void Disconnect()
        {
            if (Connected)
            {
                m_channel.disconnect();
                m_session.disconnect();
            }
        }

        /// <summary>
        /// SFTP存放文件  
        /// </summary> 
        /// <param name="localPath">本地文件路径</param>
        /// <param name="remotePath">sftp远程地址</param>
        public bool Put(string localPath, string remotePath)
        {
            try
            {
                if (this.Connected)
                {
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                    m_sftp.put(src, dst);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] Put  failed! " + ex.Message);
                return false;
            }
            return false;
        }

        /// <summary>
        /// SFTP获取文件  
        /// </summary> 
        /// <param name="remotePath">sftp远程文件地址</param>
        /// <param name="localPath">本地文件存放路径</param>  
        public bool Get(string remotePath, string localPath)
        {
            try
            {
                if (this.Connected)
                {
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
                    m_sftp.get(src, dst);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] Get failed! " + ex.Message);
                return false;
            }
            return false;
        }

        /// <summary>
        /// 删除SFTP文件  
        /// </summary> 
        /// <param name="remoteFile">sftp远程文件地址</param>
        public bool Delete(string remoteFile)
        {
            try
            {
                if (this.Connected)
                {
                    m_sftp.rm(remoteFile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] Delete failed! " + ex.Message);
                return false;
            }
            return false;
        }


        /// <summary>
        /// 移动SFTP文件  
        /// </summary> 
        /// <param name="currentFilename">sftp远程文件地址</param>
        /// <param name="newDirectory">sftp移动至文件地址</param>
        public bool Move(string currentFilename, string newDirectory)
        {
            try
            {
                if (this.Connected)
                {
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(currentFilename);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(newDirectory);
                    m_sftp.rename(src, dst);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] Move failed! " + ex.Message);
                return false;
            }
            return false;
        }

        /// <summary>
        /// 获取SFTP文件列表  
        /// </summary> 
        /// <param name="remotePath">sftp远程文件目录</param>
        /// <param name="fileType">文件类型</param>
        public ArrayList GetFileList(string remotePath, string fileType)
        {
            try
            {

                if (this.Connected)
                {
                    Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);
                    ArrayList objList = new ArrayList();
                    foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
                    {
                        string sss = qqq.getFilename();
                        if (sss.Length > (fileType.Length + 1) && fileType == sss.Substring(sss.Length - fileType.Length))
                        { objList.Add(sss); }
                        else { continue; }
                    }

                    return objList;
                }
            }
            catch (Exception ex)
            {
                Log.Project.LogDebug("[Cheque] GetFileList failed! " + ex.Message);
                return null;
            }
            return null;
        }

        //登录验证信息   
        public class MyUserInfo : UserInfo
        {
            String passwd;

            public String getPassword() { return passwd; }
            public void setPassword(String passwd) { this.passwd = passwd; }

            public String getPassphrase() { return null; }
            public bool promptPassphrase(String message) { return true; }

            public bool promptPassword(String message) { return true; }
            public bool promptYesNo(String message) { return true; }
            public void showMessage(String message) { }
        }
    }
    public class CompressFile
    {
        /// <summary>
        /// 用7z.exe压缩文件
        /// </summary>
        /// <param name="argBuilder"></param>
        /// <param name="argDest"></param>
        /// <returns></returns>
        public bool Compress(StringBuilder argBuilder, string argDest)
        {
            Log.Project.LogDebug("Begin to compress the picture ");
            bool bResult = false;

            //防止文件名过多过长导致压缩不成功。 
            string CompressDirectory = string.Empty;

            //增加异常处理，防止压缩失败还保存临时文件。
            try
            {
                if (File.Exists(argDest))
                {
                    File.Delete(argDest);
                }
                if (!string.IsNullOrEmpty(argDest))
                {
                    Log.Project.LogDebug("Begin to deal with the compress when argDest is not null");
                    //防止文件名过多过长导致压缩不成功。
                    string CompressDirectoryName = argDest.Split('\\').LastOrDefault(a => (!string.IsNullOrEmpty(a.Trim().ToString()))).Split('.').FirstOrDefault(a => (!string.IsNullOrEmpty(a.Trim().ToString())));
                    CompressDirectory = AppDomain.CurrentDomain.BaseDirectory + "temp\\" + CompressDirectoryName + "\\";

                    string strExeName = AppDomain.CurrentDomain.BaseDirectory + "7z.exe";
                    if (!File.Exists(strExeName))
                    {
                        Log.MaintenanceAction.LogError("7z.exe file is not found");
                        return bResult;
                    }

                    Debug.Assert(argBuilder.Length > 0);
                    string strDirectory = Path.GetDirectoryName(argDest);
                    Log.Project.LogDebugFormat("The argDest is :{0},the strDirectory is:{1} ", argDest, strDirectory);
                    if (!Directory.Exists(strDirectory))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(strDirectory);
                        di = null;
                    }
                    //*******************************************************************
                    //防止文件名过多过长导致压缩不成功。
                    string Marks = "\"";
                    string ReplaceMarks = "<";
                    if (!Directory.Exists(CompressDirectory))
                    {
                        Directory.CreateDirectory(CompressDirectory);
                    }
                    Log.Project.LogDebugFormat("the CompressDirectory is :{0}", CompressDirectory);
                    foreach (string SourceCompressDirectory in argBuilder.ToString().Replace(" " + Marks, ReplaceMarks).Split('<').Where(a => (!string.IsNullOrEmpty(a.Trim().ToString()))))
                    {
                        if (!string.IsNullOrEmpty(SourceCompressDirectory.Trim()))
                        {
                            if (File.Exists(SourceCompressDirectory))
                            {
                                Log.Project.LogDebug("deal with the filename ");
                                string FileLastName = SourceCompressDirectory.Split('\\').LastOrDefault(a => (!string.IsNullOrEmpty(a.Trim().ToString())));
                                Log.Project.LogDebug("the FileLastName is " + FileLastName);
                                if (File.Exists(CompressDirectory + FileLastName))
                                {
                                    File.Delete(CompressDirectory + FileLastName);
                                }
                                Log.Project.LogDebugFormat("the SourceCompressDirectory is :{0},the CompressDirectory + FileLastName is :{1}", SourceCompressDirectory, CompressDirectory + FileLastName);
                                File.Copy(SourceCompressDirectory, CompressDirectory + FileLastName);
                            }
                            else if (Directory.Exists(SourceCompressDirectory))
                            {
                                Log.Project.LogDebugFormat("copy from SourceCompressDirectory {0} to CompressDirectory {1} ", SourceCompressDirectory, CompressDirectory);
                                CopyDirectory(SourceCompressDirectory, CompressDirectory);
                            }
                        }
                    }

                    using (Process proc = new Process())
                    {
                        //proc.StartInfo.FileName = strPath;
                        //proc.StartInfo.Arguments = " a -ep1 -m1 " + argDest + argBuilder.ToString();
                        proc.StartInfo.FileName = strExeName;
                        //proc.StartInfo.Arguments = string.Format(" a -t{0} -mx7 -ssw {1} {2}", Path.GetExtension(argDest).TrimStart('.'), argDest, argBuilder.ToString());
                        proc.StartInfo.Arguments = string.Format(" a -t{0} -mx7 -ssw {1} {2}", Path.GetExtension(argDest).TrimStart('.'), argDest, CompressDirectory + "*");
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.StartInfo.UseShellExecute = true;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.Start();
                        proc.WaitForExit();
                        bResult = true;
                    }

                    //delete bat file
                    //File.Delete(strPath);
                    //*****************************************************************

                    //防止文件名过多过长导致压缩不成功。
                    if (Directory.Exists(CompressDirectory))
                    {
                        Directory.Delete(CompressDirectory, true);
                    }

                }
                else
                {
                    Log.Project.LogDebug("argDest is not null");
                }


            }
            catch
            {
                //防止文件名过多过长导致压缩不成功。
                if ((!string.IsNullOrEmpty(CompressDirectory.Trim())) && Directory.Exists(CompressDirectory))
                {
                    Directory.Delete(CompressDirectory, true);
                }

                return false;
            }
            return bResult;
        }

        //增加一个拷贝目录的方法，方便压缩文件时递归循环查找子目录。
        public void CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }
            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name);
            }
        }
    }
    [Serializable]
    public class TransRecord
    {
        public Int64 normalDepCount { get; set; }
        public Int64 normalDepAmount { get; set; }

        public Int64 normalCwdCount { get; set; }
        public Int64 normalCwdAmount { get; set; }

        public Int64 noNormalDepCount { get; set; }
        public Int64 noNormalDepAmount { get; set; }

        public Int64 noNormalCwdCount { get; set; }
        public Int64 noNormalCwdAmount { get; set; }
    }
    public static class JournalDictionary
    {
        public const string TEXT_NAM = "Nam";
        public const string TEXT_NU = "Nữ";
        public const string TEXT_MALE = "MALE";
        public const string TEXT_FEMALE = "FEMALE";
        public const string TEXT_INSTANTISSUEATMCARD = "Nhận ngay tại máy VTM";

        public const string NOTE_ID_500 = "1430";
        public const string NOTE_ID_200 = "1429";
        public const string NOTE_ID_100 = "1428";
        public const string NOTE_ID_050 = "1427";

        public const string IDS_DepositToAccount = "IDS_DepositToAccount";
        public const string IDS_DepositAmount = "IDS_DepositAmount";
        public const string IDS_DepositTotalAmount = "IDS_DepositTotalAmount";
        public const string IDS_DepositSelectBank = "IDS_DepositSelectBank";
        public const string IDS_DepositSelectBankID = "IDS_DepositSelectBankID";
        public const string IDS_DepositSelectBankName = "IDS_DepositSelectBankName";

        public const string IDS_ATMNumber = "IDS_ATMNumber";
        public const string IDS_CurrentDateTime = "IDS_CurrentDateTime";

        public const string IDS_TransactionRequest = "IDS_TransactionRequest";
        public const string IDS_TransactionCancel = "IDS_TransactionCancel";
        public const string IDS_TransactionReply = "IDS_TransactionReply";
        public const string IDS_TransactionReply_RefNumber = "IDS_TransactionReply_RefNumber";
        public const string IDS_TransactionReply_ResponseCode = "IDS_TransactionReply_ResponseCode";
        public const string IDS_TransactionReplyErrorMessage = "IDS_TransactionReplyErrorMessage";
        public const string IDS_TransactionPINEntered = "IDS_TransactionPINEntered";
        public const string IDS_TransactionPINCancel = "IDS_TransactionPINCancel";
        public const string IDS_TransactionPINTimeout = "IDS_TransactionPINTimeout";
        public const string IDS_TransactionRefNumber = "IDS_TransactionRefNumber";

        public const string IDS_TransactionCode = "IDS_TransactionCode";
        public const string IDS_TransactionDate = "IDS_TransactionDate";

        public const string IDS_PassbookSavingType = "IDS_PassbookSavingType";
        public const string IDS_PassbookAccountNumber = "IDS_PassbookAccountNumber";
        public const string IDS_PassbookTerm = "IDS_PassbookTerm";
        public const string IDS_PassbookRate = "IDS_PassbookRate";
        public const string IDS_PassbookValueDate = "IDS_PassbookValueDate";
        public const string IDS_PassbookMaturityDate = "IDS_PassbookMaturityDate";
        public const string IDS_PassbookBlockadedAmount = "IDS_PassbookBlockadedAmount";
        public const string IDS_PassbookBlockadedReason = "IDS_PassbookBlockadedReason";
        public const string IDS_PassbookTransferToAccount = "IDS_PassbookTransferToAccount";
        public const string IDS_PassbookAccountBalance = "IDS_PassbookAccountBalance";
        public const string IDS_PassbookInterestPaymentToAccount = "IDS_PassbookInterestPaymentToAccount";
        public const string IDS_PassbookRenewalMode = "IDS_PassbookRenewalMode";

        public const string IDS_PassbookIsSelectPromotion = "IDS_PassbookIsSelectPromotion";
        public const string IDS_PassbookInfomation = "IDS_PassbookInfomation";
        public const string IDS_PassbookNumber = "IDS_PassbookNumber";
        public const string IDS_PassbookInfo = "IDS_PassbookInfo";
        public const string IDS_PassbookDate = "IDS_PassbookDate";
        public const string IDS_PassbookAmount = "IDS_PassbookAmount";
        public const string IDS_PassbookType = "IDS_PassbookType";

        public const string IDS_CustomerInputInformation = "IDS_CustomerInputInformation";
        public const string IDS_CustomerCIF = "IDS_CustomerCIF";
        public const string IDS_CustomerName = "IDS_CustomerName";
        public const string IDS_CustomerID = "IDS_CustomerID";
        public const string IDS_CustomerAddress = "IDS_CustomerAddress";
        public const string IDS_CustomerGender = "IDS_CustomerGender";
        public const string IDS_CustomerDoB = "IDS_CustomerDoB";
        public const string IDS_CustomerIssuePlace = "IDS_CustomerIssuePlace";
        public const string IDS_CustomerIssueDate = "IDS_CustomerIssueDate";
        public const string IDS_CustomerNationality = "IDS_CustomerNationality";
        public const string IDS_CustomerPhoneNumber = "IDS_CustomerPhoneNumber";
        public const string IDS_CustomerEmail = "IDS_CustomerEmail";
        public const string IDS_CustomerNumberOfAccounts = "IDS_CustomerNumberOfAccounts";
        public const string IDS_CustomerSelectAccount = "IDS_CustomerSelectAccount";
        public const string IDS_CustomerSelectInputAccount = "IDS_CustomerSelectInputAccount";

        public const string IDS_CustomerNewAccountCreated = "IDS_CustomerNewAccountCreated";
        public const string IDS_CustomerNewAccountCIFNo = "IDS_CustomerNewAccountCIFNo";
        public const string IDS_CustomerNewAccountAccountNo = "IDS_CustomerNewAccountAccountNo";

        public const string IDS_CardRegistration = "IDS_CardRegistration";
        public const string IDS_CardRegistration_Visa = "IDS_CardRegistration_Visa";
        public const string IDS_CardRegistration_VisaStandard = "IDS_CardRegistration_VisaStandard";
        public const string IDS_CardRegistration_VisaGold = "IDS_CardRegistration_VisaGold";
        public const string IDS_CardRegistration_VisaPlatinum = "IDS_CardRegistration_VisaPlatinum";
        public const string IDS_CardRegistration_eCounter = "IDS_CardRegistration_eCounter";

        public const string IDS_CardIssuing_Instant = "IDS_CardIssuing_Instant";
        public const string IDS_CardIssuing_Address = "IDS_CardIssuing_Address";
        public const string IDS_CardIssuing_Branch = "IDS_CardIssuing_Branch";
        public const string IDS_CardIssuing_BranchName = "IDS_CardIssuing_BranchName";
        public const string IDS_CardIssuing_BranchAddress = "IDS_CardIssuing_BranchAddress";

        public const string IDS_eBankRegistration = "IDS_eBankRegistration";
        public const string IDS_eBankRegistration_Soft = "IDS_eBankRegistration_Soft";
        public const string IDS_eBankRegistration_SMS = "IDS_eBankRegistration_SMS";

        public const string IDS_NotificationSMS = "IDS_NotificationSMS";
        public const string IDS_NotificationTotalPhoneNumbers = "IDS_NotificationTotalPhoneNumbers";
        public const string IDS_NotificationPhoneNumbers = "IDS_NotificationPhoneNumbers";

        public const string IDS_FingerprintRegistration = "IDS_FingerprintRegistration";
        public const string IDS_Success = "IDS_Success";
        public const string IDS_Fail = "IDS_Fail";
        public const string IDS_FingerprintVerify = "IDS_FingerprintVerify";
        public const string IDS_FingerprintVerifyCount = "IDS_FingerprintVerifyCount";
        public const string IDS_FingerprintVerifyNotReachApplication = "IDS_FingerprintVerifyNotReachApplication";

        public const string IDS_CassetteStatus = "IDS_CassetteStatus";
        public const string IDS_CassetteStatusEmpty = "IDS_CassetteStatusEmpty";
        public const string IDS_CassetteStatusLow = "IDS_CassetteStatusLow";
        public const string IDS_CassetteStatusError = "IDS_CassetteStatusError";

        public const string IDS_OCRDetails = "IDS_OCRDetails";
        public const string IDS_OCRSuspiciousNote = "IDS_OCRSuspiciousNote";
        public const string IDS_OCRDenomination500 = "IDS_OCRDenomination500";
        public const string IDS_OCRDenomination200 = "IDS_OCRDenomination200";
        public const string IDS_OCRDenomination100 = "IDS_OCRDenomination100";
        public const string IDS_OCRDenomination050 = "IDS_OCRDenomination050";

        public const string CARD_PAN_ATM = "970423";
        public const string CARD_PAN_VISA = "4665";
        public const string CARD_ATM = "ATM";
        public const string CARD_VISA = "VISA";
        public const string IDS_CardBoxDispense = "IDS_CardBoxDispense";
        public const string IDS_CardBoxSupply = "IDS_CardBoxSupply";
        public const string IDS_CardBoxSupplyDetails = "IDS_CardBoxSupplyDetails";
        public const string IDS_CardBoxSupplyRetain = "IDS_CardBoxSupplyRetain";

        public const string IDS_CardBoxError = "IDS_CardBoxError";
        public const string IDS_CardBoxErrorDispense = "IDS_CardBoxErrorDispense";
        public const string IDS_CardBoxErrorReadRAW = "IDS_CardBoxErrorReadRAW";
        public const string IDS_CardBoxErrorPrint = "IDS_CardBoxErrorPrint";
        public const string IDS_CardBox_RetainBin = "IDS_CardBox_RetainBin";
        public const string IDS_CardBoxErrorNoBoxSelected = "IDS_CardBoxErrorNoBoxSelected";
        public const string IDS_CardBox_CaptureCard = "IDS_CardBox_CaptureCard";
        public const string IDS_CardBox_CaptureReset = "IDS_CardBox_CaptureReset";
        public const string IDS_CardBox_CaptureReturn = "IDS_CardBox_CaptureReturn";
        public const string IDS_CardBox_CaptureCardSuccessful = "IDS_CardBox_CaptureCardSuccessful";
        public const string IDS_CardBox_CapturePAN = "IDS_CardBox_CapturePAN";

        public const string IDS_CardBox_DispensePAN = "IDS_CardBox_DispensePAN";

        public const string IDS_EjectCardFailure = "IDS_EjectCardFailure";
        public const string IDS_CardTaken = "IDS_CardTaken";

        public const string IDS_ATMCardInserted = "IDS_ATMCardInserted";
        public const string IDS_ATMCardName = "IDS_ATMCardName";
        public const string IDS_CustomerIcType = "IDS_CustomerIcType";

        public const string IDS_CustomerAddInformation = "IDS_CustomerAddInformation";
        public const string IDS_CustomerOCRInformation = "IDS_CustomerOCRInformation";
        public const string IDS_CustomerName2 = "IDS_CustomerName2";

        public const string IDS_CustomerRegistration = "IDS_CustomerRegistration";
        public const string IDS_SMSRegistration = "IDS_SMSRegistration";
        public const string IDS_CardBox_EjectPAN = "IDS_CardBox_EjectPAN";
    }
    public static class VTMUtility
    {
        /// <summary>
        /// Mask PAN
        /// </summary>
        /// <param name="pan">PAN value</param>
        /// <param name="firstPart">first digits will NOT be masked</param>
        /// <param name="lastPart">last digits will NOT be masked</param>
        /// <returns></returns>
        public static string MaskCardNo(string pan, string seperator = "-", int firstPart = 6, int lastPart = 4)
        {
            string maskedPAN = string.Empty;
            if (string.IsNullOrWhiteSpace(pan))
            {
                maskedPAN = string.Empty;
            }
            else
            {
                if (firstPart <= 0 || lastPart <= 0 || (pan.Length <= firstPart + lastPart))
                {
                    // Mask all digits in PAN
                    maskedPAN = new String('*', pan.Length);
                }
                else
                {
                    maskedPAN = string.Format("{0}{1}{2}{3}{4}",
                                pan.Substring(0, firstPart),
                                seperator,
                                new string('X', pan.Length - firstPart - lastPart),
                                seperator,
                                pan.Substring(pan.Length - lastPart));
                }
            }
            return maskedPAN;
        }

        /// <summary>
        /// Mask ID Number, last digits will NOT be masked
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        public static string MaskIDNumber(string idNumber, int showLastDigits = 4)
        {
            string maskedValue = string.Empty;
            if (string.IsNullOrWhiteSpace(idNumber))
            {
                maskedValue = string.Empty;
            }
            else
            {
                if (showLastDigits <= 0 || idNumber.Length <= showLastDigits)
                {
                    // No mask
                    maskedValue = new String('*', idNumber.Length);
                }
                else
                {
                    maskedValue = new string('*', idNumber.Length - showLastDigits) + idNumber.Substring(idNumber.Length - showLastDigits);
                }
            }
            return maskedValue;
        }

        /// <summary>
        /// Convert Vietnamese words into unisigned words
        /// </summary>
        /// <param name="vietnameseString"></param>
        /// <returns></returns>
        public static string ConvertToUnsignString(string vietnameseString)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = vietnameseString.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string ToAgeString(this DateTime dob, out int years, out int months, out int days)
        {
            DateTime today = DateTime.Today;

            months = today.Month - dob.Month;
            years = today.Year - dob.Year;

            if (today.Day < dob.Day)
            {
                months--;
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            days = (today - dob.AddMonths((years * 12) + months)).Days;

            return string.Format("{0} year{1}, {2} month{3} and {4} day{5}",
                                 years, (years == 1) ? string.Empty : "s",
                                 months, (months == 1) ? string.Empty : "s",
                                 days, (days == 1) ? string.Empty : "s");
        }

        /// <summary>
        /// Convert HEX string into ASCII string
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexToASCIIString(string hexString)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(hexString))
            {
                byte[] data = new byte[hexString.Length / 2];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                result = Encoding.ASCII.GetString(data);
            }
            return result;
        }

        public static string GetMaskedTrack1(string track1, out string track1CardName)
        {
            string maskedTrack1 = string.Empty;
            track1CardName = string.Empty;
            if (!string.IsNullOrWhiteSpace(track1))
            {
                try
                {
                    int fsStart = track1.IndexOf('^');
                    int fsEnd = track1.LastIndexOf('^');

                    // "B4665859140051738^TPBANK VISA DEBIT         ^2409201100009999999900983000000"
                    // part1 = "B4665859140051738"
                    // part2 = "TPBANK VISA DEBIT         "
                    // part3 = "2409201100009999999900983000000"

                    string part1 = track1.Substring(0, fsStart);
                    part1 = VTMUtility.MaskCardNo(part1, "", 7, 4); // "B466585XXXXXX1738"

                    // track1 Card Name
                    string part2 = track1.Substring(fsStart + 1, fsEnd - fsStart - 1);
                    if (!string.IsNullOrWhiteSpace(part2))
                    {
                        track1CardName = part2.Trim();
                    }
                    // string part3 = track1.Substring(fsEnd + 1, track1.Length - fsEnd - 1);               

                    maskedTrack1 = part1 + "^" +
                                   part2 + "^" +
                                   new string('*', track1.Length - fsEnd - 1);
                }
                catch
                {
                    maskedTrack1 = new string('*', track1.Length);
                }
            }
            return maskedTrack1;
        }
    }
}
