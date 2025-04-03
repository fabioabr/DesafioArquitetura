using Xunit;
using Moq;
using FinancialServices.Application.Security.UseCase;
using FinancialServices.Domain.Core.Contracts;
using FinancialServices.Domain.Security.Contract;
using FinancialServices.Domain.Security.Entity;
using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Shared;
using Microsoft.Extensions.Logging;
using System.Linq;
using AutoMapper;

namespace FinancialServices.Tests
{
    public class AuthUserUseCaseTests
    {
        private readonly Mock<IRepository<UserEntity>> _mockUserRepository;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;
        private readonly AuthUserUseCase _useCase;

        public AuthUserUseCaseTests()
        {
            _mockUserRepository = new Mock<IRepository<UserEntity>>();
            _mockLogger = new Mock<ILogger>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();

            _useCase = new AuthUserUseCase(
                _mockUserRepository.Object,
                _mockAuthenticationService.Object,
                _mockAuthorizationService.Object,
                _mockLogger.Object,
                _mockMapper.Object);
        }

        [Fact]
        public void Execute_AuthenticationFailed_ReturnsFail()
        {
            // Arrange
            string apiKey = "testApiKey";
            string[] requiredRoles = { "Admin" };
            _mockAuthenticationService.Setup(auth => auth.AuthenticateByApiKey(apiKey)).Returns(false);

            // Act
            var result = _useCase.Execute(apiKey, requiredRoles);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Não foi possivel autenticar o usuário pela API KEY informada.", result.Message);
        }

        [Fact]
        public void Execute_AuthorizationFailed_ReturnsFail()
        {
            // Arrange
            string apiKey = "testApiKey";
            string[] requiredRoles = { "Admin" };
            _mockAuthenticationService.Setup(auth => auth.AuthenticateByApiKey(apiKey)).Returns(true);
            _mockAuthorizationService.Setup(auth => auth.AuthorizeUserByApiKey(apiKey, requiredRoles)).Returns(new GenericResponse().WithFail().WithMessage("Unauthorized") );

            // Act
            var result = _useCase.Execute(apiKey, requiredRoles);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unauthorized", result.Message);
        }

        [Fact]
        public void Execute_ValidUser_ReturnsSuccessWithData()
        {
            // Arrange
            string apiKey = "testApiKey";
            string[] requiredRoles = { "Admin" };
            var userEntity = new UserEntity { ApiKey = apiKey, UserName = "testUser" };
            var userModel = new UserModel { UserName = "testUser" };

            _mockAuthenticationService.Setup(auth => auth.AuthenticateByApiKey(apiKey)).Returns(true);
            _mockAuthorizationService.Setup(auth => auth.AuthorizeUserByApiKey(apiKey, requiredRoles)).Returns(new GenericResponse().WithSuccess());
            _mockUserRepository.Setup(repo => repo.Query()).Returns(new[] { userEntity }.AsQueryable());
            _mockMapper.Setup(mapper => mapper.Map<UserModel>(userEntity)).Returns(userModel);

            // Act
            var result = _useCase.Execute(apiKey, requiredRoles);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("testUser", result.Data.UserName);
            Assert.Equal("User Authenticated!", result.Message);
        }
    }
}