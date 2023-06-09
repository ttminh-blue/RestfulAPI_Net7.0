using Microsoft.AspNetCore.Mvc;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;
using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
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
               
                if (await _dbVilla.GetOne(u => u.Id == updateVillaDto.VillaNo) == null)
                {
                    return BadRequest(ModelState);
                }
                var villaFind = await  _dbVillaNumber.GetOne(u => u.VillaNo == updateVillaDto.VillaNo, false);
                if(villaFind == null)
                {
                    return NotFound();
                }
                VillaNumber model = _mapper.Map<VillaNumber>(updateVillaDto);
                await _dbVillaNumber.Update(model);
                await _dbVillaNumber.Save();

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
