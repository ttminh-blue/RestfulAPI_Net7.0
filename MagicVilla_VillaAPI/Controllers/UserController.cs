using Azure;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/UsersAuth")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		protected APIResponse _response;
        public UserController(IUserRepository userRepository, APIResponse res)
        {
			_userRepository = userRepository;
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
/*				_response.ErrorMessages.Add("Username or password is incorrect.");
*/				return BadRequest(_response); 
			}
			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = userLogin;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterationRequestDto model)
		{
			var user = _userRepository.Register(model);

			if(user.Result == null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
		/*		_response.ErrorMessages.Add("User already exists");*/
				return BadRequest(_response);	
			}
			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);

		}
	}
}
