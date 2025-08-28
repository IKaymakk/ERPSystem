using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Auth;
using ERPSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

  
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("refreshToken", response.RefreshToken, cookieOptions);

            return Ok(new
            {
                AccessToken = response.AccessToken,
                ExpiresAt = response.ExpiresAt,
                User = response.User,
                Message = "Giriş başarılı."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Token yenileme
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto? requestDto = null)
    {
        try
        {
            var refreshToken = requestDto?.RefreshToken ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { Message = "Refresh token bulunamadı." });

            var response = await _authService.RefreshTokenAsync(refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(30)
            };

            Response.Cookies.Append("refreshToken", response.RefreshToken, cookieOptions);

            return Ok(new
            {
                AccessToken = response.AccessToken,
                ExpiresAt = response.ExpiresAt,
                Message = "Token yenilendi."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutDto? logoutDto = null)
    {
        try
        {
            var refreshToken = logoutDto?.RefreshToken ?? Request.Cookies["refreshToken"];

            await _authService.LogoutAsync(refreshToken ?? string.Empty);

            Response.Cookies.Delete("refreshToken");

            return Ok(new { Message = "Çıkış başarılı." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Mevcut kullanıcı bilgilerini getir
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
                return BadRequest(new { Message = "Geçersiz kullanıcı bilgisi." });

            var user = await _authService.GetCurrentUserAsync(userId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    [HttpGet("validate-token")]
    [Authorize]
    public IActionResult ValidateToken()
    {
        return Ok(new
        {
            Message = "Token geçerli.",
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = User.FindFirst(ClaimTypes.Name)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}