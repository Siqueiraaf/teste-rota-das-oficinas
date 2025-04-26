using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.Auth.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IIdentityAbstractor> _identityAbstractorMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly LoginCommandHandler _sut;

        public LoginCommandHandlerTests()
        {
            _sut = new LoginCommandHandler(_identityAbstractorMock.Object, _configurationMock.Object);
        }

        [Fact(DisplayName = "Given user not found should return empty LoginResponse")]
        public async Task Handle_WhenUserNotFound_ShouldReturnEmptyLoginResponse()
        {
            // Arrange
            _identityAbstractorMock
                .Setup(x => x
                .FindUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((Domain.Entities.User?)null);

            var command = new LoginCommand
            {
                Username = "nonexistent",
                Password = "somepassword"
            };

            // Act
            var response = await _sut.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().BeNull();
            response.RefreshToken.Should().BeNull();
            response.Roles.Should().BeNull();
            response.IssuedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
            response.ExpirationDate.Should().Be(default);
        }

        [Fact(DisplayName = "Given valid credentials should return a valid LoginResponse with token")]
        public async Task HandleWhenCredentialsAreValidShouldReturnLoginResponseWithToken()
        {
            // Arrange
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "validuser",
                Name = "Valid User"
            };

            _identityAbstractorMock
                .Setup(x => x.FindUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _identityAbstractorMock
                .Setup(x => x.PasswordSignInAsync(user, It.IsAny<string>()))
                .ReturnsAsync(SignInResult.Success); // Aqui usamos o objeto pronto

            _identityAbstractorMock
                .Setup(x => x.GetUserRolesAsync(user))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("this_is_a_super_secret_key_1234567890");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            var command = new LoginCommand
            {
                Username = "validuser",
                Password = "validpassword"
            };

            // Act
            var response = await _sut.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().NotBeNullOrEmpty();
            response.Roles.Should().Contain(new[] { "User", "Admin" });
            response.IssuedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
            response.ExpirationDate.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact(DisplayName = "Given wrong password should return empty LoginResponse")]
        public async Task Handle_WhenPasswordIsIncorrect_ShouldReturnEmptyLoginResponse()
        {
            // Arrange
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "validuser",
                Name = "Valid User"
            };

            _identityAbstractorMock
                .Setup(x => x.FindUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _identityAbstractorMock
                .Setup(x => x.PasswordSignInAsync(user, It.IsAny<string>()))
                .ReturnsAsync(SignInResult.Failed); // Password errado!

            var command = new LoginCommand
            {
                Username = "validuser",
                Password = "wrongpassword"
            };

            // Act
            var response = await _sut.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().BeNull();
            response.RefreshToken.Should().BeNull();
            response.Roles.Should().BeNull();
            response.IssuedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
            response.ExpirationDate.Should().Be(default);
        }
    }
}
