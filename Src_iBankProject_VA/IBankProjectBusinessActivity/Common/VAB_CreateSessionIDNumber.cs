using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Attribute4ECAT;
using VTMBusinessActivityBase;
using BusinessServiceProtocol;
using LogProcessorService;
using System.Configuration;
using eCATBusinessServiceProtocol;
using System.Threading;
using UIServiceProtocol;
using FaceRecognizeServiceProtocol;
using IBankProjectBusinessActivityBase;
using Newtonsoft.Json.Linq;

namespace IBankProjectBusinessActivity
{
    [GrgActivity("{BA81309E-4384-4FB8-BF75-B14BBBC09C90}",
                 NodeNameOfConfiguration = "VAB_CreateSessionIDNumber",
                 Name = "VAB_CreateSessionIDNumber",
                 Author = "Louis")]
    public class VAB_CreateSessionIDNumber : IBankProjectActivityBase
    {
        #region creating
        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VAB_CreateSessionIDNumber() as IBusinessActivity;
        }
        #endregion

        #region constructor
        protected VAB_CreateSessionIDNumber()
        {

        }
        #endregion

        protected override emBusActivityResult_t InnerRun(BusinessContext objContext)
        {
            Log.Action.LogDebugFormat("Enter Action: {0}", GetType());
            emBusActivityResult_t result = base.InnerRun(objContext);

            if (emBusActivityResult_t.Success != result)
            {
                Log.Action.LogError("Failed to run base's implement");
                return result;
            }

            if (GenerateSSessionIDNumber())
            {
                VTMContext.NextCondition = EventDictionary.s_EventNext;
            }
            else
            {
                Log.Action.LogError("Get cif response data is empty");
                VTMContext.NextCondition = EventDictionary.s_EventFail;
            }
            Log.Action.LogDebugFormat("Leave Action: {0}", GetType());
            return emBusActivityResult_t.Success;
        }
        public bool GenerateSSessionIDNumber()
        {
            bool result = false;
            try
            {
                string vtmno = VTMContext.TerminalConfig.Terminal.ATMNumber;
                string sessionID = string.Format("STM{0}{1}", vtmno, DateTimeOffset.Now.ToUnixTimeSeconds());
                ProjVTMContext.TransactionDataCache.Set("VAB_SessionID", sessionID, GetType());
                string journalString = "- SessionID   = [{0}]";
                ProjVTMContext.LogJournal(string.Format(journalString, sessionID));
                result = true;
            }
            catch (Exception ex)
            {
                Log.Action.LogError(ex.Message);
            }
            return result;
        }
    }
}
