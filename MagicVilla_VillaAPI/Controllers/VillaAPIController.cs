using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;

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
        public  ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(_db.villas.ToList());
        }
        [HttpGet("{id:int}", Name = "getOneVilla")]
        public async Task<ActionResult<VillaDto>> getOneVilla(int id)
        {
            if (id == 0)    
            {
                return BadRequest();
            }
            var VillaDto = await _db.villas.FirstOrDefaultAsync(u => u.Id == id);
            if (VillaDto == null)
            {
                return NotFound();
            }
            return Ok(VillaDto);
        }
        [HttpPost]
        public async Task<ActionResult<VillaDto>> createNewVilla([FromBody] VillaCreateDto villaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(villaDto == null)
            {
                return BadRequest();
            }
            var idNew = _db.villas.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                Id = idNew,
                ImageUrl = villaDto.ImageUrl,
                Name = villaDto.Name,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            await _db.villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("getOneVilla", new {id = model.Id},model);
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
        public async Task<IActionResult> DeleteOneVilla(int id)
        {
            if(id == null || id == 0 )
            {
                return BadRequest();
            }
            var findVilla = await _db.villas.FirstOrDefaultAsync(v => v.Id == id);
            if(findVilla == null)
            {
                return NotFound();  
            }
            _db.villas.Remove(findVilla);
            await _db.SaveChangesAsync();
            return Ok("Delete with id " + id);
        }


        [HttpPut]
        public async Task<ActionResult<VillaDto>> updateVilla([FromBody] VillaUpdateDto villaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (villaDto == null)
            {
                return BadRequest();
            }
            var villaFind = await  _db.villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == villaDto.Id);
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
            await _db.SaveChangesAsync();



            return Ok("Updated !!!");
        }
    }       
}
