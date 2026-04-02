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
using RemoteTellerServiceProtocol;
using System.IO;
using DocPrinterDeviceProtocol;
using VTMModelLibrary;
using CheckDev;
using System.Reflection;
using ResourceManagerProtocol;
using CardDispenserDeviceProtocol;
using CashAcceptorDeviceProtocol;
using CheckDeviceServiceProtocol;

namespace VTMBusinessActivity
{
    [GrgActivity("{F00B8C20-2465-49EF-9C3F-D9651BCA0CA5}",
                  Name = "GetDevStatusEx4Exhibition",
                  NodeNameOfConfiguration = "GetDevStatusEx4Exhibition",
                  Author = "ltfei1")]
    public class GetDevStatusEx4Exhibition : BusinessActivityGetDevStatus
    {
        #region constructor
        protected GetDevStatusEx4Exhibition()
        {

        }
        #endregion
        private const string UnknownError = "UnknownError";
        private List<CardUnitInfo> carUnitList = null;
        #region create
        [GrgCreateFunction("create")]
        public static new IBusinessActivity Create()
        {
            return new GetDevStatusEx4Exhibition();
        }
        #endregion

        #region methods

        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                VTMContext.CurrentTransactionResult = TransactionResult.Fail;
                VTMContext.NextCondition = EventDictionary.s_EventFail;
                Log.Action.LogError("execute base InnerRun failed");
                return emRet;
            }
            if (CheckDevStatus(devType))
            {
                VTMContext.NextCondition = EventDictionary.s_EventNormal;
            }
            else
            {
                if (m_objContext.UIState.ContainsKey("GetDevStatusExError"))
                {
                    SwitchUIState(m_objContext.MainUI, "GetDevStatusExError", 3000);
                    WaitSignal();
                }
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            return emBusActivityResult_t.Success;

        }


