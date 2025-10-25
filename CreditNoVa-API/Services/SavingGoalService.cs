using System.Collections.Generic;
using System.Linq;
using EFund_API.Dtos;
using EFund_API.Models;
using EFund_API.Services.Interfaces;
using EFund_API.WebApp.Services.Interfaces;
using EFund_API.WebApp.Services.Services;

namespace EFund_API.Services
{
    public class SavingGoalService : BaseService, ISavingGoalService
    {
        public SavingGoalService(IRepository repo) : base(repo) { }

        public async Task<SavingGoalResponse?> CreateAsync(Guid userId, CreateSavingGoalRequest request)
        {
            if (request.DurationMonths <= 0) return null;

            var dailyBudget = request.TargetAmount / (request.DurationMonths * 30);

            var goal = new SavingGoal
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IncomePerMonth = request.IncomePerMonth,
                TargetAmount = request.TargetAmount,
                DurationMonths = request.DurationMonths,
                DailyBudget = dailyBudget
            };

            await Repo.CreateAsync(goal);
            await Repo.SaveAsync();

            return await MapToResponseAsync(goal);
        }

        public async Task<IEnumerable<SavingGoalResponse>> GetAllByUserAsync(Guid userId)
        {
            var goals = await Repo.GetAsync<SavingGoal>(g => g.UserId == userId);
            var list = new List<SavingGoalResponse>();
            foreach (var g in goals)
            {
                list.Add(await MapToResponseAsync(g));
            }
            return list;
        }

        public async Task<SavingGoalResponse?> GetByIdAsync(Guid id, Guid userId)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(id);
            if (goal == null || goal.UserId != userId) return null;
            return await MapToResponseAsync(goal);
        }

        public async Task<bool> UpdateAsync(Guid id, Guid userId, UpdateSavingGoalRequest request)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(id);
            if (goal == null || goal.UserId != userId) return false;

            if (request.IncomePerMonth.HasValue)
                goal.IncomePerMonth = request.IncomePerMonth.Value;
            if (request.TargetAmount.HasValue)
                goal.TargetAmount = request.TargetAmount.Value;
            if (request.DurationMonths.HasValue && request.DurationMonths.Value > 0)
                goal.DurationMonths = request.DurationMonths.Value;

            // Recalculate daily budget if inputs changed
            if (request.TargetAmount.HasValue || (request.DurationMonths.HasValue && request.DurationMonths.Value > 0))
            {
                var duration = goal.DurationMonths > 0 ? goal.DurationMonths : 1;
                goal.DailyBudget = goal.TargetAmount / (duration * 30);
            }

            Repo.Update(goal);
            await Repo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(id);
            if (goal == null || goal.UserId != userId) return false;

            Repo.Delete(goal);
            await Repo.SaveAsync();
            return true;
        }

        private async Task<SavingGoalResponse> MapToResponseAsync(SavingGoal goal)
        {
            // Sum all transactions linked to this goal with Type == "Saving"
            var txns = await Repo.GetAsync<EFund_API.Models.Transaction>(t => t.GoalId == goal.Id && t.UserId == goal.UserId && t.Type == "Saving");
            var currentSaved = txns.Sum(t => t.Amount);
            var ratio = goal.TargetAmount > 0 ? currentSaved / goal.TargetAmount : 0m;
            var progress = ratio < 0m ? 0m : (ratio > 1m ? 1m : ratio);

            return new SavingGoalResponse
            {
                Id = goal.Id,
                UserId = goal.UserId,
                IncomePerMonth = goal.IncomePerMonth,
                TargetAmount = goal.TargetAmount,
                DurationMonths = goal.DurationMonths,
                DailyBudget = goal.DailyBudget,
                CurrentSaved = currentSaved,
                Progress = progress,
                CreatedDate = goal.CreatedDate
            };
        }
    }
}
