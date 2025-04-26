using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

namespace RO.DevTest.WebApi.Controllers;

[Route("api/auth")]
[OpenApiTags("Auth")]
public class AuthController(IMediator mediator) : Controller {
    private readonly IMediator _mediator = mediator;

    ///[TODO] - CREATE LOGIN HANDLER HERE

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        return Ok(result);
    }
}
