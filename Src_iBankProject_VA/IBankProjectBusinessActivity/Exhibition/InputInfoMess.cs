using System;
using System.Collections.Generic;
using System.Linq;
using LogProcessorService;
using UIServiceProtocol;
using BusinessServiceProtocol;
using Attribute4ECAT;
using eCATBusinessServiceProtocol;
using VTMModelLibrary;
using VTMBusinessActivityBase;
using VTMHelperService.common;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System.Text.RegularExpressions;
using DataFormatJSON;
using RemoteServiceModel;
using VTMBusinessServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{A937DEDE-5CC7-4912-9DBA-95F472AACDC6}",
             NodeNameOfConfiguration = "InputInfoMess",
             Name = "InputInfoMess",
             Author = "ltfei1")]
    public class InputInfoMess : BusinessActivityVTMBase
    {
        #region properties
        private CreadCardInputInfo m_FormInfo = new CreadCardInputInfo();
        public CreadCardInputInfo FormInfo
        {
            get { return m_FormInfo; }
            set
            {
                m_FormInfo = value;
                OnPropertyChanged("FormInfo");
            }
        }

        [GrgBindTarget("IdCard_Name", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_Name
        {
            get
            {
                return m_FormInfo.IdCard_Name;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_Name, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_Name = value;
                    OnPropertyChanged("IdCard_Name");
                }
            }
        }

        [GrgBindTarget("IdCard_Sex", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_Sex
        {
            get
            {
                return m_FormInfo.IdCard_Sex;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_Sex, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_Sex = value;
                    OnPropertyChanged("IdCard_Sex");
                }
            }
        }

        [GrgBindTarget("IdCard_Nation", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_Nation
        {
            get
            {
                return m_FormInfo.IdCard_Nation;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_Nation, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_Nation = value;
                    OnPropertyChanged("IdCard_Nation");
                }
            }
        }

        [GrgBindTarget("IdCard_Birthday", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_Birthday
        {
            get
            {
                return m_FormInfo.IdCard_Birthday;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_Birthday, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_Birthday = value;
                    OnPropertyChanged("IdCard_Birthday");
                }
            }
        }

        [GrgBindTarget("IdCard_Address", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_Address
        {
            get
            {
                return m_FormInfo.IdCard_Address;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_Address, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_Address = value;
                    OnPropertyChanged("IdCard_Address");
                }
            }
        }

        [GrgBindTarget("IdCard_IDNo", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_IDNo
        {
            get
            {
                return m_FormInfo.IdCard_IDNo;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_IDNo, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_IDNo = value;
                    OnPropertyChanged("IdCard_IDNo");
                }
            }
        }

        [GrgBindTarget("IdCard_IDOrg", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_IDOrg
        {
            get
            {
                return m_FormInfo.IdCard_IDOrg;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_IDOrg, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_IDOrg = value;
                    OnPropertyChanged("IdCard_IDOrg");
                }
            }
        }

        [GrgBindTarget("IdCard_ValidateDate", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IdCard_ValidateDate
        {
            get
            {
                return m_FormInfo.IdCard_ValidateDate;
            }
            set
            {
                if (!string.Equals(m_FormInfo.IdCard_ValidateDate, value, StringComparison.Ordinal))
                {
                    m_FormInfo.IdCard_ValidateDate = value;
                    OnPropertyChanged("IdCard_ValidateDate");
                }
            }
        }

        [GrgBindTarget("PhoneNo", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string PhoneNo
        {
            get
            {
                return m_FormInfo.PhoneNo;
            }
            set
            {
                if (!string.Equals(m_FormInfo.PhoneNo, value, StringComparison.Ordinal))
                {
                    m_FormInfo.PhoneNo = value;
                    OnPropertyChanged("PhoneNo");
                }
            }
        }

        [GrgBindTarget("FamilyNo", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string FamilyNo
        {
            get
            {
                return m_FormInfo.FamilyNo;
            }
            set
            {
                if (!string.Equals(m_FormInfo.FamilyNo, value, StringComparison.Ordinal))
                {
                    m_FormInfo.FamilyNo = value;
                    OnPropertyChanged("FamilyNo");
                }
            }
        }

        [GrgBindTarget("CustomAddress", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string CustomAddress
        {
            get
            {
                return m_FormInfo.CustomAddress;
            }
            set
            {
                if (!string.Equals(m_FormInfo.CustomAddress, value, StringComparison.Ordinal))
                {
                    m_FormInfo.CustomAddress = value;
                    OnPropertyChanged("CustomAddress");
                }
            }
        }

        [GrgBindTarget("Career", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string Career
        {
            get
            {
                return m_FormInfo.Career;
            }
            set
            {
                m_FormInfo.Career = value;
                //根据索引，拿到值
                object objCareerCollection = null;
                m_objContext.TransactionDataCache.Get("CareerCollection", out objCareerCollection, GetType());
                if (objCareerCollection != null && objCareerCollection is UIListItemCollection)
                {
                    var careerCollection = objCareerCollection as UIListItemCollection;
                    m_FormInfo.CareerText = careerCollection.Items.FirstOrDefault(a => a.GetBindData("Key").ToString() == value).GetBindData("Value").ToString();
                }
                OnPropertyChanged("Career");
            }
        }

        [GrgBindTarget("EmailAddress", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string EmailAddress
        {
            get
            {
                return m_FormInfo.EmailAddress;
            }
            set
            {
                if (!string.Equals(m_FormInfo.EmailAddress, value, StringComparison.Ordinal))
                {
                    m_FormInfo.EmailAddress = value;
                    OnPropertyChanged("EmailAddress");
                }
            }
        }

        [GrgBindTarget("VTM_SelectedOldCardNum", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string OldCardNo
        {
            get
            {
                return m_FormInfo.OldCardNo;
            }
        }

        #endregion
        //add by lmjun2 20171222 区分身份证或护照认证的表单
        private string _authorType = string.Empty;
        [GrgBindTarget("authorType", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string AuthType
        {
            get
            {
                return _authorType;
            }
            set
            {
                _authorType = value;
                OnPropertyChanged("authorType");
            }
        }
        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new InputInfoMess() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected InputInfoMess()
        {
        }
        #endregion

        #region method
        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);
            if (emBusActivityResult_t.Success != result)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            SetLoadUIData();
            //SetLoadCareerUIData();


            SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);
            emWaitSignalResult_t waitResult;
            if (WaitPopu == 1)
            {
                waitResult = VTMWaitSignal();
            }
            else
            {
                waitResult = WaitSignal();
            }

            if (waitResult == emWaitSignalResult_t.Timeout)
            {
                m_objContext.NextCondition = EventDictionary.s_EventTimeout;
            }

            setccinfo();

            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                if (objArg.Key is string)
                {
                    string key = (string)objArg.Key;
                    if (key.Equals(EventDictionary.s_EventConfirm, StringComparison.OrdinalIgnoreCase))
                    {
                        m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);

                        bool isCk = true;
                        if (!Regex.IsMatch(FormInfo.PhoneNo.Replace(" ", ""), @"^[1][3-8]\d{9}$", RegexOptions.IgnoreCase))
                        {
                            FormInfo.MsgPhoneError = m_objContext.CurrentUIResource.LoadString("IDS_MsgPhoneError", TextCategory.s_UI);
                            isCk = false;
                        }
                        if (!String.IsNullOrEmpty(FormInfo.FamilyNo) && !Regex.IsMatch(FormInfo.FamilyNo.Replace(" ", ""), @"\b0\d{2}\d{8}|\b0\d{2}\d{7}|\b0\d{3}\d{7}|\b0\d{3}\d{8}", RegexOptions.IgnoreCase))
                        {
                            FormInfo.MsgTelephoneError = m_objContext.CurrentUIResource.LoadString("IDS_MsgPhoneError", TextCategory.s_UI);
                            isCk = false;
                        }
                        if (string.IsNullOrEmpty(FormInfo.CustomAddress))
                        {
                            FormInfo.MsgCusAddressError = m_objContext.CurrentUIResource.LoadString("IDS_MsgCusAddressError", TextCategory.s_UI);
                            isCk = false;
                        }
                        if (string.IsNullOrEmpty(FormInfo.IdCard_Name) || string.IsNullOrEmpty(FormInfo.IdCard_Sex) || string.IsNullOrEmpty(FormInfo.IdCard_Nation)
                            || string.IsNullOrEmpty(FormInfo.IdCard_Address) || string.IsNullOrEmpty(FormInfo.IdCard_IDNo) || string.IsNullOrEmpty(FormInfo.IdCard_IDOrg))
                        {
                            isCk = false;
                        }

                        string regValidDate = @"(([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3})-(((0[13578]|1[02])-(0[1-9]|[12][0-9]|3[01]))|((0[469]|11)-(0[1-9]|[12][0-9]|30))|(02-(0[1-9]|[1][0-9]|2[0-8]))))|((([0-9]{2})(0[48]|[2468][048]|[13579][26])|((0[48]|[2468][048]|[3579][26])00))-02-29)";

                        try
                        {
                            if (!string.IsNullOrWhiteSpace(FormInfo.IdCard_Birthday) && FormInfo.IdCard_Birthday.Length == 8)
                            {
                                string tmpBirthday = FormInfo.IdCard_Birthday.Substring(0, 4) + "-" + FormInfo.IdCard_Birthday.Substring(4, 2) + "-" + FormInfo.IdCard_Birthday.Substring(6, 2);

                                if (!Regex.IsMatch(tmpBirthday, regValidDate, RegexOptions.IgnoreCase))
                                {
                                    isCk = false;
                                }
                            }
                            else
                            {
                                isCk = false;
                            }

                            if (!string.IsNullOrWhiteSpace(FormInfo.IdCard_ValidateDate) && FormInfo.IdCard_ValidateDate.Length > 7)
                            {
                                string tmpValiteDate = FormInfo.IdCard_ValidateDate.Substring(0, 8);

                                tmpValiteDate = FormInfo.IdCard_ValidateDate.Substring(0, 4) + "-" + FormInfo.IdCard_ValidateDate.Substring(4, 2) + "-" + FormInfo.IdCard_ValidateDate.Substring(6, 2);

                                if (!Regex.IsMatch(tmpValiteDate, regValidDate, RegexOptions.IgnoreCase))
                                {
                                    isCk = false;
                                }
                            }
                            else
                            {
                                isCk = false;
                            }

                        }
                        catch
                        {
                            isCk = false;
                        }
                        //if (string.IsNullOrEmpty(FormInfo.TelephoneNum))
                        //{
                        //    FormInfo.MsgTelephoneError = m_objContext.CurrentUIResource.LoadString("IDS_MsgTelephoneNumError", TextCategory.s_UI);
                        //    isCk = false;
                        //}

                        //if (!Regex.IsMatch(FormInfo.FamilyNo.Replace(" ", ""), @"\b0\d{2}\d{8}|\b0\d{2}\d{7}|\b0\d{3}\d{7}|\b0\d{3}\d{8}", RegexOptions.IgnoreCase))
                        //{
                        //    FormInfo.MsgTelephoneError = m_objContext.CurrentUIResource.LoadString("IDS_MsgTelephoneNumRegError");
                        //    isCk = false;
                        //}

                        if (isCk)
                        {
                            if (m_objContext.GeneralConfig.IsCSharpSTMA == 1)
                            {
                                object obj = null;
                                ScanIDCardInfo currentIDCard = new ScanIDCardInfo();
                                m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out obj, GetType());

                                if (obj != null && obj is ScanIDCardInfo)
                                {
                                    currentIDCard = (ScanIDCardInfo)obj;
                                }

                                obj = null;

                                m_objContext.TransactionDataCache.Get(DataCacheKey.s_coreTakePhotoPath, out obj, GetType());

                                JSONFormater formater = new JSONFormater();
                                CommandArg arg = new CommandArg()
                                {
                                    CommandParams = new Object[] { "OnAssistConfirm", FormInfo.IdCard_PhotoPath, FormInfo.IdCard_Name, FormInfo.IdCard_Sex, FormInfo.IdCard_Nation, FormInfo.IdCard_EndDate, FormInfo.IdCard_Address, FormInfo.IdCard_Birthday, FormInfo.PhoneNo, FormInfo.TelephoneNum, FormInfo.CustomAddress, currentIDCard.IdCard_ScanImg1, currentIDCard.IdCard_ScanImg2, obj == null ? string.Empty : obj.ToString() }    //类似处理，生成
                                };
                                m_objContext.TransactionDataCache.Set("proj_OnProAssistConfirm", formater.FormatCommandArg(arg), GetType());
                                m_objContext.ECATPlatform.PostSystemEvent(this, new SystemEventArg(this, "OnProAssistConfirm"));
                            }

                            m_objContext.NextCondition = key;
                            FormInfo.AuthorType = AuthType;//add by lmjun2 20171222 区分身份证或护照认证的表单
                            m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_CARDFORM, FormInfo, GetType());
                            SignalCancel();
                            return emBusiCallbackResult_t.Swallowd;
                        }
                        else
                        {
                            return emBusiCallbackResult_t.Swallowd;
                        }
                    }
                    else
                    {
                        if (key.Equals("OnHelp", StringComparison.OrdinalIgnoreCase))
                        {
                            m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                            m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_CARDFORM, FormInfo, GetType());
                        }
                        m_objContext.NextCondition = key;
                        SignalCancel();
                    }
                }
            }
            return emBusiCallbackResult_t.Swallowd;
        }

        private void SetLoadUIData()
        {
            try
            {
                CreadCardInfo ccinfo = new CreadCardInfo();
                object obj = null;
                m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_CARDFORM, out obj, GetType());
                if (null != obj)
                {
                    m_FormInfo = (CreadCardInputInfo)obj;
                }
                else
                {
                    if (FormInfo == null)
                    {
                        FormInfo = new CreadCardInputInfo();
                    }
                    object cardobj = null;
                    m_objContext.TransactionDataCache.Get(VTMDataDictionary.VtmSelectedOldCardNum, out cardobj, GetType());
                    if (cardobj != null)
                    {
                        FormInfo.OldCardNo = cardobj.ToString();
                    }


                    string dateformat = m_objContext.CurrentUIResource.LoadString("IDS_DateFormater", TextCategory.s_UI);
                    string enddtime = "20300101";

                    DateTime begindtime = DateTime.ParseExact("20100101", "yyyyMMdd", null);

                    FormInfo.IdCard_ValidateDate = string.Concat(begindtime.ToString(dateformat), m_objContext.CurrentUIResource.LoadString("IDS_DateConcat", TextCategory.s_UI), enddtime);
                    FormInfo.IdCard_Birthday = "19800101";
                    FormInfo.IdCard_EndDate = "20300707";
                    FormInfo.IdCard_BeginDate = "20100101";
                    FormInfo.IdCard_IDNo = "E3177567909";
                    FormInfo.IdCard_IDOrg = "Demo";
                    FormInfo.IdCard_Name = "Eric";
                    FormInfo.IdCard_Nation = "CH";
                    FormInfo.IdCard_PhotoPath = string.Empty;
                    FormInfo.IdCard_Sex = "F";
                    FormInfo.IdCard_Address = "GUANGZHOU GUANDDONG CHINA";
                    FormInfo.CustomAddress = "GUANGZHOU GUANDDONG CHINA";
                    FormInfo.CardNamePin = "Eric";// PinyinHelper.GetPinyin(m_FormInfo.IdCard_Name);
                    FormInfo.PhoneNo = "13309990987";
                    FormInfo.MsgCusAddressError = string.Empty;
                    FormInfo.MsgPhoneError = string.Empty;
                    FormInfo.MsgTelephoneError = string.Empty;

                }
            }
            catch (NullReferenceException ex)
            {
                Log.Action.LogError("NULL ReferenceException InputInfoEx", ex);
            }
            catch (Exception ex)
            {
                Log.Action.LogError("error InputInfoEx " + ex.Message.ToString());
            }
        }

        private void setccinfo()
        {
            if (FormInfo != null)
            {
                CreadCardInfo ccInfo = new CreadCardInfo();
                ccInfo.name = FormInfo.IdCard_Name;
                ccInfo.sex = FormInfo.IdCard_Sex;
                ccInfo.birth = FormInfo.IdCard_Birthday;
                ccInfo.mobile = FormInfo.PhoneNo;
                ccInfo.idcard = FormInfo.IdCard_IDNo;
                ccInfo.vipno = string.Empty;
                m_objContext.TransactionDataCache.Set("model", ccInfo, GetType());
            }
            else
            {
                Log.Action.LogError("FormInfo is null");
            }


        }
        #endregion
    }
}
