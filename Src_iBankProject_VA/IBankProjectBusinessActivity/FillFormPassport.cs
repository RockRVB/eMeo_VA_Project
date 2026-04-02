using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIServiceProtocol;
using VTMBusinessActivityBase;
using VTMModelLibrary;
using VTMModelLibrary.Packmodels;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{ABF5EBF6-3BD3-4362-B068-483B129CCD19}",
                  Name = "FillFormPassport",
                  NodeNameOfConfiguration = "FillFormPassport",
                  Author = "")]
    public class FillFormPassport : BusinessActivityVTMBase
    {
        #region constructor
        private FillFormPassport()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new FillFormPassport();
        }
        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            try
            {
                var result = base.InnerRun(argContext);
                if (emBusActivityResult_t.Success != result)
                {
                    Log.Action.LogError("Failed to run base's implement");
                    return result;
                }

                Dictionary<string, string> passportFormInfo = new Dictionary<string, string>(){
                    { "name",string.Empty},
                    { "gender",string.Empty},
                    { "birthDate",string.Empty},
                    { "nation",string.Empty},
                    { "passportNo",string.Empty},
                    { "expire",string.Empty},
                    { "phoneNum",string.Empty},
                    { "email",string.Empty},
                    { "address",string.Empty}
                };

                PassportDetailInfo passportInfo = new PassportDetailInfo();

                object objInfo = null;
                VTMContext.TransactionDataCache.Get(DataCacheKey.VTM_PASSPORT, out objInfo, GetType());

                if (objInfo != null)
                {
                    //Log.Action.LogDebugFormat("objInfo is Dictionary<string, string>: {0}", objInfo is Dictionary<string, string>);
                    //Log.Action.LogDebugFormat("objInfo is PassportDetailInfo: {0}", objInfo is PassportDetailInfo);

                    if (objInfo is PassportDetailInfo)
                    {
                        passportInfo = objInfo as PassportDetailInfo;
                    }
                    else
                    {
                        passportInfo = JsonConvert.DeserializeObject<PassportDetailInfo>(objInfo.ToString());
                    }

                    if (passportInfo == null)
                    {
                        passportInfo = new PassportDetailInfo();
                    }
                    passportFormInfo["name"] = passportInfo.MRZname;
                    passportFormInfo["gender"] = passportInfo.MRZsex;
                    passportFormInfo["birthDate"] = passportInfo.MRZbirth;
                    passportFormInfo["nation"] = passportInfo.MRZnationality;
                    passportFormInfo["passportNo"] = passportInfo.MRZdocumentID;
                    passportFormInfo["expire"] = passportInfo.MRZenddate;
                    passportFormInfo["phoneNum"] = passportInfo.PhoneNumber;
                    passportFormInfo["email"] = passportInfo.Email;
                    passportFormInfo["address"] = passportInfo.Address;
                }

                FormData = JsonConvert.SerializeObject(passportFormInfo);


                //Log.Action.LogDebug("InitFormData: " + FormData);

                SwitchUIState(m_objContext.MainUI, DataDictionary.s_DefaultUIState);

                emWaitSignalResult_t signalResult = WaitSignal();
                if(signalResult == emWaitSignalResult_t.Timeout)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventTimeout;
                }

                passportFormInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(FormData);
                if (passportFormInfo != null)
                {
                    passportInfo.MRZname = passportFormInfo["name"] ?? string.Empty;
                    passportInfo.MRZsex = passportFormInfo["gender"] ?? string.Empty;
                    passportInfo.MRZbirth = passportFormInfo["birthDate"] ?? string.Empty;
                    passportInfo.MRZnationality = passportFormInfo["nation"] ?? string.Empty;
                    passportInfo.MRZdocumentID = passportFormInfo["passportNo"] ?? string.Empty;
                    passportInfo.MRZenddate = passportFormInfo["expire"] ?? string.Empty;
                    passportInfo.PhoneNumber = passportFormInfo["phoneNum"] ?? string.Empty;
                    passportInfo.Email = passportFormInfo["email"] ?? string.Empty;
                    passportInfo.Address = passportFormInfo["address"] ?? string.Empty;

                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_PASSPORT, passportInfo, GetType());//保存解析后的护照数据
                }
                else
                {
                    Log.Action.LogFatal("FormData format fail: " + FormData);
                }

                VTMContext.TransactionDataCache.Set("CardHolderName", passportFormInfo?["name"], GetType());
                VTMContext.TransactionDataCache.Set("PassportFormInfo", passportFormInfo, GetType());

                Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            }
            catch(Exception ex)
            {
                Log.Action.LogDebugFormat("FillFormPassport Action Exception : {0}", ex.Message);
            }
            return emBusActivityResult_t.Success;
        }

        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg objArg)
        {
            emBusiCallbackResult_t result = base.InnerOnUIEvtHandle(iUI, objArg);
            if (emBusiCallbackResult_t.Bypass != result)
            {
                return result;
            }
            m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);

            if (objArg.EventName.Equals(UIEventNames.s_ClickEvent, StringComparison.OrdinalIgnoreCase))
            {
                m_objContext.NextCondition = objArg.Key.ToString();
                SignalCancel();
            }

            return emBusiCallbackResult_t.Swallowd;
        }

        private string m_formData = string.Empty;
        [GrgBindTarget("FormData", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string FormData
        {
            get
            {
                return m_formData;
            }
            set
            {
                m_formData = value;
                OnPropertyChanged("FormData");
            }
        }

        private string m_initFormData = string.Empty;
        [GrgBindTarget("InitFormData", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string InitFormData
        {
            get
            {
                return m_initFormData;
            }
            set
            {
                m_initFormData = value;
                OnPropertyChanged("InitFormData");
            }
        }

    }
}
