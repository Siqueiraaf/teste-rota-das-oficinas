using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainUser = RO.DevTest.Domain.Entities.User;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityAbstractor _identityAbstractor;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IIdentityAbstractor identityAbstractor, IConfiguration configuration)
    {
        _identityAbstractor = identityAbstractor;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityAbstractor.FindUserByUsernameAsync(request.Username);

        if (user == null)
        {
            return new LoginResponse();
        }

        var result = await _identityAbstractor.PasswordSignInAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new LoginResponse();
        }

        var roles = await _identityAbstractor.GetUserRolesAsync(user);

        var (token, expiration) = GenerateJwtToken(user, roles);

        return new LoginResponse
        {
            AccessToken = token,
            RefreshToken = Guid.NewGuid().ToString(),
            Roles = roles,
            ExpirationDate = expiration
        };
    }

    private (string token, DateTime expiration) GenerateJwtToken(DomainUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("username", user.UserName ?? string.Empty),
            new("name", user.Name ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "defaultSecretKeyForDevelopment"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(5);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
}
