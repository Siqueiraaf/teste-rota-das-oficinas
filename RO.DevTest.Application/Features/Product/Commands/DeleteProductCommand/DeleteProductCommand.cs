using MediatR;
using System;

namespace RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;

/// <summary>
/// Command for deleting a product
/// </summary>
public class DeleteProductCommand : IRequest<bool>
{
    /// <summary>
    /// Unique identifier of the product to delete
    /// </summary>
    public Guid Id { get; set; }
}
