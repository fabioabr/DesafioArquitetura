using Xunit;
using Moq;
using FinancialServices.Application.Financial.UseCase;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Utils.Shared;
using System;
using System.Threading.Tasks;
using AutoMapper;

namespace FinancialServices.Tests
{
    public class ProcessTransactionUseCaseTests
    {
        private readonly Mock<IRepository<TransactionEntity>> _mockTransactionRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProcessTransactionUseCase _useCase;

        public ProcessTransactionUseCaseTests()
        {
            _mockTransactionRepository = new Mock<IRepository<TransactionEntity>>();
            _mockMapper = new Mock<IMapper>();
            _useCase = new ProcessTransactionUseCase(_mockTransactionRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task ProcessTransaction_ValidTransaction_ReturnsSuccess()
        {
            // Arrange
            var transactionEvent = new TransactionCreatedEventModel();
            var transactionEntity = new TransactionEntity();

            _mockMapper.Setup(mapper => mapper.Map<TransactionEntity>(transactionEvent)).Returns(transactionEntity);
            _mockTransactionRepository.Setup(repo => repo.InsertOrUpdate(transactionEntity)).Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ProcessTransaction(transactionEvent);

            // Assert
            Assert.True(result.Success);
            _mockTransactionRepository.Verify(repo => repo.InsertOrUpdate(transactionEntity), Times.Once);
        }

        [Fact]
        public async Task ProcessTransaction_ExceptionThrown_ReturnsFail()
        {
            // Arrange
            var transactionEvent = new TransactionCreatedEventModel();
            var exception = new Exception("Test Exception");

            _mockMapper.Setup(mapper => mapper.Map<TransactionEntity>(transactionEvent)).Throws(exception);

            // Act
            var result = await _useCase.ProcessTransaction(transactionEvent);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Test Exception", result.Message);
            Assert.Equal(exception, result.Exception);
        }
    }
}