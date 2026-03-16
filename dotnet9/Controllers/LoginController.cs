using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Interfaces;
using MusicWebapi.Application.Services;
using System.Security.Claims;


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

        var claims = new List<Claim>
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Name??""),
                new Claim("role", user.Role ?? ""),
                // Add NameIdentifier claim for SignalR user targeting
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
