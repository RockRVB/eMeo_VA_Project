using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using System;
using VTMBusinessActivityBase;
using VTMModelLibrary;
using VTMModelLibrary.Packmodels;

namespace VTMBusinessActivity
{
    [GrgActivity("{C18CDD11-0F17-4C29-9B7E-4381233891F1}",
                  Name = "SetFormData",
                  NodeNameOfConfiguration = "SetFormData",
                  Author = "Tommy")]
    public class SetFormData : BusinessActivityVTMBase
    {
        #region constructor
        private SetFormData()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new SetFormData();
        }
        #endregion

        private string _type = string.Empty;
        [GrgBindTarget("type", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string FormType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                OnPropertyChanged("type");
            }
        }

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            object obj;
            m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_CARDFORM, out obj, GetType());
            var creadCardInputInfo = obj as CreadCardInputInfo;
            if (null != creadCardInputInfo)
            {
                try
                {
                    //组表单信息包 
                    var formReq = new FormReq
                    {
                        busiData = new BussinessFormForDebitCard
                        {
                            IdCard_ValidateDate = creadCardInputInfo.IdCard_ValidateDate,
                            IdCard_Address = creadCardInputInfo.IdCard_Address,
                            TelephoneNum = creadCardInputInfo.TelephoneNum,
                            PhoneNo = creadCardInputInfo.PhoneNo,
                            CustomAddress = creadCardInputInfo.CustomAddress,
                            IdCard_IDOrg = creadCardInputInfo.IdCard_IDOrg,
                            IdCard_PhotoPath = creadCardInputInfo.IdCard_PhotoPath,
                            IdCard_Name = creadCardInputInfo.IdCard_Name,
                            IdCard_Sex = creadCardInputInfo.IdCard_Sex,
                            IdCard_Nation = creadCardInputInfo.IdCard_Nation,
                            IdCard_Birthday = creadCardInputInfo.IdCard_Birthday,
                            IdCard_IDNo = creadCardInputInfo.IdCard_IDNo,
                            EmailAddress = creadCardInputInfo.EmailAddress,
                            //IdCard_EndDate = creadCardInputInfo.IdCard_EndDate,
                            //IdCard_BeginDate = creadCardInputInfo.IdCard_BeginDate,
                            Career = creadCardInputInfo.CareerText,
                            familyNo = creadCardInputInfo.FamilyNo,
                            dataType = FormType,
                            AuthorType=creadCardInputInfo.AuthorType//add by lmjun2 20171222 区分身份证或护照认证的表单
                        }
                    };
                    if(FormType == "ReplaceCard")//换卡表单
                    {
                        ((BussinessFormForDebitCard)formReq.busiData).OldCardNo = creadCardInputInfo.OldCardNo;
                    }
                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_VTA_BODY, formReq, GetType());

                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                }
                catch (WorkflowIllegalException)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    throw;
                }
                catch (WorkflowTerminateException)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Action.LogError("PassportData is fail", ex);
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                }
            }
            else
            {
                Log.Action.LogError("PassportData is null");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}