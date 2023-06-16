using Azure;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/v{version:apiVersion}/UsersAuth")]
	[ApiController]
    [ApiVersion("1.0")]

    public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		protected APIResponse _response;
        public UserController(IUserRepository userRepository, APIResponse res)
        {
			_userRepository = userRepository;
			res.ErrorMessages = new List<string>();
			_response = res;

		}
		[HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] LoginRequestDto model)
		{
			var userLogin = await _userRepository.Login(model);
			if(userLogin.User == null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				return BadRequest(_response); 
			}
			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = userLogin;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterationRequestDto model)
		{
			model.Role = "Customer";
			var user = _userRepository.Register(model);

			if(user.Result.Errors != null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				foreach(var i in user.Result.Errors)
				{
					string tmp = (string)i.Description;
					_response.ErrorMessages.Add(tmp);
				}
				
				return BadRequest(_response);	
			}
			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);

		}
	}
}
