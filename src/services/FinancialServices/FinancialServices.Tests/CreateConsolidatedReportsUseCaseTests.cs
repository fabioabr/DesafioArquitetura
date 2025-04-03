using AutoMapper;
using FinancialServices.Application.Financial.UseCase;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Model;
using FinancialServices.Utils.Shared;
using global::FinancialServices.Domain.Financial.Contract;
using global::FinancialServices.Domain.Financial.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FinancialServices.Tests
{
 
    public class CreateConsolidatedReportsUseCaseTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IGetConsolidatedReportUseCase> _mockGetConsolidatedReportUseCase;
        private readonly Mock<IRepository<TransactionGroupingEntity>> _mockTransactionGroupingRepository;
        private readonly Mock<IRepository<TransactionEntity>> _mockTransactionRepository;
        private readonly Mock<IOptions<ApplicationSettingsModel>> _mockSettings;
        private readonly CreateConsolidatedReportsUseCase _useCase;

        public CreateConsolidatedReportsUseCaseTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockMapper = new Mock<IMapper>();
            _mockGetConsolidatedReportUseCase = new Mock<IGetConsolidatedReportUseCase>();
            _mockTransactionGroupingRepository = new Mock<IRepository<TransactionGroupingEntity>>();
            _mockTransactionRepository = new Mock<IRepository<TransactionEntity>>();
            _mockSettings = new Mock<IOptions<ApplicationSettingsModel>>();

            _useCase = new CreateConsolidatedReportsUseCase(
                _mockLogger.Object,
                _mockMapper.Object,
                _mockGetConsolidatedReportUseCase.Object,
                _mockTransactionGroupingRepository.Object,
                _mockTransactionRepository.Object,
                _mockSettings.Object);
        }

        [Fact]
        public void CreateTransactionGroups_NoTransactions_ReturnsSuccessMessage()
        {
            // Arrange
            _mockTransactionRepository.Setup(repo => repo.Query()).Returns(new List<TransactionEntity>().AsQueryable());

            // Act
            var result = _useCase.CreateTransactionGroups(Array.Empty<TimeZoneInfo>());

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Has no report to be created", result.Message);
        }

        [Fact]
        public void CreateTransactionGroups_WithTransactions_ReturnsSuccess()
        {
            // Arrange
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow, Amount = 100, Type = TransactionTypeEnum.Credit },
                new TransactionEntity { Id = Guid.NewGuid(), Timestamp = DateTime.UtcNow.AddHours(-1), Amount = 50, Type = TransactionTypeEnum.Debit }
            }.AsQueryable();

            _mockTransactionRepository.Setup(repo => repo.Query()).Returns(transactions);
            _mockTransactionGroupingRepository.Setup(repo => repo.Query()).Returns(new List<TransactionGroupingEntity>().AsQueryable());
            _mockGetConsolidatedReportUseCase.Setup(u => u.InvalidateTransactionGroupingCache(It.IsAny<DateTime>(), It.IsAny<TimeZoneInfo>(), It.IsAny<bool>())).Returns(new GenericResponse().WithSuccess());
            _mockTransactionGroupingRepository.Setup(repo => repo.InsertOrUpdate(It.IsAny<TransactionGroupingEntity>()));

            // Act
            var result = _useCase.CreateTransactionGroups(new TimeZoneInfo[] { TimeZoneInfo.Utc });

            // Assert
            Assert.True(result.Success);
            _mockTransactionGroupingRepository.Verify(repo => repo.InsertOrUpdate(It.IsAny<TransactionGroupingEntity>()), Times.AtLeastOnce);
            _mockGetConsolidatedReportUseCase.Verify(u => u.InvalidateTransactionGroupingCache(It.IsAny<DateTime>(), It.IsAny<TimeZoneInfo>(), It.IsAny<bool>()), Times.AtLeastOnce);
        }

        [Fact]
        public void CreateTransactionGroups_ExceptionThrown_ReturnsFail()
        {
            // Arrange
            _mockTransactionRepository.Setup(repo => repo.Query()).Throws(new Exception("Test Exception"));

            // Act
            var result = _useCase.CreateTransactionGroups(Array.Empty<TimeZoneInfo>());

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Exception);
            Assert.StartsWith("Error creating Transaction Groups", result.Message);
        }
    }
}
