using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using  MusicNameSpace.Models;
using homeWorkSe.Services;


namespace homeWork.Controllers;

    [ApiController]
    [Route("[controller]")]
    public class MusicController : ControllerBase{

        private MusicService service;

        public MusicController()
        {
            service = new MusicService();
        }

        // private Music find(int id)
        //  {
        // return list.FirstOrDefault(p => p.Id == id);

        // }

        [HttpGet()]
        public ActionResult<IEnumerable<Music>> Get()
        {
            var l=service.Get();
            if (l==null)
                return NotFound() ;
            return l;
        }

        [HttpGet("{id}")]
        public ActionResult<Music> Get(int id)
        {
            var m = service.Get(id);
            if (m == null)
                return NotFound();
            return m;
        }

        [HttpPost]
        public ActionResult Create(Music newMusic)
        {
            Music m = service.Create(newMusic);
            return CreatedAtAction(nameof(Create), new { id = m.Id });
        }

        [HttpPut("{id}")]
        public ActionResult<Music> Update(int id,Music newMusic)
        {

            var flag = service.Update(id, newMusic);
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


