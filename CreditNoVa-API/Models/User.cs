using EFund_API.WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFund_API.Models
{
    [Table("Users")]
    public class User : Entity<Guid>
    {
        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsPhoneVerified { get; set; } = false;

        [MaxLength(100)]
        public string? Avatar { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        // Authentication fields

        // 🔗 Navigation Properties
        public ICollection<CreditSurvey> CreditSurveys { get; set; } = new List<CreditSurvey>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<SavingGoal> SavingGoals { get; set; } = new List<SavingGoal>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
