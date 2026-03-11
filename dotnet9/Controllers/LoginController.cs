
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IUserServices.Interfaces;
using System.Security.Claims;
using TS.Services; // TokenService
using UserNameSpace.Models;

namespace homeWorkUser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService service;

        public LoginController(IUserService userService)
        {
            service = userService;
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
                return Unauthorized("Wrong userId / password");

            var claims = new List<Claim>
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Name),
                new Claim("password",user.Passwd),
                new Claim("type", user.Type)
            };

            var token = TokenService.GetToken(claims);
            return Ok(TokenService.WriteToken(token));
        }
    }

    public class LoginRequest
    {
        public string Name { get; set; }
        public string Passwd { get; set; }
    }
}