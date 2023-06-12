using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]

    [ApiController]
    [ApiVersion("1.0")]


    public class VillaNumberAPIController : Controller
    {       
        private readonly IVillaNumberRepository _dbVillaNumber;
        private  readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;

        protected APIResponse _response;
        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper, APIResponse response, IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            _response = response;
            _dbVilla = dbVilla;
        }

        [HttpGet]
        public async  Task<ActionResult<APIResponse>> GetVillaNumber()
        {
            try
            {
				IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAll(includeProperties: "Villa");
				_response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumberList);
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

       /* [HttpGet]
        [MapToApiVersion("1.0")]

        public IEnumerable<string> Get()
        {
            return new string[]
            {
                "value1", "value2"
            };
        }*/


        [HttpGet("{id:int}", Name = "getOneVillaNumber")]
        public async Task<ActionResult<APIResponse>> getOneVillaNumber(int id)
        {
            try { 
                if (id == 0)    
                {
                    return BadRequest();
                }
                var VillaDto = await _dbVillaNumber.GetOne(u => u.VillaNo == id);
                if (VillaDto == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaNumberDto>(VillaDto);
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

        public async Task<ActionResult<APIResponse>> createNewVillaNumber([FromBody] VillaNumberCreateDto createVillaDto)
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
                if(await _dbVillaNumber.GetOne(u => u.VillaNo == createVillaDto.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already Exists!");
                    return BadRequest(ModelState);
                }
                if(await _dbVilla.GetOne(u => u.Id == createVillaDto.villaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    return BadRequest(ModelState);
                }

                var idNew = createVillaDto.VillaNo;


                VillaNumber model = new()
                {
                    SpecialDetails = createVillaDto.SpecialDetails,
                    UpdatedDate = DateTime.Now,
                    VillaNo = idNew,
                    villaID = createVillaDto.villaID,
                    CreatedDate = DateTime.Now
                  
                };
                await _dbVillaNumber.Create(model);

                _response.Result = _mapper.Map<VillaNumberDto>(model);
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtRoute("getOneVillaNumber", new {id = model.VillaNo}, _response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };

                return BadRequest(_response);
            }
        }
        [HttpDelete("{id:int}", Name = "DeleteOneVillaNumber")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<APIResponse>> DeleteOneVillaNumber(int id)
        {
            try { 
                if(id == null || id == 0 )
                {
                    return BadRequest();
                }
                var findVilla = await _dbVillaNumber.GetOne(v => v.VillaNo == id);
                if(findVilla == null)
                {
                    return NotFound();  
                }
                await _dbVillaNumber.Remove(findVilla);
                await _dbVillaNumber.Save();


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

        public async Task<ActionResult<APIResponse>> updateVillaNumber([FromBody] VillaNumberUpdateDto updateVillaDto)
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
               
               
                var villaFind = await  _dbVillaNumber.GetOne(u => u.VillaNo == updateVillaDto.VillaNo);
                if(villaFind == null)
                {
                    return NotFound();
                }
				var getVilla = await _dbVilla.GetOne(u => u.Id == villaFind.villaID);


				VillaNumber model = _mapper.Map<VillaNumber>(villaFind);
                
                model.SpecialDetails = updateVillaDto?.SpecialDetails;
                model.Villa = getVilla;
                await _dbVillaNumber.Update(model);

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
    }       
}
