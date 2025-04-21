using RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;
using System.Collections.Generic;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductsQuery;

/// <summary>
/// View model for paginated product list results
/// </summary>
public class ProductsVm
{
    /// <summary>
    /// List of products in the current page
    /// </summary>
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();

    /// <summary>
    /// Total number of products matching the filter criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page available
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Whether there is a next page available
    /// </summary>
    public bool HasNextPage { get; set; }
}
