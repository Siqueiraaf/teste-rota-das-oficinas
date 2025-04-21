using System;

namespace RO.DevTest.Domain.Entities;

/// <summary>
/// Represents an item within a sale in the e-commerce system
/// </summary>
public class SaleItem
{
    /// <summary>
    /// Unique identifier for the sale item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the sale this item belongs to
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Reference to the parent sale
    /// </summary>
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// ID of the product being sold
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Reference to the product being sold
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Quantity of the product in this sale item
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time of sale
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this item (Quantity * UnitPrice)
    /// </summary>
    public decimal Subtotal { get; set; }
}
