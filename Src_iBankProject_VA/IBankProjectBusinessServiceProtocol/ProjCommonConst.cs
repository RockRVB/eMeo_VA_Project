using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBankProjectBusinessServiceProtocol
{
    public class TransactionType
    {
        public const string ApplyCard = "ApplyCard";
        public const string ApplyAccount = "ApplyAccount";
    }
    public class TransactionReportType
    {
        public const string CreateAccountList = "CreateAccountList";
        public const string CardIssueList = "CardIssueList";
    }
}