        protected virtual bool CheckDevSta(string devName)
        {

            return true;
        }
        private bool CheckDevStatus(string devName)
        {

            if (devName.Equals(DeviceServiceName.s_Stamper))
            {
                if (null != VTMContext.DocPrinter)
                {
                    VTMContext.DocPrinter.UpdateStatus();

                    if (!CheckStatus(VTMContext.DocPrinter))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_StamperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        //调用复位指令
                        DevResult devResult = VTMContext.DocPrinter.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetStamperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.DocPrinter.UpdateStatus();
                            if (CheckStatus(VTMContext.DocPrinter))
                            {
                                if (IsA4DevStates())
                                {
                                    VTMContext.LogJournalKey("IDS_StamperResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return true;
                                }
                                else
                                {
                                    VTMContext.LogJournalKey("IDS_StamperNoPaperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return false;
                                }
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_ResetStamperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                        }

                    }
                    else
                    {

                        if (IsA4DevStates())
                        {
                            return true;
                        }
                        else
                        {

                            VTMContext.LogJournalKey("IDS_StamperNoPaperFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_StamperNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("IDReader"))
            {
                if (null != VTMContext.IDReader)
                {
                    VTMContext.IDReader.UpdateStatus();

                    if (!CheckStatus(VTMContext.IDReader))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_ScanIDCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.IDReader.Reset(IDCResetOption.NoAction);
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetScanIDCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.IDReader.UpdateStatus();
                            if (!CheckStatus(VTMContext.IDReader))
                            {
                                VTMContext.LogJournalKey("IDS_ResetScanIDCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_IDCardResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_IDCardNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("UkeyRightReader"))
            {
                if (null != VTMContext.UKeyRightReader)
                {
                    VTMContext.UKeyRightReader.UpdateStatus();

                    if (!CheckStatus(VTMContext.UKeyRightReader))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_ReadUkeyRightRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.UKeyRightReader.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetReadUkeyRightRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.UKeyRightReader.UpdateStatus();
                            if (!CheckStatus(VTMContext.UKeyRightReader))
                            {
                                VTMContext.LogJournalKey("IDS_ResetReadUkeyRightRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_UkeyRightReadResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_UkeyRightReadNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("UkeyReader"))
            {
                if (null != VTMContext.UKeyReader)
                {
                    VTMContext.UKeyReader.UpdateStatus();

                    if (!CheckStatus(VTMContext.UKeyReader))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_ReadUkeyRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.UKeyReader.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetReadUkeyRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.UKeyReader.UpdateStatus();
                            if (!CheckStatus(VTMContext.UKeyReader))
                            {
                                VTMContext.LogJournalKey("IDS_ResetReadUkeyRawDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_UkeyReadResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_UkeyReadNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals(DeviceServiceName.s_IDC))
            {
                if (null != VTMContext.CardReader)
                {
                    VTMContext.CardReader.UpdateStatus();

                    if (!CheckStatus(VTMContext.CardReader))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_CardReaderDataFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.CardReader.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetCardReaderFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.CardReader.UpdateStatus();
                            if (!CheckStatus(VTMContext.CardReader))
                            {
                                VTMContext.LogJournalKey("IDS_ResetCardReaderFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_CardReaderResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        //是否检查有卡
                        if (IsEject == 1)
                        {
                            CardReaderDeviceProtocol.GrgCmdIDCStatusInfo stateInfo;
                            //检查通道中是否有卡
                            VTMContext.CardReader.GetStatusInfo(out stateInfo);
                            if (null != stateInfo)
                            {
                                if (stateInfo.MediaState == MediaState.Present)
                                //  if (stateInfo.MediaState == MediaState.Present || stateInfo.MediaState == MediaState.Entering)
                                {
                                    //吞卡
                                    HandleCaptureCardRecord(m_objContext.CurrentJPTRResource.LoadString("IDS_CaptureCardForIDCReset", TextCategory.s_journal));
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS__CardReaderNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }

            else if (devName.Equals(DeviceServiceName.s_CRD))
            {
                if (null != VTMContext.CardDispenser)
                {
                    VTMContext.CardDispenser.UpdateStatus();

                    if (!CheckStatus(VTMContext.CardDispenser))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_DispenseCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        //调用复位指令
                        DevResult devResult = VTMContext.CardDispenser.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetDispenseCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.CardDispenser.UpdateStatus();
                            if (CheckStatus(VTMContext.CardDispenser))
                            {
                                if (CheckCardOrUkeyUnit())
                                {
                                    VTMContext.LogJournalKey("IDS_CardDispenserResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return true;
                                }
                                else
                                {
                                    m_objContext.LogJournalKey("IDS_CardUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return false;
                                }
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_ResetDispenseCardFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (CheckCardOrUkeyUnit())
                        {
                            return true;
                        }
                        else
                        {
                            m_objContext.LogJournalKey("IDS_CardUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CardDispenserNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("UkeyRightDispenser"))
            {
                if (null != VTMContext.UKeyRight)
                {
                    VTMContext.UKeyRight.UpdateStatus();

                    if (!CheckStatus(VTMContext.UKeyRight))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_DispenseRightUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        //调用复位指令
                        DevResult devResult = VTMContext.UKeyRight.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetDispenseRightUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.UKeyRight.UpdateStatus();
                            if (CheckStatus(VTMContext.UKeyRight))
                            {
                                if (CheckCardOrUkeyUnit(2))
                                {
                                    VTMContext.LogJournalKey("IDS_UkeyRightDispenserResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return true;
                                }
                                else
                                {
                                    m_objContext.LogJournalKey("IDS_UkeyRightUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return false;
                                }
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_ResetDispenseRightUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (CheckCardOrUkeyUnit(2))
                        {
                            return true;
                        }
                        else
                        {
                            m_objContext.LogJournalKey("IDS_UkeyRightUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_UkeyRightDispenserNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("UkeyDispenser"))
            {
                if (null != VTMContext.UKey)
                {
                    VTMContext.UKey.UpdateStatus();

                    if (!CheckStatus(VTMContext.UKey))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_DispenseUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        //调用复位指令
                        DevResult devResult = VTMContext.UKey.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetDispenseUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.UKey.UpdateStatus();
                            if (CheckStatus(VTMContext.UKey))
                            {
                                if (CheckCardOrUkeyUnit(1))
                                {
                                    VTMContext.LogJournalKey("IDS_UkeyDispenserResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return true;
                                }
                                else
                                {
                                    m_objContext.LogJournalKey("IDS_UkeyUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                    return false;
                                }
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_ResetDispenseUkeyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (CheckCardOrUkeyUnit(1))
                        {
                            return true;
                        }
                        else
                        {
                            m_objContext.LogJournalKey("IDS_UkeyUnitEmptyFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_UkeyDispenserNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }           
            else if (devName.Equals("CAM"))
            {
                if (null != VTMContext.CameraService)
                {
                    VTMContext.CameraService.UpdateStatus();

                    if (!CheckStatus(VTMContext.CameraService))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_OpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.CameraService.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetOpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.CameraService.UpdateStatus();
                            if (!CheckStatus(VTMContext.CameraService))
                            {
                                VTMContext.LogJournalKey("IDS_ResetOpenScanFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_CAMResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CAMNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("USBCam"))
            {
                if (null != VTMContext.USBCAMService)
                {
                    VTMContext.USBCAMService.UpdateStatus();

                    if (!CheckStatus(VTMContext.USBCAMService))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_USBCamFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.USBCAMService.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetUSBCamFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.USBCAMService.UpdateStatus();
                            if (!CheckStatus(VTMContext.USBCAMService))
                            {
                                VTMContext.LogJournalKey("IDS_ResetUSBCamFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_USBCamResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_USBCamNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals("FingerScaner"))
            {
                if (null != VTMContext.FingerService)
                {
                    VTMContext.FingerService.UpdateStatus();

                    if (!CheckStatus(VTMContext.FingerService))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_FingerScanerFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.FingerService.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_FingerScanerFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.FingerService.UpdateStatus();
                            if (!CheckStatus(VTMContext.FingerService))
                            {
                                VTMContext.LogJournalKey("IDS_FingerScanerFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_FingerScanerResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_FingerScanerNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals(DeviceServiceName.s_CIM))
            {
                if (null != VTMContext.CashAcceptor)
                {
                    VTMContext.CashAcceptor.UpdateStatus();

                    if (!CheckStatus(VTMContext.CashAcceptor))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_CIMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.CashAcceptor.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetCIMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.CashAcceptor.UpdateStatus();
                            if (!CheckStatus(VTMContext.CashAcceptor))
                            {
                                VTMContext.LogJournalKey("IDS_ResetCIMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_CIMResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CIMNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else if (devName.Equals(DeviceServiceName.s_CDM))
            {
                if (null != VTMContext.CashDispenser)
                {
                    VTMContext.CashDispenser.UpdateStatus();

                    if (!CheckStatus(VTMContext.CashDispenser))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                        VTMContext.LogJournalKey("IDS_CDMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);

                        DevResult devResult = VTMContext.CashDispenser.Reset();
                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetCDMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            VTMContext.CashDispenser.UpdateStatus();
                            if (!CheckStatus(VTMContext.CashDispenser))
                            {
                                VTMContext.LogJournalKey("IDS_ResetCDMFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_CDMResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CDMNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }

            else if (devName.Equals(DeviceServiceName.s_CHK))
            {
                if (null != VTMContext.CheckDevService)
                {
                    if (!CheckStatus(VTMContext.CheckDevService, "GetStatus"))
                    {
                        SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);
                        VTMContext.LogJournalKey("IDS_CHKFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                        DevResult devResult;
                        if (IsEject == 1)
                        {
                            devResult = VTMContext.CheckDevService.Reset(ResetAction.Reject);
                        }
                        else
                        {
                            devResult = VTMContext.CheckDevService.Reset(ResetAction.None);
                        }

                        if (devResult.IsFailure)
                        {
                            VTMContext.LogJournalKey("IDS_ResetCHKFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                            return false;
                        }
                        else
                        {
                            if (!CheckStatus(VTMContext.CheckDevService))
                            {
                                VTMContext.LogJournalKey("IDS_ResetCHKFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return false;
                            }
                            else
                            {
                                VTMContext.LogJournalKey("IDS_CHKResetSuccess", TextCategory.s_journal, LogSymbol.DeviceFailure);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    VTMContext.LogJournalKey("IDS_CHKNoDevFailure", TextCategory.s_journal, LogSymbol.DeviceFailure);
                    return false;
                }
            }
            else
            {
                return CheckDevSta(devName);
            }
        }

        protected virtual bool CheckCardOrUkeyUnit(int flag = 0)
        {
            if (flag == 0)
            {
                VTMContext.CardDispenser.GetCardUnitInfo(out carUnitList);
            }
            else if (flag == 1)
            {
                VTMContext.UKey.GetCardUnitInfo(out carUnitList);
            }
            else
            {
                VTMContext.UKeyRight.GetCardUnitInfo(out carUnitList);
            }

            if (null != carUnitList && carUnitList.Count > 0)
            {
                bool isAllCardEmpty = false;
                foreach (CardUnitInfo cardUnit in carUnitList)
                {
                    if (cardUnit.Type == emCRDType.SupplyBin && cardUnit.Count > 0)
                    {
                        isAllCardEmpty = true;
                    }
                }
                return isAllCardEmpty;
            }
            else
            {
                return false;
            }
        }

        protected virtual bool CheckCommonDev(DeviceServiceWrapperBase dw, string idsNodev, string idsDevFailure, string idsResetDevSuccess)
        {

            if (null != dw)
            {
                if (!CheckStatus(dw))
                {
                    SwitchUIState(VTMContext.MainUI, DataDictionary.s_DefaultUIState, -int.MaxValue);

                    VTMContext.LogJournalKey(idsDevFailure, TextCategory.s_journal, LogSymbol.DeviceFailure);
                    //调用复位指令
                    if (ResetDev(dw))
                    {
                        VTMContext.LogJournalKey(idsResetDevSuccess, TextCategory.s_journal, LogSymbol.DeviceFailure);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                VTMContext.LogJournalKey(idsNodev, TextCategory.s_journal, LogSymbol.DeviceFailure);
                return false;
            }
        }

        private Type tp;
        private GrgDevInfo devInfo;
        private object[] ParamArray;
        private ParameterModifier[] ParamMods;
        private DevResult result;
        /// <summary>
        /// 验证状态
        /// </summary>
        /// <param name="statusName">状态域名</param>
        /// <param name="errorCodeName">错误码域名</param>
        /// <param name="dw">协议</param>
        /// <param name="strBuild">变长字符对象</param>
        /// <param name="argKeyValueOfPack">数据对象</param>
        /// <param name="methedNameame">方法名称 默认GetStatusInfo</param>
        protected virtual bool CheckStatus(DeviceServiceWrapperBase dw, string methedNameame = "GetStatusInfo", string devName = "")
        {
            try
            {
                if (null == dw)
                {
                    return false;
                }
                else
                {
                    tp = dw.GetType();
                    devInfo = null;
                    ParamArray = new object[2];
                    ParamArray[0] = devInfo;
                    ParamArray[1] = m_devTimeout;
                    ParamMods = new ParameterModifier[1];
                    ParamMods[0] = new ParameterModifier(2); // 初始化为接口参数的个数
                    ParamMods[0][0] = true;
                    result = (DevResult)tp.InvokeMember(methedNameame, // 接口函数名
     BindingFlags.Default | BindingFlags.InvokeMethod,
     null,
     dw,
     ParamArray, // 参数数组
     ParamMods, // 指定返回参数的ParameterModifier数组
     null,
     null);
                    if (null != ParamArray[0])
                    {
                        devInfo = ParamArray[0] as GrgDevInfo;
                    }

                    if (null != devInfo && (HardwareState.Online.Equals(devInfo.DeviceState) || HardwareState.Busy.Equals(devInfo.DeviceState)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("error for  DevStatus", ex);
                return false;
            }
        }

        protected virtual bool ResetDev(DeviceServiceWrapperBase dw, string methedNameame = "Reset")
        {

            try
            {
                if (null == dw)
                {
                    return false;
                }
                else
                {
                    tp = dw.GetType();
                    result = (DevResult)tp.InvokeMember(methedNameame, BindingFlags.InvokeMethod,
null, dw, new object[] { });
                    if (result.IsFailure)
                    {
                        return false;
                    }
                    else
                    {

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Action.LogError("ResetDev error", ex);
                return false;
            }
        }

        /// <summary>
        /// 打印机纸状态转换
        /// </summary>
        /// <param name="strBuild">可变字符</param>
        /// <param name="docp">纸源状态</param>
        private bool ConvertPaperState(DocPrinterDeviceProtocol.PAPERSourceState docp)
        {
            switch (docp)
            {
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPERJAMMED:
                    Log.Action.LogError("Printer error:" + EquipmentStatus.JamPaper.ToString());
                    return false;
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPERFULL:
                    return true;
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPERLOW:
                    Log.Action.LogWarn("Printer Warn:" + EquipmentStatus.PaperLow.ToString());
                    return true;
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPERNOTSUPP:
                    Log.Action.LogError("Printer error:" + EquipmentStatus.NoPaper.ToString());
                    return false;
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPEROUT:
                    Log.Action.LogError("Printer error:" + EquipmentStatus.NoPaper.ToString());
                    return false;
                case DocPrinterDeviceProtocol.PAPERSourceState.PAPERUNKNOWN:
                    Log.Action.LogError("Printer error:" + EquipmentStatus.NoPaper.ToString());
                    return false;
            }

            return false;
        }
        protected virtual bool IsA4DevStates()
        {

            DocPrinterDeviceProtocol.GrgCmdDocPrinterStatusInfo l_printStatusInfo = null;
            VTMContext.DocPrinter.GetStatusInfo(out l_printStatusInfo, m_devTimeout);

            if (null != l_printStatusInfo && (HardwareState.Online == l_printStatusInfo.DeviceState || HardwareState.Busy == l_printStatusInfo.DeviceState))
            {
                //打印机设备正常 
                // 5－纸少 6－没纸 8－卡纸 
                //获得纸源
                DocPrinterDeviceProtocol.GrgCmdDocPrinterCapInfo docCap = null;
                VTMContext.DocPrinter.GetCapabilitiesInfo(out docCap, string.Empty, m_devTimeout);

                if (null != docCap && docCap.PaperSources.Count > 0)
                {
                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPERAUX))
                    {
                        //状态转换
                        if (ConvertPaperState(l_printStatusInfo.PAPERAUXState))
                        {
                            return true;
                        }
                    }

                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPERAUX2))
                    {
                        //状态转换
                        if (ConvertPaperState(l_printStatusInfo.PAPERAUX2State))
                        {
                            return true;
                        }
                    }

                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPEREXTERNAL))
                    {
                        if (ConvertPaperState(l_printStatusInfo.PAPEREXTERNALState))
                        {
                            return true;
                        }
                    }
                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPERLOWER))
                    {
                        if (ConvertPaperState(l_printStatusInfo.PAPERLOWERState))
                        {
                            return true;
                        }
                    }

                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPERPARK))
                    {
                        if (ConvertPaperState(l_printStatusInfo.PAPERPARKState))
                        {
                            return true;
                        }
                    }
                    if (docCap.PaperSources.Contains(DocPrinterDeviceProtocol.PaperSource.PAPERUPPER))
                    {
                        if (ConvertPaperState(l_printStatusInfo.PAPERUPPERState))
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        protected override bool InnerCanTerminate(ref string strMsg)
        {
            strMsg = "GetDevStatusEx can not Terminate";
            return false;
        }

        private int m_IsEject = 0;
        [GrgBindTarget("eject", Access = AccessRight.ReadAndWrite, Type = TargetType.Short)]
        public int IsEject
        {
            get
            {
                return m_IsEject;
            }
            set
            {
                m_IsEject = value;
                OnPropertyChanged("IsEject");
            }
        }

        protected VTMBusinessContext VTMContext
        {
            get
            {
                if (null == m_objContext ||
                     !(m_objContext is VTMBusinessContext))
                {
                    return null;
                }

                return (VTMBusinessContext)m_objContext;
            }
        }

        #endregion
    }
}
