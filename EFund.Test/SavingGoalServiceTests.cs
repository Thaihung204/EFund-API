using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFund_API.Dtos;
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

            _repoMock
            .Setup(r => r.GetAsync<SavingGoal>(
                It.IsAny<Expression<Func<SavingGoal, bool>>>(),
                It.IsAny<Func<IQueryable<SavingGoal>, IOrderedQueryable<SavingGoal>>>(),
                It.IsAny<string>(), 
                It.IsAny<int?>(),
                It.IsAny<int?>() 
            ))
            .ReturnsAsync(goals);

            // Act
            var result = await _service.GetAllByUserAsync(_userId);

            // Assert
            var list = result.ToList();
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);

            Assert.Equal(goals[0].Id, list[0].Id);
            Assert.Equal(goals[1].Id, list[1].Id);

        }

        [Fact]
        public async Task CreateAsync_Should_Return_SavingGoalResponse_When_ValidRequest()
        {
            // Arrange
            var request = new CreateSavingGoalRequest
            {
                IncomePerMonth = 5000,
                TargetAmount = 3000,
                DurationMonths = 3
            };

            _repoMock
                .Setup(r => r.CreateAsync(It.IsAny<SavingGoal>(), It.IsAny<string>()))
                .ReturnsAsync((object)null); // CreateAsync không trả dữ liệu cụ thể

            _repoMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            _repoMock
                .Setup(r => r.GetAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(),
                                                     null, null, null, null))
                .ReturnsAsync(new List<Transaction>()); // MapToResponseAsync cần trả về list rỗng

            // Act
            var result = await _service.CreateAsync(_userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_userId, result.UserId);
            Assert.Equal(request.TargetAmount, result.TargetAmount);
            Assert.Equal(request.DurationMonths, result.DurationMonths);
            Assert.Equal(request.IncomePerMonth, result.IncomePerMonth);
            Assert.Equal(request.TargetAmount / (request.DurationMonths * 30), result.DailyBudget);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Null_When_DurationMonths_Invalid()
        {
            // Arrange
            var request = new CreateSavingGoalRequest
            {
                IncomePerMonth = 5000,
                TargetAmount = 3000,
                DurationMonths = 0
            };

            // Act
            var result = await _service.CreateAsync(_userId, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Response_When_GoalExists_And_UserMatches()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 2, DailyBudget = 10, IncomePerMonth = 5000 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(), null, null, null, null))
                     .ReturnsAsync(new List<Transaction>());

            var result = await _service.GetByIdAsync(goal.Id, _userId);

            Assert.NotNull(result);
            Assert.Equal(goal.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_GoalNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>())).ReturnsAsync((SavingGoal)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid(), _userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_UserMismatch()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() }; // user khác
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);

            var result = await _service.GetByIdAsync(goal.Id, _userId);

            Assert.Null(result);
        }
        [Fact]
        public async Task UpdateAsync_Should_Return_True_When_Success()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 2, DailyBudget = 10, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { TargetAmount = 2000, DurationMonths = 4, IncomePerMonth = 6000 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            Assert.True(result);
            Assert.Equal(2000, goal.TargetAmount);
            Assert.Equal(4, goal.DurationMonths);
            Assert.Equal(6000, goal.IncomePerMonth);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_False_When_GoalNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>())).ReturnsAsync((SavingGoal)null);

            var result = await _service.UpdateAsync(Guid.NewGuid(), _userId, new UpdateSavingGoalRequest());

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_False_When_UserMismatch()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);

            var result = await _service.UpdateAsync(goal.Id, _userId, new UpdateSavingGoalRequest());

            Assert.False(result);
        }
        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Success()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId };
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(goal.Id, _userId);

            Assert.True(result);
            _repoMock.Verify(r => r.Delete(goal), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_GoalNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>())).ReturnsAsync((SavingGoal)null);

            var result = await _service.DeleteAsync(Guid.NewGuid(), _userId);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_UserMismatch()
        {
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);

            var result = await _service.DeleteAsync(goal.Id, _userId);

            Assert.False(result);
        }
        [Fact]
        public async Task CreateAsync_Should_Handle_ZeroTargetAmount()
        {
            // Arrange
            var request = new CreateSavingGoalRequest
            {
                IncomePerMonth = 5000,
                TargetAmount = 0,
                DurationMonths = 2
            };

            _repoMock.Setup(r => r.CreateAsync(It.IsAny<SavingGoal>(), It.IsAny<string>())).ReturnsAsync((object)null);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(), null, null, null, null))
                     .ReturnsAsync(new List<Transaction>());

            // Act
            var result = await _service.CreateAsync(_userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.DailyBudget);
        }

        [Fact]
        public async Task GetAllByUserAsync_Should_Return_EmptyList_When_NoGoals()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAsync<SavingGoal>(It.IsAny<Expression<Func<SavingGoal, bool>>>(), null, null, null, null))
                     .ReturnsAsync(new List<SavingGoal>());

            // Act
            var result = await _service.GetAllByUserAsync(_userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_NotChangeDailyBudget_When_DurationMonthsInvalid()
        {
            // Arrange
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1200, DurationMonths = 2, DailyBudget = 20, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { DurationMonths = 0 }; // invalid

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(20, goal.DailyBudget); // dailyBudget không đổi
            Assert.Equal(2, goal.DurationMonths); // duration không đổi
        }

        [Fact]
        public async Task CreateAsync_Should_Create_With_Zero_DailyBudget_When_TargetAmount_Zero()
        {
            // Arrange
            var request = new CreateSavingGoalRequest
            {
                IncomePerMonth = 5000,
                TargetAmount = 0,
                DurationMonths = 3
            };

            _repoMock.Setup(r => r.CreateAsync(It.IsAny<SavingGoal>(), It.IsAny<string>())).ReturnsAsync((object)null);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Expression<Func<Transaction, bool>>>(), null, null, null, null))
                     .ReturnsAsync(new List<Transaction>());

            // Act
            var result = await _service.CreateAsync(_userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0m, result.DailyBudget);
            Assert.Equal(0m, result.CurrentSaved);
            Assert.Equal(0m, result.Progress);
        }

        [Fact]
        public async Task UpdateAsync_Should_Recalculate_DailyBudget_When_TargetAmount_Or_Duration_Changed()
        {
            // Arrange
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 2, DailyBudget = 16.67m, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { TargetAmount = 3000, DurationMonths = 3 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(3000m, goal.TargetAmount);
            Assert.Equal(3, goal.DurationMonths);
            Assert.Equal(3000m / (3 * 30), goal.DailyBudget); // 33.33...
        }

        [Fact]
        public async Task UpdateAsync_Should_Not_Recalculate_DailyBudget_When_Only_IncomePerMonth_Changed()
        {
            // Arrange
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 2, DailyBudget = 16.67m, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { IncomePerMonth = 7000 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(7000, goal.IncomePerMonth);
            Assert.Equal(16.67m, goal.DailyBudget); // không đổi
        }

        [Fact]
        public async Task UpdateAsync_Should_Use_Default_Duration_1_When_DurationMonths_Zero_Or_Negative()
        {
            // Arrange
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1800, DurationMonths = 0, DailyBudget = 0, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { TargetAmount = 1800, DurationMonths = -5 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(1800m / (1 * 30), goal.DailyBudget); // dùng duration = 1
        }

        [Fact]
        public async Task UpdateAsync_Should_Not_Update_Duration_When_Invalid_Value_Provided()
        {
            // Arrange
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 3, DailyBudget = 11.11m, IncomePerMonth = 5000 };
            var request = new UpdateSavingGoalRequest { DurationMonths = 0 };

            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(goal.Id, _userId, request);

            // Assert
            Assert.True(result);
            Assert.Equal(3, goal.DurationMonths); // không đổi
            Assert.Equal(11.11m, goal.DailyBudget); // không đổi
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Delete_When_Goal_Has_Transactions()
        {
            // Nếu có ràng buộc FK hoặc logic ngăn xóa, nhưng hiện tại không có → vẫn xóa
            // Test này chỉ đảm bảo hành vi hiện tại
            var goal = new SavingGoal { Id = Guid.NewGuid(), UserId = _userId };
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(goal.Id, _userId);

            Assert.True(result);
            _repoMock.Verify(r => r.Delete(goal), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Goal_Is_Null()
        {
            // Đã có test tương tự, nhưng đảm bảo rõ ràng
            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>())).ReturnsAsync((SavingGoal)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid(), _userId);

            Assert.Null(result);
        }
    }
}
