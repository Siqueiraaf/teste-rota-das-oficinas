using System;

namespace RO.DevTest.Application.Features.Product.Commands.CreateProductCommand;

/// <summary>
/// Result returned after creating a product
/// </summary>
public class CreateProductResult
{
    /// <summary>
    /// Unique identifier of the created product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Quantity available in stock
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Date and time when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
