namespace EFund_API.Dtos
{
    // DataTransferObjects/SavingGoalDtos.cs
    public class CreateSavingGoalRequest
    {
        public decimal IncomePerMonth { get; set; }
        public decimal TargetAmount { get; set; }
        public int DurationMonths { get; set; }
    }

    public class UpdateSavingGoalRequest
    {
        public decimal? IncomePerMonth { get; set; }
        public decimal? TargetAmount { get; set; }
        public int? DurationMonths { get; set; }
    }

    public class SavingGoalResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal IncomePerMonth { get; set; }
        public decimal TargetAmount { get; set; }
        public int DurationMonths { get; set; }
        public decimal DailyBudget { get; set; }
        public decimal CurrentSaved { get; set; }
        public decimal Progress { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
