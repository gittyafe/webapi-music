using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserNameSpace.Models;
using UserHW.Services;
using IUserServices.Interfaces;

namespace homeWorkUser.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase{

        IUserService service;

        public UserController(IUserService userService)
        {
            this.service=userService;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<User>> Get()
        {
            var l=service.Get();
            if (l==null)
                return NotFound() ;
            return l;
        }

        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var m = service.Get(id);
            if (m == null)
                return NotFound();
            return m;
        }

        [HttpPost]
        public ActionResult Create(User newU)
        {
            User m = service.Create(newU);
            return CreatedAtAction(nameof(Create), new { id = m.Id });
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update(int id,User newUser)
        {

            var flag = service.Update(id, newUser);
            if (flag == 0)
                return NotFound();
            if (flag==1)
                return BadRequest();
            return NoContent();
        }

         [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var flag = service.Delete(id);
            if (flag == false)
                return NotFound();
            return NoContent();
        }

    }


