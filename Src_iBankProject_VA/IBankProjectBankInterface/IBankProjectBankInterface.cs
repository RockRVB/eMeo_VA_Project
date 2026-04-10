using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using VTMBusinessActivity.VTMBankInterface;
using Attribute4ECAT;
using BusinessServiceProtocol;
using IBankProjectBusinessServiceProtocol;
using VideoServiceProtocol;
using VTMBusinessServiceProtocol;
using LogProcessorService;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WebSocketServiceProtocol;
using System.Text.RegularExpressions;
using System.Text;

namespace IBankProjectBankInterface

{
    [GrgBankInterface(
        Name = "IBankProjectBankInterface",
        Bank = "Ibank"
    )]
    public partial class IBankProjectBankInterface : VTMBankInterface
    {
        private IBankProjectBusinessServiceContext ProjVTMContext;

        [GrgCreateFunction("Create")]
        public static new IBankProjectBankInterface Create(BusinessContext argContext)
        {
            return new IBankProjectBankInterface(argContext);
        }

        public IBankProjectBankInterface(BusinessContext argContext) : base(argContext)
        {
            ProjVTMContext = argContext as IBankProjectBusinessServiceContext;
            if (ProjVTMContext == null)
            {
                throw new NullReferenceException(nameof(ProjVTMContext) + "is null!");
            }

            ProjVTMContext.VideoStatusWrapper = new VideoStatusWrapper(ProjVTMContext.VideoService);
            ProjVTMContext.TerminalDataCache.Set(VTMDataDictionary.VtmVideoStatusWrapper, ProjVTMContext.VideoStatusWrapper, GetType());
        }

        public override emRetCode BeforePackRestMessage(string argType)
        {
            Log.Project.LogDebugFormat("BankInterface.BeforePackRestMessage({0}) ", argType);
            SetJsonHeader(argType);
            SetJsonData(argType);
            WriteJournalLogBefore(argType);
            return emRetCode.Default;
        }

        public override emRetCode AfterUnpackRestMessage(string argType, string resultCode, string argResponse)
        {
            Log.BusinessService.LogDebugFormat($"AfterUnpackRestMessage argType: {argType}; resultCode: {resultCode} ; argResponse: {argResponse}");
            JObject dataObj = null;
            dataObj = JObject.Parse(argResponse);
            ProjVTMContext.LogJournal("TRANSACTION RESPONSE [" + argType + "]");
            ProjVTMContext.LogJournal("Response Code [" + dataObj["code"]?.ToString() + "]");
            ProjVTMContext.LogJournal("Response Desc [" + dataObj["message"]?.ToString().ConvertToEngUpChar() + "]");
            if (resultCode == "200000" || resultCode == "0" || resultCode == "200" || resultCode == "00")
                UnpackJsonData(argType, resultCode, argResponse);
             
            return emRetCode.Default;
        }


        public override emRetCode PackXDCMessage()
        {
            return emRetCode.Default;
        }

        public override emRetCode HandleXDCSendResult(emBusActivityResult_t SendResult)
        {
            return emRetCode.Default;
        }

        public override emRetCode HandleXDCInteractiveMsg()
        {
            return emRetCode.Default;
        }

        public override emRetCode HandleXDCErrorFieldValue(ref string errorInfo)
        {
            return emRetCode.Default;
        }

        public override emRetCode HandleXDCScreenUpdateData()
        {
            object obj_ScreenData = null;
            ProjVTMContext.TransactionDataCache.Get("core_ScreenData", out obj_ScreenData, GetType());
            if (obj_ScreenData != null)
            {
                string str_ScreenData = obj_ScreenData.ToString();
                char SI = (char)0x0F;
                int indexSI = str_ScreenData.IndexOf(SI);
                if (indexSI > -1)
                {
                    ProjVTMContext.TransactionDataCache.Set("newBalance", str_ScreenData.Substring(indexSI + 1), GetType());
                    //Log.Project.LogDebug("newBalance:"+ str_ScreenData.Substring(indexSI + 1));
                }
            }
            return emRetCode.Default;
        }

        public override emRetCode HandleXDCRecepitPrintData()
        {
            return emRetCode.Default;
        }

        public override emRetCode HandleEMVReceiveData(object objEMV)
        {
            return emRetCode.Default;
        }

        public override emRetCode CalculateCondition(string argIDValue1, string argIDValue2, string argIDOperator)
        {
            return emRetCode.Default;
        }

        public override emRetCode UnPackMessage(string msgID, object recvMsg, out List<string> replyBufferList)
        {
            replyBufferList = new List<string>();
            return emRetCode.Default;
        }

        public override object GetWebSocketServiceExtension(IWebSocketServiceProtocol webSocketService)
        {
            return new WebSocketServiceExtension
            {
                WebSocketService = webSocketService,
                Context = ProjVTMContext
            };
        }

        public override bool ValidateWebSocketServiceCommand(IWebSocketServiceProtocol webSocketService, string cmd)
        {
            if (!base.ValidateWebSocketServiceCommand(webSocketService, cmd))
            {
                return false;
            }

            var denyCmdPatterns = new List<string>();
            /*
            //CDM相关接口的正则表达式
            denyCmdPatterns.AddRange(new[] { "(.+(4CDM){1})", "(Denominate)", "(Denominate1)", "(Dispense1)", "(Dispense2)" });
            //CIM相关接口的正则表达式
            denyCmdPatterns.AddRange(new[] { "(.+(4CIM){1})", "(CashInStart)", "(GetDepositDeno)", "(CashInStart2)", "(CashIn)", "(CashInEnd)", "(CashInRollback)", "(CancelCashIn)", "(GetPUInfoCIM)", "(GetLastCashIn)" });
            */
            var denyCmdPattern = new StringBuilder();
            if (denyCmdPatterns.Count == 0)
            {
                return true;
            }

            denyCmdPattern.Append('^');
            for (var i = 0; i < denyCmdPatterns.Count; i++)
            {
                denyCmdPattern.Append(denyCmdPatterns[i]);
                if (i != denyCmdPatterns.Count - 1)
                {
                    denyCmdPattern.Append('|');
                }
            }
            denyCmdPattern.Append('$');

            var match = Regex.Match(cmd, denyCmdPattern.ToString());
            if (match.Success)
            {
                return false;
            }

            return true;
        }
        
        
        
    }
}