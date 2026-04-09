using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using eCATBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using eCATBusinessServiceProtocol;
using VTMBusinessServiceProtocol;
using System.Threading;
using System.Collections.ObjectModel;
using IBankProjectBusinessActivityBase;
using VTMBusinessActivityBase;
using RemoteTellerServiceProtocol;
using DevServiceProtocol;
using BusinessActivityBaseImp;
using InputOrSelect;
using UIServiceProtocol;
using EPPKeypadDeviceProtocol;
using System.Web.Script.Serialization;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{2FBABDD7-C033-4C3A-B46A-E4C9F149FA5C}",
                 Name = "VAB_InputWithdrawAmount",
                 NodeNameOfConfiguration = "VAB_InputWithdrawAmount",
                 Author = "rocky")]
    public class VAB_InputWithdrawAmount : BusinessActivityInputWithdrawalAmount
    {
        private bool ResetTimer = false;
        private string m_input_val = string.Empty;
        [GrgBindTarget("input_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string input_val
        {
            get { return m_input_val; }
            set
            {
                m_input_val = value;
                OnPropertyChanged("input_val");
            }
        }
        private string m_output_val = string.Empty;
        [GrgBindTarget("output_val", Type = TargetType.String, Access = AccessRight.ReadAndWrite)]
        public string output_val
        {
            get { return m_output_val; }
            set
            {
                m_output_val = value;
                OnPropertyChanged("output_val");
            }
        }
        #region constructor
        private VAB_InputWithdrawAmount()
        {
        }
        #endregion

        #region create
        [GrgCreateFunction("create")]
        public static IBusinessActivity Create()
        {
            return new VAB_InputWithdrawAmount();
        }
        #endregion

        public string str_amountcwd = "";

        public struct CassetteInfo
        {
            public string IDHopper;
            public int Denomination;
            public int Remain;
            public int Out;
            public void InputInfo(string ID, int denomination, int remain)
            {
                IDHopper = ID;
                Denomination = denomination;
                Remain = remain;
                Out = 0;
            }
        }
        public override void HandleSideKey(string argEvent, string argKey)
        {
            if (argKey == "OnConfirmWithdraw")
            {
                //解决页面值回传给后端的问题
                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            }

            base.HandleSideKey(argEvent, argKey);

        }

        public override bool HandlePinPadKey(string argKey)
        {
            if ("ENTER" == argKey)
            {
                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);
            }
            return base.HandlePinPadKey(argKey);
        }
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Project.LogDebug("Enter action: VAB_InputWithdrawAmount");

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emBusActivityResult_t.Success != emRet)
            {
                m_objContext.LogJournal("Execute base inner run fail!", LogSymbol.Alert);
                Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount");
                return emRet;
            }
            try
            {
                if (m_objContext.NextCondition == "OnExit" || m_objContext.NextCondition == "OnTimeout")
                {
                    return emBusActivityResult_t.Success;
                }
                string AMOUNTCWD = "";
                int denoMin = 20000;
                int denoMax = 5000000;
                long totalremain = 0;
                int Amount = 0;
                List<CashUnitInfo> listCashUnitInfo = GetCashUnitInfo(2);
                if (listCashUnitInfo.Count == 0)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventCancel;
                    Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount");
                    return emBusActivityResult_t.Success;
                }
                else
                {
                    foreach (CashUnitInfo item in listCashUnitInfo)
                    {
                        if (item.DenoValue == 500000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;
                            continue;
                        }
                        if (item.DenoValue == 200000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;
                            continue;
                        }
                        if (item.DenoValue == 100000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;
                            continue;
                        }
                        if (item.DenoValue == 50000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;

                            continue;
                        }
                        if (item.DenoValue == 20000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;

                            continue;
                        }
                        if (item.DenoValue == 10000 && (item.Status != CashUnitStatus.EMPTY) && (item.Status != CashUnitStatus.MISSING) && (item.Status != CashUnitStatus.INOP))
                        {
                            totalremain = totalremain + item.Count * item.DenoValue;
                            continue;
                        }
                    }
                    Log.Project.LogDebug("total remain = " + totalremain.ToString());
                }
                Log.Project.LogDebug("output_val = " + output_val);
                saveBuffer = output_val;
                if (saveBuffer != null && (saveBuffer.Length > 5))
                {
                    AMOUNTCWD = saveBuffer;
                    Log.Project.LogDebug("CWD AMOUNT = " + AMOUNTCWD);
                    AMOUNTCWD = AMOUNTCWD.Replace(",", "");
                }
                if (AMOUNTCWD.Length < 5)
                {
                    m_objContext.NextCondition = "OnNoChangeState";
                    return emBusActivityResult_t.Success;
                }
                Amount = Int32.Parse(AMOUNTCWD.ToString());
                if (Amount % 10000 > 0)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventContinue;
                    Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount Total % 10000 >0");
                    return emBusActivityResult_t.Success;
                }

                else if (Amount > denoMax || Amount < denoMin || Amount > totalremain)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventContinue;
                    Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount Wrong Amount");
                    return emBusActivityResult_t.Success;
                }

                List<CashUnitInfo> argUintInfo = new List<CashUnitInfo>();
                int tem_remain = 0;
                m_objContext.CashDispenser.GetCashUnitInfo(out argUintInfo);

                int TongSoHocTien = 0;
                int i = 0;
                for (i = 0; i < argUintInfo.Count; i++)
                {
                    if (argUintInfo[i].DenoValue != 0) TongSoHocTien++;
                }
                Log.Project.LogDebug("total cassette = " + TongSoHocTien.ToString());
                CassetteInfo[] cassetteinfo;
                cassetteinfo = new CassetteInfo[TongSoHocTien];
                i = 0;
                foreach (CashUnitInfo item2 in argUintInfo)
                {
                    if (i >= TongSoHocTien) break;
                    else
                    {
                        if (item2.UseType == CashUnitType.BILL || CashUnitType.RECYCLING == item2.UseType)
                        {
                            if (item2.Status == CashUnitStatus.EMPTY || item2.Status == CashUnitStatus.INOP || item2.Status == CashUnitStatus.MISSING)
                            {
                                tem_remain = 0;
                            }
                            else
                                tem_remain = item2.Count;

                            cassetteinfo[i].InputInfo(item2.UnitID, item2.DenoValue, tem_remain);
                            Log.Project.LogDebug("cassette InputInfo = " + item2.UnitID + " " + item2.DenoValue.ToString() + " " + tem_remain.ToString());
                            i++;
                        }

                    }

                }

                cassetteinfo = SortArray(cassetteinfo);
                int TongSoTienCanRut = Amount;
                Dictionary<string, int> CassetteDispenseCount = new Dictionary<string, int>();
                CassetteDispenseCount = CalculatorNotes(TongSoTienCanRut, cassetteinfo, TongSoHocTien);
                if (checkCalculatorNote(CassetteDispenseCount, TongSoTienCanRut) == false)
                {
                    m_objContext.NextCondition = EventDictionary.s_EventContinue;
                    Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount checkCalculatorNote= false");
                    return emBusActivityResult_t.Success;
                }
                m_objContext.TransactionDataCache.Set(DataDictionary.s_coreOriginalWithdrawalAmount, AMOUNTCWD, GetType());
                m_objContext.NextCondition = EventDictionary.s_EventConfirm;
                Log.Project.LogDebug("Leave action: VAB_InputWithdrawAmount");
                return emBusActivityResult_t.Success;

            }
            catch
            {
                Log.Project.LogDebug("InnerEndRun VAB_InputWithdrawAmount Fail");
            }

            return emBusActivityResult_t.Success;
        }
        protected override emBusiCallbackResult_t InnerOnUIEvtHandle(IUIService iUI, UIEventArg argUIEvent)
        {
            if (argUIEvent.EventName == UIPropertyKey.s_clickKey)
            {
                m_objContext.MainUI.ExecuteCustomCommand(UIServiceCommands.s_updateData);

                string strKeyOther = argUIEvent.Key as string;
                if (strKeyOther == "OnExit" || strKeyOther == "OnAnotherCashValue" || strKeyOther == "OnConfirmWithdraw")
                {
                    m_objContext.NextCondition = strKeyOther;
                    SignalCancel();
                }
                
                return emBusiCallbackResult_t.Swallowd;
            }
            return base.InnerOnUIEvtHandle(iUI, argUIEvent);
        }
        protected override void InnerEndRun(BusinessServiceProtocol.BusinessContext argContext)
        {
            base.InnerEndRun(argContext);

        }
        public Dictionary<string, int> CalculatorNotes(int SoTienCanRut, CassetteInfo[] Cassette, int tonghoctien)
        {
            //    Log.Project.LogDebug("CalculatorNotes Function: Amount="+ SoTienCanRut.ToString()+ " total cassette "+tonghoctien.ToString());
            int l_iTotalMoney = SoTienCanRut;
            int[] l_aiBill = new int[6];
            Dictionary<string, int> value = new Dictionary<string, int>();
            int[] l_iTotalDenomination = new int[] { 0, 0, 0, 0, 0, 0 };

            int[] l_alDenomination = new int[6];
            l_alDenomination[0] = 500000;
            l_alDenomination[1] = 200000;
            l_alDenomination[2] = 100000;
            l_alDenomination[3] = 50000;
            l_alDenomination[4] = 20000;
            l_alDenomination[5] = 10000;



            int l_iBoxNo = 0;
            int l_iCount = 0;
            int dem = 0;
            int l_iTotalCount = 0;
            int l_iMaxCount = 60;
            while ((l_iTotalMoney > 0) && (l_iBoxNo < 6))
            {
                for (dem = 0; dem < tonghoctien; dem++)
                {
                    if (l_alDenomination[l_iBoxNo] == Cassette[dem].Denomination)
                    {
                        Log.Project.LogDebug("Detail " + Cassette[dem].Denomination.ToString() + " count " + Cassette[dem].Remain);
                        l_iCount = l_iTotalMoney / l_alDenomination[l_iBoxNo];
                        if (Cassette[dem].Remain < l_iCount)
                        {
                            l_iCount = Cassette[dem].Remain;
                        }

                        l_aiBill[dem] = l_iCount;

                        l_iTotalMoney -= l_aiBill[dem] * (Cassette[dem].Denomination);
                        l_iTotalCount += l_aiBill[dem];
                    }

                }
                l_iBoxNo++;


            }


            if ((0 != l_iTotalMoney) || (l_iTotalCount > l_iMaxCount))
            {

                if (l_iTotalCount > l_iMaxCount)
                {
                    Log.Project.LogDebug("CalculatorNotes More than Max counts");

                }

                return value;
            }




            // RETURN VALUE

            for (int i = 0; i < tonghoctien; i++)
            {
                value.Add(Cassette[i].IDHopper, l_aiBill[i]);
                Log.Project.LogDebug("return CalculatorNotes " + Cassette[i].IDHopper + " " + l_aiBill[i]);
            }

            return value;
        }

        public bool checkCalculatorNote(Dictionary<string, int> CassetteDispenseCount, int totalAmount)
        {
            List<CashUnitInfo> argUintInfo = new List<CashUnitInfo>();
            int totalCompare = 0;
            //   Log.Project.LogDebug("checkCalculatorNote Function debug");
            m_objContext.CashDispenser.GetCashUnitInfo(out argUintInfo);
            for (int i = 0; i < argUintInfo.Count; i++)
            {
                foreach (var item in CassetteDispenseCount)
                {
                    if (argUintInfo[i].UnitID == item.Key) totalCompare = totalCompare + item.Value * argUintInfo[i].DenoValue;
                    //     Log.Project.LogDebug(argUintInfo[i].UnitID+" count "+ item.Key);
                }

            }
            if (totalAmount != totalCompare) return false;
            else return true;
        }
        public CassetteInfo[] SortArray(CassetteInfo[] cassetteinfo)
        {
            CassetteInfo tem = new CassetteInfo();
            for (int i = 0; i < cassetteinfo.Length - 1; i++)
            {
                for (int j = i + 1; j < cassetteinfo.Length; j++)
                {
                    if (cassetteinfo[i].Denomination < cassetteinfo[j].Denomination)
                    {
                        tem = cassetteinfo[i];
                        cassetteinfo[i] = cassetteinfo[j];
                        cassetteinfo[j] = tem;
                    }
                }
            }
            return cassetteinfo;
        }

        protected override emBusActivityResult_t InnerPreRun(BusinessContext argContext)
        {

            base.InnerPreRun(argContext);
            m_objContext = (eCATContext)argContext;
            Log.Project.LogDebug("Enter action: InnerPreRun VAB_InputWithdrawAmount");
            object value = null;
            // set value selectaccount and show on HTML
            //set value CWDDenoAvailable and show on HTML

            Log.Project.LogDebug("Leave action: InnerPreRun VAB_InputWithdrawAmount");
            return emBusActivityResult_t.Success;
        }
    }
}
