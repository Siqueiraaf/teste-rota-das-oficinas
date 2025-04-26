using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Sale.Commands.CreateSaleCommand;
using RO.DevTest.Application.Features.Sale.Queries.GetSaleByIdQuery;
using RO.DevTest.Application.Features.Sale.Queries.GetSalesAnalysisQuery;
using RO.DevTest.Application.Features.Sale.Queries.GetSalesQuery;
using System;
using System.Threading.Tasks;

namespace RO.DevTest.WebApi.Controllers;

/// <summary>
/// Controller for sale-related operations
/// </summary>
[ApiController]
[Route("api/sales")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of sales with optional filtering and sorting
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SalesVm), StatusCodes.Status200OK)]
    public async Task<ActionResult<SalesVm>> GetSales([FromQuery] GetSalesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets a sale by its ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SaleDto>> GetSaleById(Guid id)
    {
        var query = new GetSaleByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateSaleResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSaleResult>> CreateSale([FromBody] CreateSaleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSaleById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets sales analysis data for a specific period
    /// </summary>
    [HttpGet("analysis")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SalesAnalysisVm), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SalesAnalysisVm>> GetSalesAnalysis([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetSalesAnalysisQuery
        {
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
