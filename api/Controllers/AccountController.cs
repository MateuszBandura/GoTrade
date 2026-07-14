using api.DTOs;
using api.Entities;
using api.Extensions;
using api.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class AccountController(UserManager<AppUser> userManager, 
                            SignInManager<AppUser> signInManager, 
                            ITokenService tokenService) : BaseApiController
{
    [HttpPost("Register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Trainer = new Trainer
            {
                FriendCode = dto.FriendCode
            }
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if(!result.Succeeded) return Unauthorized("Invalid credentials");

        return user.ToDto(tokenService);
    }
}
