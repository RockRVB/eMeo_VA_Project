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
using System.Threading;
using ResourceManagerProtocol;
using CardReaderDeviceProtocol;
using EmvServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{A47B6466-3BAC-4B58-B870-075769AFEC7E}",
                  Name = "DisorderCardReadRawDataProj",
                  NodeNameOfConfiguration = "DisorderCardReadRawDataProj",
                  Author = "")]
    public class DisorderCardReadRawDataProj : BusinessActivityVTMBase
    {
        #region constructor
        private DisorderCardReadRawDataProj()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new DisorderCardReadRawDataProj();
        }
        #endregion

        #region methods
        string _pan = string.Empty;
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            VTMContext.NextCondition = EventDictionary.s_EventContinue;
            if (GetPAN())
            {
                Log.Action.LogDebug("_pan:" + _pan);
                VTMContext.TransactionDataCache.Set("core_PAN", _pan, GetType());
            }
            else
            {
                Log.Action.LogInfo("GetPAN Failed");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        #endregion

        #region define function
        public bool getlessCardPan()
        {
            try
            {
                Log.Action.LogError("***getlessCardPan***");
                VTMContext.CRDReader.MoveCard(3);
                GrgCmdReadRawDataOutArg objOutArg;
                DevResult devResult = VTMContext.CRDReader.ReadRawData(out objOutArg);
                VTMContext.CRDReader.MoveCard(4);
                VTMContext.CRDReader.CancelReadRawData();
                if (devResult.IsFailure || objOutArg == null)
                {
                    return false;
                }
                else
                {
                    _pan = objOutArg.StrATR;
                    Log.Action.LogInfo("StrATR:" + _pan);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return false;
        }
        public bool GetPAN()
        {
            if (null != VTMContext.CRDReader)
            {
                try
                {
                    VTMContext.TransactionDataCache.Get("proj_SEM_CardType", out var cardtype, GetType());
                    
                    if ((cardtype?.ToString()).Equals("CONTACTLESS"))
                    {
                        return getlessCardPan();
                    }
                    GrgCmdReadRawDataOutArg objOutArg;
                    DevResult devResult = VTMContext.CRDReader.ReadRawData(out objOutArg);
                    if (devResult.IsFailure || objOutArg == null)
                    {
                        return false;
                    }
                    else
                    {
                        //IC 发卡
                        VTMContext.CardType = VTMContext.CRDReader.GetCardType();
                        Log.Action.LogError("card type" + VTMContext.CardType);
                        VTMContext.CRDReader.CancelReadRawData();
                        //end IC 发卡 
                        if (!SaveRawData(objOutArg.StrTrack1, objOutArg.StrTrack2, objOutArg.StrTrack3))
                        {
                            if (VTMContext.CardType == emCardType.SimplexCard || VTMContext.CardType == emCardType.ComplexCard)
                            {
                                if (GetIcPan())
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(_pan))
                            {
                                return true;
                            }
                            if (VTMContext.CardType == emCardType.SimplexCard || VTMContext.CardType == emCardType.ComplexCard)
                            {
                                if (GetIcPan())
                                {
                                    return true;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Action.LogError(ex.ToString());
                }
                //finally
                //{
                //    VTMContext.CRDReader.CancelReadRawData();
                //}
            }
            return false;
        }
        /// <summary>
        /// 芯片卡处理
        /// </summary>
        /// <returns></returns>
        public bool GetIcPan()
        {
            try
            {
                short shRet = VTMContext.EmvService4CRDReader.EmvBuildAppCandidates();
                if (shRet == ICFuncReturnValueType.Success)
                {
                    short appCount = VTMContext.EmvService4CRDReader.EmvGetAppCount();
                    if (appCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        shRet = VTMContext.EmvService4CRDReader.EmvApplicationSelected(-1);
                        if (shRet == ICFuncReturnValueType.Success)
                        {
                            shRet = VTMContext.EmvService4CRDReader.EmvApplicationInitialised();
                            if (shRet == ICFuncReturnValueType.Success)
                            {
                                shRet = VTMContext.EmvService4CRDReader.EmvApplicationLoaded();
                                if (shRet == ICFuncReturnValueType.Success)
                                {
                                    string track1 = VTMContext.EmvService4CRDReader.EmvGetTag("9F1F");

                                    // get IC track2
                                    string track2 = VTMContext.EmvService4CRDReader.EmvGetTag("57");

                                    if (string.IsNullOrEmpty(track2))
                                    {
                                        // get IC track2
                                        track2 = VTMContext.EmvService4CRDReader.EmvGetTag("5A");
                                    }
                                    if (!string.IsNullOrEmpty(track2))
                                    {
                                        track2 = track2.ToLowerInvariant();
                                        track2 = track2.Replace('d', '=');
                                        track2 = track2.TrimEnd('f');
                                    }
                                    if (SaveRawData(track1, track2, null))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.ToString());
            }
            return false;
        }
        private bool SaveRawData(string track1, string track2, string track3)
        {

            FitItem fitItem = null;

            MatchResult matchResult = m_objContext.SettingOfCardAndPB.CheckCardType(track1, track2, null, out fitItem);

            if (MatchResult.Success == matchResult)
            {
                _pan = fitItem.PAN;
                return true;
            }
            return false;
        }
        #endregion
    }
}
