using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserNameSpace.Models;
using UserHW.Services;
using IUserServices.Interfaces;
using System.Security.Claims;
using TS.Services;
using Microsoft.AspNetCore.Authorization;
using IActiveUserN.Interfaces;


namespace homeWorkUser.Controllers;

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase{

        IUserService service;
        User activeUser;

        public UserController(IUserService userService,IActiveUser activeUser)
        {
            this.service=userService;
            this.activeUser = activeUser.ActiveUser
                ?? throw new System.InvalidOperationException("Active user is required");
        }
        
        [Authorize(Policy="AllUsers")]
        [HttpGet()]
        public ActionResult<IEnumerable<User>> Get()
        {
            var l=service.Get();
            if (l==null)
                return NotFound() ;

            if (activeUser.Type == "Regular")
            {
                var user = l.FirstOrDefault(u => u.Id == activeUser.Id);
                if (user == null)
                    return NotFound();
                return new List<User> { user };
            }
            return l;
        }

        [Authorize(Policy="AllUsers")]
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            if(id == activeUser.Id || activeUser.Type == "Admin"){
                var m = service.Get(id);
                if (m == null)
                    return NotFound();
                return m;
            }
            return Unauthorized();
          
        }

        [Authorize(Policy="Admin")]
        [HttpPost]
        public ActionResult Create(User newU)
        {

            User m = service.Create(newU);
            return CreatedAtAction(nameof(Create), new { id = m.Id });
        }

        [Authorize(Policy="AllUsers")]
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id,User newUser)
        {
            if(!(id == activeUser.Id || activeUser.Type == "Admin"))
                return Unauthorized();
            if(activeUser.Type != "Admin" && newUser.Type!=activeUser.Type)
                return Unauthorized();
                
            var flag = service.Update(id, newUser);
            if (flag == 0)
                return NotFound();
            if (flag==1)
                return BadRequest();
            return NoContent();
        }

        [Authorize(Policy="Admin")]
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var flag = service.Delete(id);
            if (flag == false)
                return NotFound();
            return NoContent();
        }

    }


