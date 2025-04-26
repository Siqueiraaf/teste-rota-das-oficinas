using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Tests.Unit.Application.Features.Sale.Queries;

public class GetSaleByIdQueryHandlerTests
{
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
    private readonly GetSaleByIdQueryHandler _handler;

    public GetSaleByIdQueryHandlerTests()
    {
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
        _handler = new GetSaleByIdQueryHandler(_mockSaleRepository.Object, _mockIdentityAbstractor.Object);
    }

    [Fact]
    public async Task Handle_ExistingSale_ShouldReturnSaleDto()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var sale = new Domain.Entities.Sale
        {
            Id = saleId,
            UserId = userId.ToString(),
            TotalAmount = 150.0m,
            SaleDate = DateTime.UtcNow,
            Items =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.0m,
                    Subtotal = 100.0m,
                    Product = new Domain.Entities.Product { Name = "Product A" }
                },
                new() {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 1,
                    UnitPrice = 50.0m,
                    Subtotal = 50.0m,
                    Product = null! // simula produto não encontrado
                }
            ]
        };

        var user = new Domain.Entities.User
        {
            Id = userId.ToString(),
            Name = "Test User"
        };

        _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync(sale);
        _mockIdentityAbstractor.Setup(i => i.FindUserByIdAsync(userId.ToString())).ReturnsAsync(user);

        var query = new GetSaleByIdQuery { Id = saleId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.UserId.Should().Be(userId.ToString());
        result.UserName.Should().Be(user.Name);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.Items.Should().HaveCount(2);
        result.Items.First().ProductName.Should().Be("Product A");
        result.Items.Last().ProductName.Should().Be("Unknown Product");
    }

    [Fact]
    public async Task Handle_NonExistingSale_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _mockSaleRepository.Setup(r => r.GetByIdAsync(saleId)).ReturnsAsync((Domain.Entities.Sale?)null);
        var query = new GetSaleByIdQuery { Id = saleId };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}
