using System;
using api.DTOs;
using api.Entities;
using api.Interfaces;

namespace api.Extensions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Username = user.UserName!,
            Token = tokenService.CreateToken(user)
        };
    }
}
