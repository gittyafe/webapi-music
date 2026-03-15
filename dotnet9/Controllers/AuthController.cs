using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MusicWebapi.Application.Services; // adjust namespace if needed
using MusicWebapi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Application.Services;
using System.Security.Claims;


namespace MusicWebapi.Api.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController : ControllerBase
{
    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest("Missing idToken");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch
        {
            return Unauthorized("Invalid Google token");
        }

        // בדוק אם המשתמש קיים ב-User.json לפי מייל
        var usersPath = Path.Combine(AppContext.BaseDirectory, "Data", "User.json");
        if (!System.IO.File.Exists(usersPath))
            return StatusCode(500, "Users data not found");
        var usersJson = System.IO.File.ReadAllText(usersPath);
        var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (users == null || !users.Any(u => string.Equals(u.Email, payload.Email, StringComparison.OrdinalIgnoreCase)))
            return Unauthorized("User not registered");
        var user = users.First(u => string.Equals(u.Email, payload.Email, StringComparison.OrdinalIgnoreCase));
        // var claims = new List<Claim>
        // {

        //       new Claim("userid", user.Id.ToString()),
        //         new Claim("username", user.Name),
        //         new Claim("password",user.Passwd),
        //         new Claim("role", user.Role),
        //         // Add NameIdentifier claim for SignalR user targeting
        //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //     new Claim(ClaimTypes.Name, payload.Name ?? ""),
        //     new Claim(ClaimTypes.Email, payload.Email ?? "")
        // };
        var claims = new List<Claim>
        {
            new Claim("userid", user.Id.ToString()),
            new Claim("username", user.Name),
            new Claim("password", user.Passwd),
            new Claim("role", user.Role),

            // חשוב ל‑SignalR — רק אחד!
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var token = TokenService.GetToken(claims);
        var jwt = TokenService.WriteToken(token);
        return Ok(jwt);

    }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; }
}
