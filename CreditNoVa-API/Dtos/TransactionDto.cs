namespace EFund_API.Dtos
{
    // DataTransferObjects/TransactionDtos.cs
    public class CreateTransactionRequest
    {
        public Guid? GoalId { get; set; }
        public string Type { get; set; } // "Income", "Expense", "Saving"
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? GoalId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
