using EFund_API.WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFund_API.Models
{
    public class Transaction : Entity<Guid>
    {

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(SavingGoal))]
        public Guid? GoalId { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; } // income, expense, saving

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        // Navigation properties
        public User User { get; set; }
        public SavingGoal SavingGoal { get; set; }
    }
}
