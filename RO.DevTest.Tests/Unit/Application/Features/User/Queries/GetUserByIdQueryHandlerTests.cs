using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserByIdQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingUser_ShouldReturnUserDto()
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

        var query = new GetUserByIdQuery { Id = userId }; // Convertendo Guid para string

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId.ToString()); // Assumindo que o DTO também usa string para Id
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_NonExistingUser_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        var query = new GetUserByIdQuery { Id = userId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
