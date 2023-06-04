using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : Controller
    {       
        private readonly ILogging _logger;

        public VillaAPIController(ILogging logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return VillaStore.listvalla;    
        }
        [HttpGet("{id:int}", Name = "getOneVilla")]
        public ActionResult<VillaDto> getOneVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var VillaDto = VillaStore.listvalla.FirstOrDefault(u => u.Id == id);
            if (VillaDto == null)
            {
                return NotFound();
            }
            return Ok(VillaDto);
        }
        [HttpPost]
        public ActionResult<VillaDto> createNewVilla([FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(villaDto == null)
            {
                return BadRequest();
            }
            villaDto.Id = VillaStore.listvalla.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.listvalla.Add(villaDto);

            return CreatedAtRoute("getOneVilla", new {id = villaDto.Id},villaDto);
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
        public IActionResult DeleteOneVilla(int id)
        {
            if(id == null || id == 0 )
            {
                return BadRequest();
            }
            var findVilla = VillaStore.listvalla.FirstOrDefault(v => v.Id == id);
            if(findVilla == null)
            {
                return NotFound();  
            }
            VillaStore.listvalla.Remove(findVilla);
            return Ok();
        }


        [HttpPut]
        public ActionResult<VillaDto> updateVilla([FromBody] VillaDto villaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (villaDto == null)
            {
                return BadRequest();
            }
            var villaFind = VillaStore.listvalla.FirstOrDefault(u => u.Id == villaDto.Id);
            if(villaFind == null)
            {
                return NotFound();
            }
            villaFind.Name = villaDto.Name;

       

            return Ok("Updated !!!");
        }
    }       
}
