using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MusicWebapi.Api.Models;
using MusicWebapi.Application.Services;
using MusicWebapi.Application.Interfaces;

namespace MusicWebapi.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{

    IUserService service;

    public UserController(IUserService service)
    {
        this.service = service;
    }

    [Authorize(Policy = "Admin")]
    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        var l = service.Get();
        if (l == null)
            return NotFound();
        return l;
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var m = service.Get(id);
        if (m == null)
            return NotFound(); // return Unauthorized();
        return m;
    }

    [Authorize(Policy = "AllUsers")]
    [HttpGet("me")]
    public ActionResult<User> GetMe()
    {
        var m = service.GetMe();
        if (m == null)
            return NotFound();
        return m;
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public ActionResult Create(User newU)
    {
        User m = service.Create(newU);
        return CreatedAtAction(nameof(Create), new { id = m.Id });
    }

    [Authorize(Policy = "AllUsers")]
    [HttpPut("{id}")]
    public ActionResult<User> Update(int id, User newUser)
    {
        var flag = service.Update(id, newUser);
        if (flag == 0)
            return NotFound();
        if (flag == 1)
            return BadRequest();
        if (flag == 4) //symbolying 'Unauthorized'
            return Unauthorized();
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var flag = service.Delete(id);
        if (flag == false)
            return NotFound();
        return NoContent();
    }

}
