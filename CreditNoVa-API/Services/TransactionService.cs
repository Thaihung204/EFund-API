using System.Collections.Generic;
using System.Linq;
using EFund_API.Dtos;
using EFund_API.Models;
using EFund_API.Services.Interfaces;
using EFund_API.WebApp.Services.Interfaces;
using EFund_API.WebApp.Services.Services;

namespace EFund_API.Services
{
    public class TransactionService : BaseService, ITransactionService
    {
        public TransactionService(IRepository repo) : base(repo) { }
        public async Task<TransactionResponse?> CreateAsync(Guid userId, CreateTransactionRequest request)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(request.GoalId);
            if (goal == null || goal.UserId != userId) return null;
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SavingGoal = goal,
                Amount = request.Amount,
                Description = request.Description,
            };
            await Repo.CreateAsync(transaction);
            await Repo.SaveAsync();
            return await MapToResponseAsync(transaction);
        }
        public async Task<IEnumerable<TransactionResponse>> GetByUserAsync(Guid userId)
        {
            var goals = await Repo.GetAsync<SavingGoal>(g => g.UserId == userId);
            var goalIds = goals.Select(g => g.Id).ToList();
            var transactions = await Repo.GetAsync<Transaction>(t => goalIds.Contains(t.SavingGoal.Id));
            var list = new List<TransactionResponse>();
            foreach (var t in transactions)
            {
                list.Add(await MapToResponseAsync(t));
            }
            return list;
        }
        public async Task<IEnumerable<TransactionResponse>> GetByGoalAsync(Guid userId, Guid goalId)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(goalId);
            if (goal == null || goal.UserId != userId) return new List<TransactionResponse>();
            var transactions = await Repo.GetAsync<Transaction>(t => t.SavingGoal.Id == goalId);
            var list = new List<TransactionResponse>();
            foreach (var t in transactions)
            {
                list.Add(await MapToResponseAsync(t));
            }
            return list;
        }
        public async Task<TransactionResponse?> GetByIdAsync(Guid userId, Guid id)
        {
            var transaction = await Repo.GetByIdAsync<Transaction>(id);
            if (transaction == null) return null;
            var goal = await Repo.GetByIdAsync<SavingGoal>(transaction.SavingGoal.Id);
            if (goal == null || goal.UserId != userId) return null;
            return await MapToResponseAsync(transaction);
        }
        public async Task<bool> DeleteAsync(Guid userId, Guid id)
        {
            var transaction = await Repo.GetByIdAsync<Transaction>(id);
            if (transaction == null) return false;
            var goal = await Repo.GetByIdAsync<SavingGoal>(transaction.SavingGoal.Id);
            if (goal == null || goal.UserId != userId) return false;
            Repo.Delete(transaction);
            await Repo.SaveAsync();
            return true;
        }
        private async Task<TransactionResponse> MapToResponseAsync(Transaction transaction)
        {
            var goal = await Repo.GetByIdAsync<SavingGoal>(transaction.SavingGoal.Id);
            return new TransactionResponse
            {
                Id = transaction.Id,
                GoalId = transaction.SavingGoal.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                CreatedDate = transaction.CreatedDate
            };
        }
    }
}
