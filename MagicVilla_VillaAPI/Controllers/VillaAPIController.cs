using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : Controller
    {       
        private readonly IVillaRepository _dbVilla;
        private  readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaAPIController (IVillaRepository dbVilla, IMapper mapper, APIResponse response)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = response;
        }

        [HttpGet]
        public async  Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villaList = await _dbVilla.GetAll();

                _response.Result = _mapper.Map<List<VillaDto>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
        [HttpGet("{id:int}", Name = "getOneVilla")]
        public async Task<ActionResult<APIResponse>> getOneVilla(int id)
        {
            try { 
                if (id == 0)    
                {
                    return BadRequest();
                }
                var VillaDto = await _dbVilla.GetOne(u => u.Id == id);
                if (VillaDto == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<List<VillaDto>>(VillaDto);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> createNewVilla([FromBody] VillaCreateDto createVillaDto)
        {
            try { 
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

                _response.Result = _mapper.Map<List<VillaDto>>(model);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtRoute("getOneVilla", new {id = model.Id}, _response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
        public async Task<ActionResult<APIResponse>> DeleteOneVilla(int id)
        {
            try { 
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


                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok("Delete with id " + id);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }


        [HttpPut]
        public async Task<ActionResult<APIResponse>> updateVilla([FromBody] VillaUpdateDto updateVillaDto)
        {
            try { 
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

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok("Updated !!!");
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
    }       
}
