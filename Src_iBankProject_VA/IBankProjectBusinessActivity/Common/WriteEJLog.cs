using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Attribute4ECAT;
using BusinessServiceProtocol;
using eCATBusinessServiceProtocol;
using LogProcessorService;
using ResourceManagerProtocol;
using VTMBusinessActivityBase;

namespace IBankProjectBusinessActivity
{
[GrgActivity("{8DE08C98-AD57-4609-A8E4-532C06992AC1}",
                Name = "WriteEJLog",
                Description = "WriteEJLog Description",
                NodeNameOfConfiguration = "WriteEJLog",
                Catalog = "VTMActivities",
                ForwardTargets = new string[] { EventDictionary.s_EventConfirm },
                Author = "louis")]
    public class VTMWriteEJLog : BusinessActivityVTMBase
    {
        #region Constructor/Create
        public VTMWriteEJLog()
        {
        }

        [GrgCreateFunction("Create")]
        public static IBusinessActivity Create()
        {
            return new VTMWriteEJLog() as IBusinessActivity;
        }

        #endregion Constructor/Create
        
        #region Property
        private string logData = String.Empty;
        [GrgBindTarget("LogData", Access = AccessRight.ReadAndWrite, Type = TargetType.String)]
        public string LogData
        {
            get
            {
                return logData;
            }
            set
            {
                logData = value;
            }
        }
        #endregion Property

        #region Method
        protected override emBusActivityResult_t InnerRun(BusinessContext argContext)
        {
            Log.Action.LogDebugFormat("{0}: Enter Action.", GetType());

            //Log.ElectricJournal.LogInfo("VTMWriteEJLog");

            emBusActivityResult_t emRet = base.InnerRun(argContext);
            if (emRet != emBusActivityResult_t.Success)
            {
                Log.Action.LogErrorFormat("{0}: Run Base's Implement Failed.", GetType());
            }
            if (!string.IsNullOrWhiteSpace(logData))
            {
                string ejTransactionStart = "TRANSACTION START";

                string[] arrayData = logData.Split(';');
                Log.Action.LogDebugFormat("{0}: LogData Has {1} Line(s) for EJ.", GetType(), arrayData.Length);
                for (int i = 0; i < arrayData.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(arrayData[i]))
                    {
                        int spacePrefixCount = arrayData[i].IndexOf(ejTransactionStart);
                        if (spacePrefixCount >= 0)
                        {
                            Log.Action.LogDebugFormat("{0}: Contain TRANSACTION START", GetType());

                            string ejFormat = "{0}{1} {2}";
                            string prefix = new string(' ', spacePrefixCount);
                            
                            // Log ATM NUMBER
                            string journal = m_objContext.CurrentJPTRResource.LoadString("STM NUMBER:", TextCategory.s_journal);
                            string journalValue = string.Format(ejFormat, prefix, journal, VTMContext.TerminalConfig.Terminal.ATMNumber);                           
                            Log.ElectricJournal.LogInfo(journalValue);

                            // Log DATE TIME
                            journal = m_objContext.CurrentJPTRResource.LoadString("DATE TIME:", TextCategory.s_journal);
                            journalValue = string.Format(ejFormat, prefix, journal, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            Log.ElectricJournal.LogInfo(journalValue);

                            //Log.ElectricJournal.LogInfo("   ATM NUMBER:  " + VTMContext.TerminalConfig.Terminal.ATMNumber);
                            //Log.ElectricJournal.LogInfo("   DATE TIME: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }

                        Log.ElectricJournal.LogInfo(arrayData[i]);
                    }
                }
            }
            else
            {
                Log.Action.LogDebugFormat("{0}: No Data for EJ.", GetType());
            }
            PrintCashUnitInfo();
            m_objContext.NextCondition = EventDictionary.s_EventConfirm;

            Log.Action.LogDebugFormat("{0}: Leave Action.", GetType());
            return emRet;
        }
        #endregion Method
        
    }
}
