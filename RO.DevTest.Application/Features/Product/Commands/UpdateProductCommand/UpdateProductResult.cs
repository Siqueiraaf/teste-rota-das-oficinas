using System;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;

/// <summary>
/// Result returned after updating a product
/// </summary>
public class UpdateProductResult
{
    /// <summary>
    /// Unique identifier of the updated product
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Updated name of the product
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Updated description of the product
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Updated price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Updated stock quantity of the product
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
