using System.Text;
using Azure;
using EFund_API.Models;
using EFund_API.Services.Interfaces;
using EFund_API.WebApp.Models;
using EFund_API.WebApp.Services.Interfaces;
using EFund_API.WebApp.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace EFund_API.Services
{
    public class CreditSurveyService : BaseService,  ICreditSurveyService
    {
        public CreditSurveyService(IRepository repo) : base(repo)
        {
        }
   
        public async Task<IEnumerable<CreditSurvey>> GetAllAsync()
        {
            return await Repo.GetAllAsync<CreditSurvey>();
        }

        public async Task<CreditSurvey?> GetByIdAsync(Guid id)
        {
            return await Repo.GetByIdAsync<CreditSurvey>(id);
        }

        public async Task<CreditSurvey> CreateAsync(EFund_API.DataTransferObjects.CreditSurvey dto)
        {
            var survey = new CreditSurvey
            {
                Id = Guid.NewGuid(),
                // 🔹 Nhóm 1: Personal Information
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                IdentityNumber = dto.IdentityNumber,
                MaritalStatus = dto.MaritalStatus,
                NumberOfDependents = dto.NumberOfDependents,
                EducationLevel = dto.EducationLevel,
                Address = dto.Address,

                // 🔹 Nhóm 2: Employment & Income
                Occupation = dto.Occupation,
                CompanyName = dto.CompanyName,
                CompanyType = dto.CompanyType,
                YearsAtCurrentJob = dto.YearsAtCurrentJob,
                MonthlyIncome = dto.MonthlyIncome,
                SalaryPaymentMethod = dto.SalaryPaymentMethod,

                // 🔹 Nhóm 4: Assets & Collateral
                OwnHouseOrLand = dto.OwnHouseOrLand,
                OwnCarOrValuableVehicle = dto.OwnCarOrValuableVehicle,
                HasSavingsAccount = dto.HasSavingsAccount,
                LifeInsuranceValue = dto.LifeInsuranceValue,
                Investments = dto.Investments,

                // 🔹 Nhóm 5: Credit History
                HadPreviousLoans = dto.HadPreviousLoans,
                LoanInstitution = dto.LoanInstitution,
                LoanLimit = dto.LoanLimit,
                LoanTerm = dto.LoanTerm,
                CurrentOutstandingDebt = dto.CurrentOutstandingDebt,

                // 🔹 Nhóm 6: Contact Information
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Facebook = dto.Facebook
            };

            if (dto.SalarySlipImage != null)
            {
                using var ms = new MemoryStream();
                await dto.SalarySlipImage.CopyToAsync(ms);
                survey.SalarySlipImage = ms.ToArray();
            }

            if (dto.UtilityBillImage != null)
            {
                using var ms = new MemoryStream();
                await dto.UtilityBillImage.CopyToAsync(ms);
                survey.UtilityBillImage = ms.ToArray();
            }

            await Repo.CreateAsync(survey);
            await Repo.SaveAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    var score = await CalculateCreditScoreAndSendAsync(survey);
                    survey.CreditScore = score;

                    Repo.Update(survey); 
                    await Repo.SaveAsync();
                }
                catch (Exception ex)
                {
                    
                }
            });

            return survey;
        }

        public async Task<CreditSurvey?> UpdateAsync(Guid id, CreditSurvey survey)
        {
            var existing = await Repo.GetByIdAsync<CreditSurvey>(id);
            if (existing == null) return null;
            existing.SalarySlipImage = survey.SalarySlipImage;
            Repo.Update(existing);
            await Repo.SaveAsync();

            return existing;
        }

        public async Task<CreditSurvey?> UpdateScore(Guid id, int score)
        {
            var existing = await Repo.GetByIdAsync<CreditSurvey>(id);
            if (existing == null) return null;
            existing.CreditScore = score;
            Repo.Update(existing);
            await Repo.SaveAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await Repo.GetByIdAsync<CreditSurvey>(id);
            if (entity == null) return false;

            Repo.Delete(entity);
            await Repo.SaveAsync();

            return true;
        }

        private async Task<int> CalculateCreditScoreAndSendAsync(CreditSurvey survey)
        {
            string salarySlipBase64 = survey.SalarySlipImage != null
                ? Convert.ToBase64String(survey.SalarySlipImage)
                : null;

            string utilityBillBase64 = survey.UtilityBillImage != null
                ? Convert.ToBase64String(survey.UtilityBillImage)
                : null;

            using (var client = new HttpClient())
            {
                var payload = new
                {
                    survey.Id,
                    survey.FullName,
                    survey.DateOfBirth,
                    survey.Gender,
                    survey.IdentityNumber,
                    survey.MaritalStatus,
                    survey.NumberOfDependents,
                    survey.EducationLevel,
                    survey.Address,
                    survey.Occupation,
                    survey.CompanyName,
                    survey.CompanyType,
                    survey.YearsAtCurrentJob,
                    survey.MonthlyIncome,
                    survey.SalaryPaymentMethod,
                    SalarySlipImage = salarySlipBase64,
                    UtilityBillImage = utilityBillBase64,
                    survey.OwnHouseOrLand,
                    survey.OwnCarOrValuableVehicle,
                    survey.HasSavingsAccount,
                    survey.LifeInsuranceValue,
                    survey.Investments,
                    survey.HadPreviousLoans,
                    survey.LoanInstitution,
                    survey.LoanLimit,
                    survey.CurrentOutstandingDebt,
                    survey.LoanTerm,
                    survey.PhoneNumber,
                    survey.Email,
                    survey.Facebook
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://dattien2703.app.n8n.cloud/webhook-test/cc9772a0-c0f9-49c5-850f-3942beceb49a",
                    content);

                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(responseString);

                    if (doc.RootElement.TryGetProperty("credit_score", out var scoreElement))
                    {
                        return scoreElement.GetInt32();
                    }

                    throw new Exception("Không tìm thấy trường 'credit_score' trong JSON.");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi parse JSON: {ex.Message}\nResponse: {responseString}");
                }
            }
        }

        private int CalculateCreditScore(CreditSurvey survey)
        {
            int score = 500;

            if (survey.MonthlyIncome > 20000000) score += 100;
            if (survey.CurrentOutstandingDebt == 0) score += 50;
            if (survey.HadPreviousLoans &&
                string.Equals(survey.LoanTerm, "Đúng hạn", StringComparison.OrdinalIgnoreCase))
                score += 50;

            return Math.Min(score, 850); // max 850
        }
    }
}
