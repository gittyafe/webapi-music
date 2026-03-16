using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Application.Services;
using System.Security.Claims;
using Google.Apis.Auth;

namespace MusicWebapi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IGenericRepository<User> service;

    public LoginController(IGenericRepository<User> service)
    {
        this.service = service;
    }

    // לא דורש טוקן
    [AllowAnonymous]
    [HttpPost]
    public ActionResult<string> Login([FromBody] LoginRequest req)
    {
        var user = service
            .Get()
            .FirstOrDefault(u => u.Name == req.Name && u.Passwd == req.Passwd);

        if (user == null)
            return NotFound("Wrong userId / password");

        return CreateToken(user);
    }



    [HttpPost("google-login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BadRequest("Missing idToken");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch (Exception ex)
        {
            return Unauthorized($"Internal Error: {ex.Message}");
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

        return CreateToken(user);
    }


    private ActionResult<string> CreateToken(User user)
    {

        var claims = new List<Claim>
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Name??""),
                new Claim("role", user.Role ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

        var token = TokenService.GetToken(claims);
        return Ok(TokenService.WriteToken(token));
    }
}

public class LoginRequest
{
    public string? Name { get; set; }
    public string? Passwd { get; set; }
}
public class GoogleLoginRequest
{
    public string? IdToken { get; set; }
}
