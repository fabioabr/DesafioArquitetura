using Xunit;
using Moq;
using FinancialServices.Application.Financial.UseCase;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Entity;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace FinancialServices.Tests
{
    public class GetConsolidatedReportUseCaseTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepository<TransactionGroupingEntity>> _mockTransactionGroupingRepository;
        private readonly GetConsolidatedReportUseCase _useCase;

        public GetConsolidatedReportUseCaseTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockTransactionGroupingRepository = new Mock<IRepository<TransactionGroupingEntity>>();
            _useCase = new GetConsolidatedReportUseCase(_mockMapper.Object, _mockTransactionGroupingRepository.Object);
        }

        [Fact]
        public void GetConsolidatedReport_FutureDate_ReturnsFail()
        {
            // Arrange
            var futureDate = DateTime.UtcNow.AddDays(1);
            var timezone = TimeZoneInfo.Utc;

            // Act
            var result = _useCase.GetConsolidatedReport(futureDate, timezone);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid Date", result.Message);
            Assert.IsType<InvalidDataException>(result.Exception);
        }

        [Fact]
        public void GetConsolidatedReport_ValidDate_ReturnsSuccessWithData()
        {
            // Arrange
            var validDate = DateTime.UtcNow.Date;
            var timezone = TimeZoneInfo.Utc;
            var groupingEntities = new List<TransactionGroupingEntity>
            {
                new TransactionGroupingEntity { Period = validDate, TransactionType = FinancialServices.Domain.Financial.Enum.TransactionTypeEnum.Credit, TotalAmount = 100, TransactionCount = 1 },
                new TransactionGroupingEntity { Period = validDate, TransactionType = FinancialServices.Domain.Financial.Enum.TransactionTypeEnum.Debit, TotalAmount = 50, TransactionCount = 1 }
            };
            var groupingModels = new List<TransactionGroupingModel>
            {
                new TransactionGroupingModel { Period = validDate, TransactionType = FinancialServices.Domain.Financial.Enum.TransactionTypeEnum.Credit, TotalAmount = 100, TransactionCount = 1 },
                new TransactionGroupingModel { Period = validDate, TransactionType = FinancialServices.Domain.Financial.Enum.TransactionTypeEnum.Debit, TotalAmount = 50, TransactionCount = 1 }
            };

            _mockTransactionGroupingRepository.Setup(repo => repo.Query()).Returns(groupingEntities.AsQueryable());
            _mockMapper.Setup(mapper => mapper.Map<List<TransactionGroupingModel>>(groupingEntities)).Returns(groupingModels);

            // Act
            var result = _useCase.GetConsolidatedReport(validDate, timezone);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public void InvalidateTransactionGroupingCache_ValidCall_ReturnsSuccess()
        {
            // Arrange
            var validDate = DateTime.UtcNow.Date;
            var timezone = TimeZoneInfo.Utc;
            bool eraseAllRecordsOfDay = false;

            // Act
            var result = _useCase.InvalidateTransactionGroupingCache(validDate, timezone, eraseAllRecordsOfDay);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void InvalidateTransactionGroupingCache_ExceptionThrown_ReturnsFail()
        {
            // Arrange
            var validDate = DateTime.UtcNow.Date;
            var timezone = TimeZoneInfo.Utc;
            bool eraseAllRecordsOfDay = false;

            //Forcing an error by sending null to cacheaspect.cache.invalidate
            FinancialServices.Utils.Cache.CacheAspect.Cache = null;

            // Act
            var result = _useCase.InvalidateTransactionGroupingCache(validDate, timezone, eraseAllRecordsOfDay);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Exception);
        }
    }
}