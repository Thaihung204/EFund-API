using EFund_API.Dtos;

namespace EFund_API.Services.Interfaces
{
    public interface ISavingGoalService
    {
        Task<SavingGoalResponse?> CreateAsync(Guid userId, CreateSavingGoalRequest request);
        Task<IEnumerable<SavingGoalResponse>> GetAllByUserAsync(Guid userId);
        Task<SavingGoalResponse?> GetByIdAsync(Guid id, Guid userId);
        Task<bool> UpdateAsync(Guid id, Guid userId, UpdateSavingGoalRequest request);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}
