using EFund_API.Dtos;

namespace EFund_API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse?> CreateAsync(Guid userId, CreateTransactionRequest request);
        Task<IEnumerable<TransactionResponse>> GetByUserAsync(Guid userId);
        Task<IEnumerable<TransactionResponse>> GetByGoalAsync(Guid userId, Guid goalId);
        Task<TransactionResponse?> GetByIdAsync(Guid userId, Guid id);
        Task<bool> DeleteAsync(Guid userId, Guid id);
    }
}
