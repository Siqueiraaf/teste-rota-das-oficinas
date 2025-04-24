namespace RO.DevTest.Application.Features.User.Queries.GetUserByIdQuery;

public record UserDto(
    Guid Id,
    string Name,
    string? Email
);