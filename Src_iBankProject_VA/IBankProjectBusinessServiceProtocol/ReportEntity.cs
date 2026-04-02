using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBankProjectBusinessServiceProtocol
{
    [Serializable]
    public class AccountReportEntity
    {
        public string AccountNumber { get; set; }

        public string AMLNumber { get; set; }

        public DateTime DateTime { get; set; }
    }

    [Serializable]
    public class CardIssueReportEntity
    {
        public string AccountNumber { get; set; }

        public string CardNumber { get; set; }

        public string Status { get; set; }

        public DateTime DateTime { get; set; }
    }
}
