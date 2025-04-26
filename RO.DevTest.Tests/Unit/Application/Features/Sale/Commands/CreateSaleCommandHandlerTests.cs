using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;
using RO.DevTest.Domain.Enums;
using RO.DevTest.Domain.Exception;
using System.ComponentModel.DataAnnotations;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Commands;

public class CreateSaleCommandHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();

        _handler = new CreateSaleCommandHandler(
            _mockSaleRepository.Object,
            _mockProductRepository.Object,
            _mockIdentityAbstractor.Object);
    }

    [Fact]
    public async Task Handle_ValidSale_ShouldCreateSaleAndReturnResult()
    {
        // Arrange
        var userId = "user123";
        var productId = Guid.NewGuid();

        var user = new Domain.Entities.User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        var product = new Domain.Entities.Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 10.99m,
            StockQuantity = 100
        };

        var command = new CreateSaleCommand
        {
            UserId = userId,
            Items = new List<SaleItemDto>
            {
                new SaleItemDto
                {
                    ProductId = productId,
                    Quantity = 2
                }
            },
            PaymentMethod = PaymentMethod.CreditCard
        };

        _mockIdentityAbstractor.Setup(abstractor => abstractor.FindUserByIdAsync(userId))
            .ReturnsAsync(user);

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockSaleRepository.Setup(repo => repo.AddAsync(It.IsAny<Domain.Entities.Sale>()))
            .ReturnsAsync((Domain.Entities.Sale sale) => sale);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.TotalAmount.Should().Be(21.98m); // 10.99 * 2
        result.Items.Should().HaveCount(1);
        result.Items[0].ProductId.Should().Be(productId);
        result.Items[0].Quantity.Should().Be(2);
        result.Items[0].UnitPrice.Should().Be(10.99m);
        result.Items[0].Subtotal.Should().Be(21.98m);

        _mockSaleRepository.Verify(repo => repo.AddAsync(It.IsAny<Domain.Entities.Sale>()), Times.Once);
        _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Product>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = "nonexistent";

        var command = new CreateSaleCommand
        {
            UserId = userId,
            Items =
            [
                new SaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        _mockIdentityAbstractor.Setup(abstractor => abstractor.FindUserByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowValidationException()
    {
        // Arrange
        var userId = "user123";
        var productId = Guid.NewGuid();

        var user = new Domain.Entities.User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        var product = new Domain.Entities.Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 10.99m,
            StockQuantity = 1 // Only 1 in stock
        };

        var command = new CreateSaleCommand
        {
            UserId = userId,
            Items = new List<SaleItemDto>
            {
                new SaleItemDto
                {
                    ProductId = productId,
                    Quantity = 2 // Trying to buy 2
                }
            }
        };

        _mockIdentityAbstractor.Setup(abstractor => abstractor.FindUserByIdAsync(userId))
            .ReturnsAsync(user);

        _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
