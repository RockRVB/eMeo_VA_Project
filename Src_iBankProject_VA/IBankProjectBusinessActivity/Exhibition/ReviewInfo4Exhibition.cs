using System;
using System.Collections.Generic;
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
using IBankProjectBusinessServiceImp;

namespace VTMBusinessActivity
{
    [GrgActivity("{04BB383A-F6B8-4568-89CF-389112C016A0}",
             NodeNameOfConfiguration = "ReviewInfo4Exhibition",
             Name = "ReviewInfo4Exhibition",
             Author = "")]
    public class ReviewInfo4Exhibition : BusinessActivityVTMBase
    {
        #region properties
        private ScanIDCardInfo m_SIDInfo = new ScanIDCardInfo();
        public ScanIDCardInfo SIDInfo
        {
            get { return m_SIDInfo; }
            set
            {
                m_SIDInfo = value;
                OnPropertyChanged("SIDInfo");
            }
        }

        private CreadCardInfo m_ccInfo = new CreadCardInfo();
        public CreadCardInfo ccInfo
        {
            get { return m_ccInfo; }
            set
            {
                m_ccInfo = value;
                OnPropertyChanged("ccInfo");
            }
        }

        private string m_IDName = "";
        [GrgBindTarget("IDName", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDName
        {
            get { return m_IDName; }
            set
            {
                m_IDName = value;
                SIDInfo.IdCard_Name = ccInfo.name = m_IDName;
                OnPropertyChanged("IDName");
            }
        }

        private string m_IDGender = "M";
        [GrgBindTarget("IDGender", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDGender
        {
            get { return m_IDGender; }
            set
            {
                m_IDGender = value;
                SIDInfo.IdCard_Sex = ccInfo.sex = m_IDGender;
                OnPropertyChanged("IDGender");
            }
        }

        private bool m_Male = true;
        [GrgBindTarget("Male", Type = TargetType.Bool, Access = AccessRight.ReadAndWrite)]
        public bool Male
        {
            get { return m_Male; }
            set
            {
                m_Male = value;
                if (m_Male)
                {
                    IDGender = "M";
                }
                OnPropertyChanged("Male");
            }
        }

        private bool m_Female = false;
        [GrgBindTarget("Female", Type = TargetType.Bool, Access = AccessRight.ReadAndWrite)]
        public bool Female
        {
            get { return m_Female; }
            set
            {
                m_Female = value;
                if (m_Female)
                {
                    IDGender = "F";
                }
                OnPropertyChanged("Female");
            }
        }

        private string m_IDBirth = "";
        [GrgBindTarget("IDBirth", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDBirth
        {
            get { return m_IDBirth; }
            set
            {
                m_IDBirth = value;
                SIDInfo.IdCard_Birthday = ccInfo.birth = m_IDBirth;
                OnPropertyChanged("IDBirth");
            }
        }

        private string m_IDPhone = "";
        [GrgBindTarget("IDPhone", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDPhone
        {
            get { return m_IDPhone; }
            set
            {
                m_IDPhone = value;
                ccInfo.mobile = m_IDPhone;
                OnPropertyChanged("IDPhone");
            }
        }

        private string m_IDNum = "";
        [GrgBindTarget("IDNum", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDNum
        {
            get { return m_IDNum; }
            set
            {
                m_IDNum = value;
                SIDInfo.IdCard_IDNo = ccInfo.idcard = m_IDNum;
                OnPropertyChanged("IDNum");
            }
        }

        private string m_IDVipNum = "";
        [GrgBindTarget("IDVipNum", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string IDVipNum
        {
            get { return m_IDVipNum; }
            set
            {
                m_IDVipNum = value;
                ccInfo.vipno = m_IDVipNum;
                OnPropertyChanged("IDVipNum");
            }
        }

        #endregion

        #region create
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new ReviewInfo4Exhibition() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected ReviewInfo4Exhibition()
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
                    if (key.Equals(EventDictionary.s_EventConfirm) || key.Equals(EventDictionary.s_EventContinue))
                    {
                        m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
                        object isIDScanFlag = null;
                        m_objContext.TransactionDataCache.Get("isIDScanFlag", out isIDScanFlag, GetType());
                        if (null == isIDScanFlag || (null != isIDScanFlag && !isIDScanFlag.ToString().Equals("True")))
                        {
                            //ScanIDCardInfo idInfo = new ScanIDCardInfo();
                            SIDInfo.IdCard_Address = "GRGBANKING, Kelin 9, Guangzhou.";
                            SIDInfo.IdCard_BeginDate = "20080101";
                            SIDInfo.IdCard_Birthday = IDBirth;
                            SIDInfo.IdCard_Country = "China";
                            SIDInfo.IdCard_Day = "30";
                            SIDInfo.IdCard_EndDate = "20991231";
                            if (!string.IsNullOrWhiteSpace(IDNum))
                            {
                                SIDInfo.IdCard_IDNo = IDNum;
                            }
                            else
                            {
                                SIDInfo.IdCard_IDNo = "";
                            }
                            SIDInfo.IdCard_IDOrg = "Guangzhou";
                            SIDInfo.IdCard_Month = "12";
                            SIDInfo.IdCard_Name = IDName;
                            SIDInfo.IdCard_Nation = "Chinese";
                            SIDInfo.IdCard_Sex = IDGender;
                            SIDInfo.IdCard_Year = "2019";
                            m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_IDCARD, SIDInfo, GetType());
                        }
                        m_objContext.TransactionDataCache.Set(DataCacheKey.VTM_IDCARD, SIDInfo, GetType());
                        m_objContext.TransactionDataCache.Set("model", ccInfo, GetType());

                    }
                    m_objContext.NextCondition = key;
                    SignalCancel();
                }
            }

            return emBusiCallbackResult_t.Swallowd;
        }

        private void SetLoadUIData()
        {
            try
            {
                object isIDScanFlag = null;
                m_objContext.TransactionDataCache.Get("isIDScanFlag", out isIDScanFlag, GetType());
                if (null != isIDScanFlag && isIDScanFlag.ToString().Equals("True"))
                {
                    object idInfo = null;
                    m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out idInfo, GetType());
                    {
                        if (null != idInfo)
                        {
                            SIDInfo = idInfo as ScanIDCardInfo;
                            IDBirth = SIDInfo.IdCard_Birthday;
                            if (SIDInfo.IdCard_Sex.Equals("男"))
                            {
                                IDGender = "M";
                            }
                            else if (SIDInfo.IdCard_Sex.Equals("女"))
                            {
                                IDGender = "F";
                            }
                            else
                            {
                                IDGender = SIDInfo.IdCard_Sex;
                            }

                            if (!string.IsNullOrWhiteSpace(IDGender))
                            {
                                if (IDGender.Substring(0, 1).ToUpper().Equals("M"))
                                {
                                    Male = true;
                                    Female = false;
                                }
                                else
                                {
                                    Male = false;
                                    Female = true;
                                }
                            }
                            IDName = SIDInfo.IdCard_Name;
                            if (!string.IsNullOrWhiteSpace(SIDInfo.IdCard_IDNo))
                            {
                                IDNum = SIDInfo.IdCard_IDNo;
                            }
                            else
                            {
                                object obj1 = null;
                                m_objContext.TransactionDataCache.Get("ExhibitionIDNum", out obj1, GetType());
                                if (null != obj1)
                                {
                                    IDNum = obj1.ToString();
                                }
                                else
                                {
                                    IDNum = "";
                                }
                            }
                            IDPhone = "";
                            IDVipNum = "";
                        }
                    }
                }
                else
                {
                    object obj = null;
                    m_objContext.TransactionDataCache.Get("model", out obj, GetType());
                    if (null != obj)
                    {
                        m_ccInfo = (CreadCardInfo)obj;
                    }
                    IDBirth = ccInfo.birth;
                    IDGender = ccInfo.sex;
                    if (!string.IsNullOrWhiteSpace(IDGender))
                    {
                        if (IDGender.Substring(0, 1).ToUpper().Equals("M"))
                        {
                            Male = true;
                            Female = false;
                        }
                        else
                        {
                            Male = false;
                            Female = true;
                        }
                    }
                    IDName = ccInfo.name;
                    if (!string.IsNullOrWhiteSpace(ccInfo.idcard))
                    {
                        IDNum = ccInfo.idcard;
                    }
                    else
                    {
                        object obj1 = null;
                        m_objContext.TransactionDataCache.Get("ExhibitionIDNum", out obj1, GetType());
                        if (null != obj1)
                        {
                            IDNum = obj1.ToString();
                        }
                        else
                        {
                            IDNum = "";
                        }
                    }
                    IDPhone = ccInfo.mobile;
                    IDVipNum = ccInfo.vipno;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message, ex);
            }
        }


        #endregion
    }
}
