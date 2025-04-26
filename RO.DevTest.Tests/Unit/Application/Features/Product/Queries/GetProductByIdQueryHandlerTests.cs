using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Queries;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductByIdQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingProduct_ShouldReturnProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Domain.Entities.Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            StockQuantity = 100,
            CreatedAt = DateTime.UtcNow
        };

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var query = new GetProductByIdQuery { Id = productId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
        result.Description.Should().Be(product.Description);
        result.Price.Should().Be(product.Price);
        result.StockQuantity.Should().Be(product.StockQuantity);
        result.CreatedAt.Should().Be(product.CreatedAt);
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ShouldThrowNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Domain.Entities.Product?)null);

        var query = new GetProductByIdQuery { Id = productId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
