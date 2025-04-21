using System;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;

/// <summary>
/// Data transfer object for product information
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Unique identifier of the product
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

    /// <summary>
    /// Date and time when the product was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
