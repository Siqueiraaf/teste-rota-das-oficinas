using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.User.Commands.DeleteUserCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnTrueAndDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Domain.Entities.User
        {
            Id = userId.ToString(),
            Name = "Test User",
            Email = "testuser@example.com"
        };

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(repo => repo.DeleteAsync(user))
            .Returns(Task.CompletedTask);  // Simulando que a exclusão ocorreu sem erro.

        var command = new DeleteUserCommand { Id = userId }; // Convertendo Guid para string

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue(); // Esperamos que o comando retorne true
        _mockUserRepository.Verify(repo => repo.DeleteAsync(user), Times.Once); // Verifica que o método DeleteAsync foi chamado uma vez
    }

    [Fact]
    public async Task Handle_NonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null); // Simulando que o usuário não foi encontrado

        var command = new DeleteUserCommand { Id = userId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
