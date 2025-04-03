using AutoMapper;
using FinancialServices.Application.Financial.UseCase;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Financial.Event;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Financial.Service;
using Moq;

namespace FinancialServices.Tests
{
    public class CreateTransactionUseCaseTests
    {
        [Fact]
        public void CreateTransaction_ValidTransaction_ReturnsSuccessResponse()
        {
            // Arrange
            var mockValidationService = new Mock<ITransactionValidationService>();
            var mockMapper = new Mock<IMapper>();
            var mockEventPublisher = new Mock<IEventPublisher<TransactionCreatedEventModel>>();

            var transaction = new TransactionModel();
            var transactionEntity = new TransactionEntity();
            var transactionCreatedEvent = new TransactionCreatedEventModel();

            mockMapper.Setup(m => m.Map<TransactionEntity>(transaction)).Returns(transactionEntity);
            mockMapper.Setup(m => m.Map<TransactionCreatedEventModel>(transaction)).Returns(transactionCreatedEvent);

            var useCase = new CreateTransactionUseCase(mockValidationService.Object, mockEventPublisher.Object, mockMapper.Object);

            // Act
            var result = useCase.CreateTransaction(transaction);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(transaction, result.Data);
            mockValidationService.Verify(v => v.ValidateTransaction(transaction), Times.Once);
            mockEventPublisher.Verify(p => p.Publish(transactionCreatedEvent), Times.Once);
        }

        [Fact]
        public void CreateTransaction_InvalidTransaction_ReturnsErrorResponse()
        {
            // Arrange
            var mockValidationService = new Mock<ITransactionValidationService>();
            var mockMapper = new Mock<IMapper>();
            var mockEventPublisher = new Mock<IEventPublisher<TransactionCreatedEventModel>>();

            var transaction = new TransactionModel();
            var exception = new Exception("Invalid transaction");

            mockValidationService.Setup(v => v.ValidateTransaction(transaction)).Throws(exception);

            var useCase = new CreateTransactionUseCase(mockValidationService.Object, mockEventPublisher.Object, mockMapper.Object);

            // Act
            var result = useCase.CreateTransaction(transaction);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(transaction, result.Data);
            Assert.Equal(exception.Message, result.Message);
            Assert.Equal(exception, result.Exception);
            mockEventPublisher.Verify(p => p.Publish(It.IsAny<TransactionCreatedEventModel>()), Times.Never);
        }
    }
}