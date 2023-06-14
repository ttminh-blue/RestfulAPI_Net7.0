using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : Controller
    {       
        private readonly IVillaRepository _dbVilla;
        private  readonly IMapper _mapper;
        protected APIResponse _response;
        private const int pageSizeDefault = 0;
        private const int pageNumberDefault = 1;

        public VillaAPIController (IVillaRepository dbVilla, IMapper mapper, APIResponse response)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = response;
        }

        [HttpGet]
        /*[ResponseCache(Duration = 30, Location = ResponseCacheLocation.None, NoStore = true)]*/
        public async  Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name = "filterOccupancy")]int? occupancy, [FromQuery] string? searchName, 
            [FromQuery]int? pageSize, [FromQuery]int? pageNumber)
        {
            try
            {
                int newPageSize, newPageNumber;

                if(pageSize == null)
                {
                    newPageSize = 0;
                }
                else
                {
                    newPageSize =  pageSize.Value;
                }
                if(pageNumber == null)
                {
                    newPageNumber = 1;
                }
                else
                {
                    newPageNumber = pageNumber.Value;
                }
                
                IEnumerable<Villa> villaList;
                if(occupancy > 0)
                {
                    villaList = await _dbVilla.GetAll(y => y.Occupancy == occupancy, null , newPageSize, newPageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAll(pageSize: newPageSize, pageNumber : newPageNumber);
                }
                if (!searchName.IsNullOrEmpty())
                {
                    villaList = villaList.Where(u =>  u.Name.ToLower().Contains(searchName));
                }
                Pagination pagination = new Pagination()
                {
                    pageNumber = newPageNumber,
                    pageSize = newPageSize
                };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
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
        [ResponseCache(Duration = 30)]
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
                _response.Result = _mapper.Map<VillaDto>(VillaDto);
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
		[Authorize(Roles = "Admin")]

		[ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> createNewVilla([FromBody] VillaCreateDto createVillaDto)
        {
          
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                if (createVillaDto == null)
                {
                    return BadRequest();
                }

                var idNew = await _dbVilla.GetLength() + 2;


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
               

                _response.Result = _mapper.Map<VillaDto>(model);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtRoute("getOneVilla", new { id = model.Id }, _response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVilla")]
		[Authorize(Roles = "Admin")]

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
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]

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

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(e.Message);
            }
        }
    }       
}
