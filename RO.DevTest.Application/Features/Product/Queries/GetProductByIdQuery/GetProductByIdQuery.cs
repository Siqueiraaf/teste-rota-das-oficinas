using MediatR;
using System;

namespace RO.DevTest.Application.Features.Product.Queries.GetProductByIdQuery;

/// <summary>
/// Query for retrieving a product by its ID
/// </summary>
public class GetProductByIdQuery : IRequest<ProductDto>
{
    /// <summary>
    /// Unique identifier of the product to retrieve
    /// </summary>
    public Guid Id { get; set; }
}
