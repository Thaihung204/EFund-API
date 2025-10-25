//using System;
//using EFund_API.Dtos;
//using EFund_API.Models;
//using EFund_API.Services;
//using EFund_API.WebApp.Services.Interfaces;
//using Moq;
//using NUnit.Framework;

//namespace EFund_API.Tests.UnitTests
//{
//    [TestFixture]
//    public class SavingGoalServiceTests
//    {
//        private Mock<IRepository> _repoMock;
//        private SavingGoalService _service;
//        private Guid _userId;

//        [SetUp]
//        public void Setup()
//        {
//            _repoMock = new Mock<IRepository>();
//            _service = new SavingGoalService(_repoMock.Object);
//            _userId = Guid.NewGuid();
//        }

//        [Test]
//        public async Task CreateAsync_Should_Return_Null_When_DurationMonths_Invalid()
//        {
//            var request = new CreateSavingGoalRequest
//            {
//                DurationMonths = 0,
//                TargetAmount = 1000,
//                IncomePerMonth = 5000
//            };

//            var result = await _service.CreateAsync(_userId, request);

//            Assert.IsNull(result);
//            _repoMock.Verify(r => r.CreateAsync(It.IsAny<SavingGoal>()), Times.Never);
//        }

//        [Test]
//        public async Task CreateAsync_Should_Create_Goal_Successfully()
//        {
//            // Arrange
//            var request = new CreateSavingGoalRequest
//            {
//                DurationMonths = 2,
//                TargetAmount = 6000,
//                IncomePerMonth = 10000
//            };

//            _repoMock.Setup(r => r.CreateAsync(It.IsAny<SavingGoal>())).Returns(Task.CompletedTask);
//            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
//            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Func<Transaction, bool>>()))
//                .ReturnsAsync(new List<Transaction>());

//            // Act
//            var result = await _service.CreateAsync(_userId, request);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(request.TargetAmount / (request.DurationMonths * 30), result.DailyBudget);
//            _repoMock.Verify(r => r.CreateAsync(It.IsAny<SavingGoal>()), Times.Once);
//            _repoMock.Verify(r => r.SaveAsync(), Times.Once);
//        }

//        [Test]
//        public async Task GetAllByUserAsync_Should_Return_List()
//        {
//            var goals = new List<SavingGoal>
//            {
//                new SavingGoal { Id = Guid.NewGuid(), UserId = _userId, TargetAmount = 1000, DurationMonths = 2 }
//            };

//            _repoMock.Setup(r => r.GetAsync<SavingGoal>(It.IsAny<Func<SavingGoal, bool>>()))
//                .ReturnsAsync(goals);
//            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Func<Transaction, bool>>()))
//                .ReturnsAsync(new List<Transaction>());

//            var result = await _service.GetAllByUserAsync(_userId);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(1, result.Count());
//        }

//        [Test]
//        public async Task GetByIdAsync_Should_Return_Null_If_Not_Found_Or_WrongUser()
//        {
//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync((SavingGoal?)null);

//            var result = await _service.GetByIdAsync(Guid.NewGuid(), _userId);
//            Assert.IsNull(result);

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync(new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() });

//            result = await _service.GetByIdAsync(Guid.NewGuid(), _userId);
//            Assert.IsNull(result);
//        }

//        [Test]
//        public async Task GetByIdAsync_Should_Return_Goal_When_Valid()
//        {
//            var goal = new SavingGoal
//            {
//                Id = Guid.NewGuid(),
//                UserId = _userId,
//                TargetAmount = 1000,
//                DurationMonths = 1
//            };

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
//            _repoMock.Setup(r => r.GetAsync<Transaction>(It.IsAny<Func<Transaction, bool>>()))
//                .ReturnsAsync(new List<Transaction>());

//            var result = await _service.GetByIdAsync(goal.Id, _userId);

//            Assert.IsNotNull(result);
//            Assert.AreEqual(goal.TargetAmount, result.TargetAmount);
//        }

//        [Test]
//        public async Task UpdateAsync_Should_Return_False_When_NotFound_Or_WrongUser()
//        {
//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync((SavingGoal?)null);

//            var result = await _service.UpdateAsync(Guid.NewGuid(), _userId, new UpdateSavingGoalRequest());
//            Assert.IsFalse(result);

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync(new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() });

//            result = await _service.UpdateAsync(Guid.NewGuid(), _userId, new UpdateSavingGoalRequest());
//            Assert.IsFalse(result);
//        }

//        [Test]
//        public async Task UpdateAsync_Should_Update_And_Save()
//        {
//            var goal = new SavingGoal
//            {
//                Id = Guid.NewGuid(),
//                UserId = _userId,
//                TargetAmount = 1000,
//                DurationMonths = 2
//            };

//            var request = new UpdateSavingGoalRequest
//            {
//                TargetAmount = 3000,
//                DurationMonths = 3
//            };

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
//            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

//            var result = await _service.UpdateAsync(goal.Id, _userId, request);

//            Assert.IsTrue(result);
//            Assert.AreEqual(request.TargetAmount.Value / (request.DurationMonths.Value * 30), goal.DailyBudget);
//            _repoMock.Verify(r => r.Update(goal), Times.Once);
//            _repoMock.Verify(r => r.SaveAsync(), Times.Once);
//        }

//        [Test]
//        public async Task DeleteAsync_Should_Return_False_When_NotFound_Or_WrongUser()
//        {
//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync((SavingGoal?)null);

//            var result = await _service.DeleteAsync(Guid.NewGuid(), _userId);
//            Assert.IsFalse(result);

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(It.IsAny<Guid>()))
//                .ReturnsAsync(new SavingGoal { Id = Guid.NewGuid(), UserId = Guid.NewGuid() });

//            result = await _service.DeleteAsync(Guid.NewGuid(), _userId);
//            Assert.IsFalse(result);
//        }

//        [Test]
//        public async Task DeleteAsync_Should_Delete_When_Valid()
//        {
//            var goal = new SavingGoal
//            {
//                Id = Guid.NewGuid(),
//                UserId = _userId
//            };

//            _repoMock.Setup(r => r.GetByIdAsync<SavingGoal>(goal.Id)).ReturnsAsync(goal);
//            _repoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

//            var result = await _service.DeleteAsync(goal.Id, _userId);

//            Assert.IsTrue(result);
//            _repoMock.Verify(r => r.Delete(goal), Times.Once);
//            _repoMock.Verify(r => r.SaveAsync(), Times.Once);
//        }
//    }
//}
