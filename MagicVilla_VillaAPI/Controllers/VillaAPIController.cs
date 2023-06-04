using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : Controller
    {       
        private readonly AppDbContext _db;
        public VillaAPIController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(_db.villas.ToList());
        }
        [HttpGet("{id:int}", Name = "getOneVilla")]
        public ActionResult<VillaDto> getOneVilla(int id)
        {
            if (id == 0)    
            {
                return BadRequest();
            }
            var VillaDto = _db.villas.FirstOrDefault(u => u.Id == id);
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
            villaDto.Id = _db.villas.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            _db.villas.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("getOneVilla", new {id = villaDto.Id},villaDto);
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
        public IActionResult DeleteOneVilla(int id)
        {
            if(id == null || id == 0 )
            {
                return BadRequest();
            }
            var findVilla = _db.villas.FirstOrDefault(v => v.Id == id);
            if(findVilla == null)
            {
                return NotFound();  
            }
            _db.villas.Remove(findVilla);
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
            var villaFind = _db.villas.FirstOrDefault(u => u.Id == villaDto.Id);
            if(villaFind == null)
            {
                return NotFound();
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = villaDto.Id,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            _db.villas.Update(model);
            _db.SaveChanges();



            return Ok("Updated !!!");
        }
    }       
}
