using Moq;
using URLShortener.Application.DTOs.Auth;
using URLShortener.Application.Interfaces;
using URLShortener.Application.Services;
using URLShortener.Domain.Entities;
using URLShortener.Domain.Enums;

namespace URLShortener.Application.UnitTests.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public async Task LoginAsync_UserNotFound_ReturnsNull()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "Password123!"
            };

            userRepository
                .Setup(repository => repository.GetUserByEmailAsync(loginRequest.Email))
                .ReturnsAsync((User?)null);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.LoginAsync(loginRequest);

            Assert.IsNull(result);
            userRepository.Verify(repository => repository.GetUserByEmailAsync(loginRequest.Email), Times.Once);
            tokenService.Verify(serviceMock => serviceMock.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public void Constructor_ValidDependencies_CreatesInstance()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public async Task LoginAsync_InvalidPassword_ReturnsNull()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "Password123!"
            };
            var user = new User
            {
                Email = loginRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("DifferentPassword!"),
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            userRepository
                .Setup(repository => repository.GetUserByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.LoginAsync(loginRequest);

            Assert.IsNull(result);
            userRepository.Verify(repository => repository.GetUserByEmailAsync(loginRequest.Email), Times.Once);
            tokenService.Verify(serviceMock => serviceMock.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        [TestMethod]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var loginRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "Password123!"
            };
            var user = new User
            {
                Email = loginRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password),
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            userRepository
                .Setup(repository => repository.GetUserByEmailAsync(loginRequest.Email))
                .ReturnsAsync(user);
            tokenService
                .Setup(serviceMock => serviceMock.GenerateToken(user))
                .Returns("token-value");

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.LoginAsync(loginRequest);

            Assert.AreEqual("token-value", result);
            userRepository.Verify(repository => repository.GetUserByEmailAsync(loginRequest.Email), Times.Once);
            tokenService.Verify(serviceMock => serviceMock.GenerateToken(user), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_UserExists_ReturnsFalse()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var registerRequest = new RegisterRequest
            {
                Email = "existing@example.com",
                Password = "Password123!",
                RegisterAsAdmin = true
            };

            userRepository
                .Setup(repository => repository.UserExistsByEmailAsync(registerRequest.Email))
                .ReturnsAsync(true);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.RegisterAsync(registerRequest);

            Assert.IsFalse(result);
            userRepository.Verify(repository => repository.UserExistsByEmailAsync(registerRequest.Email), Times.Once);
            userRepository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Never);
            userRepository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public async Task RegisterAsync_NewUser_AddsUserAndSavesChanges()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var registerRequest = new RegisterRequest
            {
                Email = "new@example.com",
                Password = "Password123!",
                RegisterAsAdmin = true
            };
            User? addedUser = null;

            userRepository
                .Setup(repository => repository.UserExistsByEmailAsync(registerRequest.Email))
                .ReturnsAsync(false);
            userRepository
                .Setup(repository => repository.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(user => addedUser = user)
                .Returns(Task.CompletedTask);
            userRepository
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.RegisterAsync(registerRequest);

            Assert.IsTrue(result);
            Assert.IsNotNull(addedUser);
            Assert.AreEqual(registerRequest.Email, addedUser.Email);
            Assert.AreEqual(Role.Admin, addedUser.Role);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(registerRequest.Password, addedUser.PasswordHash));
            Assert.AreNotEqual(default, addedUser.CreatedAt);
            Assert.AreNotEqual(default, addedUser.UpdatedAt);
            userRepository.Verify(repository => repository.UserExistsByEmailAsync(registerRequest.Email), Times.Once);
            userRepository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Once);
            userRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task RegisterAsync_RegisterAsUser_SetsUserRole()
        {
            var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            var tokenService = new Mock<ITokenService>(MockBehavior.Strict);
            var registerRequest = new RegisterRequest
            {
                Email = "user@example.com",
                Password = "Password123!",
                RegisterAsAdmin = false
            };
            User? addedUser = null;

            userRepository
                .Setup(repository => repository.UserExistsByEmailAsync(registerRequest.Email))
                .ReturnsAsync(false);
            userRepository
                .Setup(repository => repository.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(user => addedUser = user)
                .Returns(Task.CompletedTask);
            userRepository
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new AuthService(userRepository.Object, tokenService.Object);

            var result = await service.RegisterAsync(registerRequest);

            Assert.IsTrue(result);
            Assert.IsNotNull(addedUser);
            Assert.AreEqual(Role.User, addedUser.Role);
            userRepository.Verify(repository => repository.UserExistsByEmailAsync(registerRequest.Email), Times.Once);
            userRepository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Once);
            userRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }
    }
}
