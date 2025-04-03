using FinancialServices.Domain.Financial.Enum;
using FinancialServices.Domain.Financial.Model;
using FinancialServices.Domain.Financial.Service;

namespace FinancialServices.Tests
{
    public class TransactionValidationServiceTests
    {
        private readonly TransactionValidationService _validationService;

        public TransactionValidationServiceTests()
        {
            _validationService = new TransactionValidationService();
        }

        [Fact]
        public void ValidateTransaction_ValidTransaction_DoesNotThrowException()
        {
            // Arrange
            var validTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "AccountA",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Credit, // Usando Credit como exemplo
                OriginalTransactionId = null
            };

            // Act & Assert
            _validationService.ValidateTransaction(validTransaction); // Should not throw an exception
        }

        [Fact]
        public void ValidateTransaction_AmountIsZero_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = 0,
                SourceAccount = "AccountA",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Credit,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_AmountIsNegative_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = -10,
                SourceAccount = "AccountA",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Credit,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_SourceAccountIsEmpty_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Credit,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_DestinationAccountIsEmpty_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "AccountA",
                DestinationAccount = "",
                Type = TransactionTypeEnum.Credit,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_SourceAccountEqualsDestinationAccount_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "SameAccount",
                DestinationAccount = "SameAccount",
                Type = TransactionTypeEnum.Credit,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_RefundTypeWithoutOriginalTransactionId_ThrowsException()
        {
            // Arrange
            var invalidTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "AccountA",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Refund,
                OriginalTransactionId = null
            };

            // Act & Assert
            Assert.Throws<Exception>(() => _validationService.ValidateTransaction(invalidTransaction));
        }

        [Fact]
        public void ValidateTransaction_RefundTypeWithOriginalTransactionId_DoesNotThrowException()
        {
            // Arrange
            var validTransaction = new TransactionModel
            {
                Amount = 100,
                SourceAccount = "AccountA",
                DestinationAccount = "AccountB",
                Type = TransactionTypeEnum.Refund,
                OriginalTransactionId = Guid.NewGuid() // Usando Guid para OriginalTransactionId
            };

            // Act & Assert
            _validationService.ValidateTransaction(validTransaction); // Should not throw an exception
        }
    }
}
