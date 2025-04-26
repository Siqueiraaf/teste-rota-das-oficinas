using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

namespace RO.DevTest.Tests.Unit.Application.Features.Product.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidProduct_ShouldCreateProductAndReturnResult()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 10.99m,
            StockQuantity = 100
        };

        _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Product>()))
            .ReturnsAsync((Domain.Entities.Product product) => product);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Description.Should().Be(command.Description);
        result.Price.Should().Be(command.Price);
        result.StockQuantity.Should().Be(command.StockQuantity);

        _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
    }
}
