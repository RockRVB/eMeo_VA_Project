using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBankProjectBusinessServiceProtocol
{
    public class CustomerInfo
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Success { get; set; }
        public string RequestId { get; set; }
        public string ResponseTime { get; set; }
        public string TransReference { get; set; }
        public string CIF { get; set; }
        public string FullName { get; set; }
        public string DOB { get; set; }
        public string POB { get; set; } //Đây là địa chỉ nơi sinh
        public string POR { get; set; } //Đây là địa chỉ thường trú, cái này mới add thêm
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string IDNumber { get; set; }
        public string EkycStatus { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string OpenCifDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        
        public string ResidentialStatus { get; set; }
        public string Occupation { get; set; }
        public string Position { get; set; }
        
        
        public string BranchCode { get; set; }
        public string Employee { get; set; }
        
        public string IDType { get; set; }
        public string CBSIdType { get; set; }
        public string IDIssueDate { get; set; }
        public string IDExprireDate { get; set; }
        public string IDIssuePlace { get; set; }
        
        public string Nationality { get; set; }
        public string AddressType { get; set; }
        public string CurrentAddress { get; set; } // Đây là địa chỉ hiện tại đang ở
        public string City { get; set; }
        public string CityCode { get; set; }
        public string Province { get; set; }
        public string Distict { get; set; }
        public string Country { get; set; }
        public string CBS { get; set; }
        public string CusDocType { get; set; }
        
        public string LegalExpDate { get; set; }
        public string JobCode { get; set; }
        public string PositionCode { get; set; }
        public string TaxCode { get; set; }
        public string BranchName { get; set; }
        public string EBankEmail { get; set; }
        public string EBankPhone { get; set; }
        public string EBankInquiry { get; set; }
        public string EBankFinance { get; set; }
        public string OccupationCode { get; set; }
        public string IDExprireDateStatus { get; set; }
        public List<Account> Accounts { get; set; }

        public CustomerInfo()
        {
            Code = string.Empty;
            Message = string.Empty;
            Success = string.Empty;
            RequestId = string.Empty;
            ResponseTime = string.Empty;
            TransReference = string.Empty;
            CIF = string.Empty;
            Status = string.Empty;
            OpenCifDate = string.Empty;
            FirstName = string.Empty;
            MiddleName = string.Empty;
            LastName = string.Empty;
            Gender = string.Empty;
            ResidentialStatus = string.Empty;
            Occupation = string.Empty;
            Position = string.Empty;
            MobileNo = string.Empty;
            Email = string.Empty;
            BranchCode = string.Empty;
            Employee = string.Empty;
            IDNumber = string.Empty;
            IDType = string.Empty;
            CBSIdType = string.Empty;
            IDIssueDate = string.Empty;
            IDExprireDate = string.Empty;
            IDIssuePlace = string.Empty;
            DOB = string.Empty;
            POB = string.Empty;
            Nationality = string.Empty;
            AddressType = string.Empty;
            CurrentAddress = string.Empty;
            City = string.Empty;
            CityCode = string.Empty;
            Province = string.Empty;
            Distict = string.Empty;
            Country = string.Empty;
            CBS = string.Empty;
            CusDocType = string.Empty;
            FullName = string.Empty;
            LegalExpDate = string.Empty;
            JobCode = string.Empty;
            PositionCode = string.Empty;
            TaxCode = string.Empty;
            BranchName = string.Empty;
            EBankEmail = string.Empty;
            EBankPhone = string.Empty;
            EBankInquiry = string.Empty;
            EBankFinance = string.Empty;
            OccupationCode = string.Empty;
            IDExprireDateStatus = string.Empty;
            EkycStatus = string.Empty;
            Accounts = new List<Account>();
        }
    }
    public class Account
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AvailableBalance { get; set; }
        public string Currency { get; set; }
        public string AccountStatus { get; set; }
        public string BranchCode { get; set; }
        public Account()
        {
            AccountNumber = string.Empty;
            AccountName = string.Empty;
            AvailableBalance = string.Empty;
            Currency = string.Empty;
            AccountStatus = string.Empty;
            BranchCode = string.Empty;
        }
    }
    public class Fee
    {
        public string FeeCode { get; set; }
        public string FeeAmount { get; set; }
        public string TaxAmount { get; set; }
        public string Currency { get; set; }
        public Fee()
        {
            FeeCode = string.Empty;
            FeeAmount = string.Empty;
            TaxAmount = string.Empty;
            Currency = string.Empty;
        }
    }
    public class DepositResutl
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Success { get; set; }
        public string RequestId { get; set; }
        public string ResponseTime { get; set; }
        public string TransReference { get; set; }
        public string CifNumber { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AvailableBalance { get; set; }
        public string Currency { get; set; }
        public string BranchCode { get; set; }
        public DepositResutl()
        {
            Code = string.Empty;
            Message = string.Empty;
            RequestId = string.Empty;
            ResponseTime = string.Empty;
            TransReference = string.Empty;
            CifNumber = string.Empty;
            AccountNumber = string.Empty;
            AccountName = string.Empty;
            AvailableBalance = string.Empty;
            Currency = string.Empty;
            BranchCode = string.Empty;
        }
    }
    [Serializable]
    public class VTMDepositDetail
    {
        public string Denomination { get; set; }

        public int NoteNumber { get; set; }

        public string Amount { get; set; }
    }
}
