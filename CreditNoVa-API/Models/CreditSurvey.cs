using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EFund_API.WebApp.Models;

namespace EFund_API.Models
{
    public class CreditSurvey : Entity<Guid>
    {
        // 🔹 Nhóm 1: Thông tin cá nhân
        #region Personal Information
        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; } // Nam/Nữ/Khác

        [Required, MaxLength(20)]
        public string IdentityNumber { get; set; } // CMND/CCCD/Hộ chiếu

        [MaxLength(20)]
        public string MaritalStatus { get; set; } // Độc thân/Kết hôn/Ly hôn/Góa

        [Range(0, 20)]
        public int NumberOfDependents { get; set; } // Số người phụ thuộc

        [MaxLength(50)]
        public string EducationLevel { get; set; } // Đại học / Cao đẳng / Trung cấp 

        [MaxLength(200)]
        public string Address { get; set; }
        #endregion

        // 🔹 Nhóm 2: Thông tin nghề nghiệp & thu nhập
        #region Employment & Income
        [MaxLength(100)]
        public string? Occupation { get; set; }  // Ngề nghiệp

        [MaxLength(150)]
        public string? CompanyName { get; set; }

        [MaxLength(50)]
        public string? CompanyType { get; set; } // Nhà nước/Tư nhân/Nước ngoài/Tự kinh doanh

        public int? YearsAtCurrentJob { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MonthlyIncome { get; set; }

        [MaxLength(20)]
        public string? SalaryPaymentMethod { get; set; } // Tiền mặt/Chuyển khoản

        public string? SalarySlipImagePath { get; set; } // đường dẫn file ảnh lưu bảng lương
        #endregion

        // 🔹 Nhóm 4: Thông tin tài sản & đảm bảo
        #region Assets & Collateral
        public bool OwnHouseOrLand { get; set; }
        public bool OwnCarOrValuableVehicle { get; set; }
        public bool HasSavingsAccount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LifeInsuranceValue { get; set; }

        public string? Investments { get; set; } // Cổ phiếu/trái phiếu/đầu tư khác
        public byte[]? SalarySlipImage { get; set; }  // Ảnh bảng lương
        public byte[]? UtilityBillImage { get; set; } // Ảnh hóa đơn điện nước
        #endregion

        // 🔹 Nhóm 5: Lịch sử tín dụng
        #region Credit History
        public bool HadPreviousLoans { get; set; }

        [MaxLength(150)]
        public string? LoanInstitution { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LoanLimit { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CurrentOutstandingDebt { get; set; }

        [MaxLength(50)]
        public string? LoanTerm { get; set; }
        #endregion

        // 🔹 Nhóm 6: Thông tin liên hệ
        #region Contact Information
        [Required, Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? Facebook { get; set; }
        #endregion

        public int? CreditScore { get; set; } // Điểm tín dụng
    }
}
