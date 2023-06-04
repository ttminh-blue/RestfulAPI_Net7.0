using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : Controller
    {       
        private readonly AppDbContext _db;
        private  readonly IMapper _mapper;
        public VillaAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async  Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDto>>(villaList));
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
            return Ok(_mapper.Map<VillaDto>(VillaDto));
        }
        [HttpPost]
        public async Task<ActionResult<VillaDto>> createNewVilla([FromBody] VillaCreateDto createVillaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(createVillaDto == null)
            {
                return BadRequest();
            }
            var idNew = _db.villas.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;


            Villa model = new()
            {
                Amenity = createVillaDto.Amenity,
                Details = createVillaDto.Details,
                Id = idNew,
                ImageUrl = createVillaDto.ImageUrl,
                Name = createVillaDto.Name,
                Occupancy = createVillaDto.Occupancy,
                Rate = createVillaDto.Rate,
                Sqft = createVillaDto.Sqft
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
        public async Task<ActionResult<VillaDto>> updateVilla([FromBody] VillaUpdateDto updateVillaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (updateVillaDto == null)
            {
                return BadRequest();
            }
            var villaFind = await  _db.villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == updateVillaDto.Id);
            if(villaFind == null)
            {
                return NotFound();
            }
            Villa model = _mapper.Map<Villa>(updateVillaDto);
            _db.villas.Update(model);
            await _db.SaveChangesAsync();



            return Ok("Updated !!!");
        }
    }       
}
