using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UpdateUserCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnUpdatedUserResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new Domain.Entities.User
        {
            Id = userId.ToString(),
            Name = "Old Name",
            Email = "oldemail@example.com",
            PasswordHash = "oldpasswordhash"
        };

        var updateCommand = new UpdateUserCommand
        {
            Id = userId.ToString(),
            Name = "New Name",
            Email = "newemail@example.com",
            Password = "newpasswordhash"
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId.ToString()))
            .ReturnsAsync(existingUser);

        _mockUserRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(updateCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId.ToString());
        result.Name.Should().Be("New Name");
        result.Email.Should().Be("newemail@example.com");
        result.Password.Should().Be("newpasswordhash");

        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }


    [Fact]
    public async Task Handle_NonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateCommand = new UpdateUserCommand
        {
            Id = userId.ToString(),
            Name = "New Name",
            Email = "newemail@example.com",
            Password = "newpasswordhash"
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(updateCommand, CancellationToken.None));
    }
}
