using EFund_API.Models;

namespace EFund_API.Services.Interfaces
{
    public interface ICreditSurveyService
    {
        Task<IEnumerable<CreditSurvey>> GetAllAsync();
        Task<CreditSurvey> GetByIdAsync(Guid id);
        Task<CreditSurvey> CreateAsync(EFund_API.DataTransferObjects.CreditSurvey survey);
        Task<CreditSurvey> UpdateAsync(Guid id, CreditSurvey survey);
        Task<CreditSurvey> UpdateScore(Guid id, int survey);
        Task<bool> DeleteAsync(Guid id);
    }
}
