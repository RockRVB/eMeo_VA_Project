using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using RemoteTellerServiceProtocol;
using ResourceManagerProtocol;
using System;
using VTMBusinessActivityBase;
using VTMModelLibrary;
using VTMModelLibrary.Packmodels;

namespace VTMBusinessActivity
{
    [GrgActivity("{8A206182-CEB8-45C5-ADDD-52F7C6682B63}",
                  Name = "SetIDInfo4Exhibition",
                  NodeNameOfConfiguration = "SetIDInfo4Exhibition",
                  Author = "ltfei1")]
    public class SetIDInfo4Exhibition : BusinessActivityVTMBase
    {
        #region constructor
        private SetIDInfo4Exhibition()
        {

        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new SetIDInfo4Exhibition();
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            var emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }

            SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState);
            object obj;
            m_objContext.TransactionDataCache.Get(DataCacheKey.VTM_IDCARD, out obj, GetType());
            var idCardInfo = obj as ScanIDCardInfo;
            if (idCardInfo != null)
            {
                try
                {
                    string exp = "20010101";//身份证有效期

                    //组身份证信息包 
                    var identi = new IdentityReq
                    {
                        CusData = new IdentityInfo
                        {
                            name = idCardInfo.IdCard_Name,
                            address = idCardInfo.IdCard_Address,
                            birth = idCardInfo.IdCard_Birthday,
                            ethnic = idCardInfo.IdCard_Nation,
                            idno = idCardInfo.IdCard_IDNo,
                            issue = idCardInfo.IdCard_IDOrg,
                            sex = idCardInfo.IdCard_Sex,
                            startdate = exp,
                            dataType = ((int)UploadedDataType.ID).ToString()
                        }
                    };
                    VTMContext.TransactionDataCache.Set(DataCacheKey.VTM_VTA_BODY, identi, GetType());

                    VTMContext.NextCondition = EventDictionary.s_EventContinue;
                }
                catch (WorkflowIllegalException ex)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    throw ex;
                }
                catch (WorkflowTerminateException ex)
                {
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.Action.LogError("identityData is fail", ex);
                    VTMContext.NextCondition = EventDictionary.s_EventFail;
                }
            }
            else
            {
                SwitchUIState(m_objContext.MainUI, "IdentityDataIsNull", 3000); //add by lmjun2 20171213 没有扫描到身份证信息给出device error界面提示   
                System.Threading.Thread.Sleep(3000);
                Log.Action.LogError("identityData is null");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }

        #endregion
    }
}