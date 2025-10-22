namespace EFund_API.DataTransferObjects
{
    public class CreditSurvey
    {
        // 🔹 Nhóm 1: Thông tin cá nhân
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string IdentityNumber { get; set; }
        public string? MaritalStatus { get; set; }
        public int NumberOfDependents { get; set; }
        public string? EducationLevel { get; set; }
        public string? Address { get; set; }

        // 🔹 Nhóm 2: Nghề nghiệp & thu nhập
        public string? Occupation { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyType { get; set; }
        public int? YearsAtCurrentJob { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? SalaryPaymentMethod { get; set; }

        // Upload file ảnh
        public IFormFile? SalarySlipImage { get; set; }
        public IFormFile? UtilityBillImage { get; set; }

        // 🔹 Nhóm 4: Tài sản & đảm bảo
        public bool OwnHouseOrLand { get; set; }
        public bool OwnCarOrValuableVehicle { get; set; }
        public bool HasSavingsAccount { get; set; }
        public decimal? LifeInsuranceValue { get; set; }
        public string? Investments { get; set; }

        // 🔹 Nhóm 5: Lịch sử tín dụng
        public bool HadPreviousLoans { get; set; }
        public string? LoanInstitution { get; set; }
        public decimal? LoanLimit { get; set; }
        public decimal? CurrentOutstandingDebt { get; set; }
        public string? LoanTerm { get; set; }

        // 🔹 Nhóm 6: Liên hệ
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Facebook { get; set; }
    }
}
