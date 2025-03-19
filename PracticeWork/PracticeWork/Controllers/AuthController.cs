using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace PracticeWork.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthBL _authBL;

    public AuthController(IAuthBL authBL)
    {
        _authBL = authBL;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerDto)
    {
        var user = await _authBL.RegisterUserAsync(registerDto);
        if (user == null) return BadRequest("User already exists.");

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        var token = await _authBL.LoginUserAsync(loginDto);
        if (token == null) return Unauthorized("Invalid credentials.");

        return Ok(new { Token = token });
    }

    // POST: /api/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email))
            return BadRequest("Email is required.");

        var result = await _authBL.ForgotPasswordAsync(model.Email);
        return Ok(new { message = result });
    }

    // POST: /api/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
    {
        if (model == null || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            return BadRequest("Token and new password are required.");

        var result = await _authBL.ResetPasswordAsync(model.Token, model.NewPassword);
        return Ok(new { message = result });
    }
}
