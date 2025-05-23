﻿using MediatR;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;

public class UpdateUserResult
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

