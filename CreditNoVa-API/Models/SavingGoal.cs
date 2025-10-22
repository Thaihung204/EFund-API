using EFund_API.Models.Interfaces;
using EFund_API.WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFund_API.Models
{
    public class SavingGoal : Entity<Guid>
    {

        public Guid UserId { get; set; }

        public decimal IncomePerMonth { get; set; }
        public decimal TargetAmount { get; set; }
        public int DurationMonths { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyBudget { get; set; }

        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
