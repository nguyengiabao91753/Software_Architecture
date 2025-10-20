using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shares.Base.Auth;
using Shares.Core.Dtos;
using System.Security.Claims;

namespace AuthAPI.Controllers;
[Route("api/auth/")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthAPIService _authService;
    public AuthController(IAuthAPIService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
    {
        var response = await _authService.Register(registrationRequestDto);
        if (response.IsSuccess)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var response = await _authService.Login(loginRequestDto);
        if (!response.IsSuccess)
        {
            return Unauthorized(response);
        }
        return Ok(response);
    }
}
