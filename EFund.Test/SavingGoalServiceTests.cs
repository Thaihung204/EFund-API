using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFund_API.Models;
using EFund_API.Services;
using EFund_API.WebApp.Services.Interfaces;
using Moq;

namespace EFund.Test
{
    public class SavingGoalServiceTests
    {
        private readonly Mock<IRepository> _repoMock;
        private readonly SavingGoalService _service;
        private readonly Guid _userId = Guid.NewGuid();

        public SavingGoalServiceTests()
        {
            _repoMock = new Mock<IRepository>();
            _service = new SavingGoalService(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllByUserAsync_Should_Return_List_Of_SavingGoalResponse()
        {
            //Arrange
            var goals = new List<SavingGoal>
            {
                new SavingGoal
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TargetAmount = 1000,
                    DurationMonths = 2,
                    DailyBudget = 10,
                    IncomePerMonth = 5000
                },
                new SavingGoal
                {
                    Id = Guid.NewGuid(),
                    UserId = _userId,
                    TargetAmount = 2000,
                    DurationMonths = 4,
                    DailyBudget = 20,
                    IncomePerMonth = 6000
                }
            };

            // Mock GetAsync để trả về danh sách goals It.IsAny<Func<SavingGoal, bool>>())
            _repoMock.Setup(r => r.GetAllAsync<SavingGoal>(Func<IQueryable<SavingGoal>)
                     .ReturnsAsync(goals);

            // 🧠 Act
            var result = await _service.GetAllByUserAsync(_userId);

            // ✅ Assert
            var list = result.ToList();
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);

            Assert.Equal(goals[0].Id, list[0].Id);
            Assert.Equal(goals[1].Id, list[1].Id);

            //_repoMock.Verify(r => r.GetAsync<SavingGoal>(It.IsAny<Func<SavingGoal, bool>>()), Times.Once);
        }
    }
}
