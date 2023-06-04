using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : Controller
    {       
        private readonly IVillaRepository _dbVilla;
        private  readonly IMapper _mapper;
        public VillaAPIController (IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

        [HttpGet]
        public async  Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _dbVilla.GetAll();
            return Ok(_mapper.Map<List<VillaDto>>(villaList));
        }
        [HttpGet("{id:int}", Name = "getOneVilla")]
        public async Task<ActionResult<VillaDto>> getOneVilla(int id)
        {
            if (id == 0)    
            {
                return BadRequest();
            }
            var VillaDto = await _dbVilla.GetOne(u => u.Id == id);
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

            var idNew = await _dbVilla.GetLength() + 1;


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
            await _dbVilla.Create(model);
            await _dbVilla.Save();

            return CreatedAtRoute("getOneVilla", new {id = model.Id},model);
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
        public async Task<IActionResult> DeleteOneVilla(int id)
        {
            if(id == null || id == 0 )
            {
                return BadRequest();
            }
            var findVilla = await _dbVilla.GetOne(v => v.Id == id);
            if(findVilla == null)
            {
                return NotFound();  
            }
            await _dbVilla.Remove(findVilla);
            await _dbVilla.Save();
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
            var villaFind = await  _dbVilla.GetOne(u => u.Id == updateVillaDto.Id, false);
            if(villaFind == null)
            {
                return NotFound();
            }
            Villa model = _mapper.Map<Villa>(updateVillaDto);
            await _dbVilla.Update(model);
            await _dbVilla.Save();



            return Ok("Updated !!!");
        }
    }       
}
