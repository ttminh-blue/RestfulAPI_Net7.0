using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
	public class AuthService : BaseService, IAuthService
    {
		public APIResponse responseModel { get; set; }
		public IHttpClientFactory httpClient { get; set; }
		private string villaUrl;
		public AuthService(IHttpClientFactory _httpClient, IConfiguration confg) : base(_httpClient)
		{
			villaUrl = confg.GetValue<string>("ServiceUrls:VillaAPI");
			httpClient = _httpClient;
		}


		public Task<T> LoginAsync<T>(LoginRequestDto objToCreate)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = objToCreate,
				Url = villaUrl + "/api/UsersAuth/login"
			});
		}

		public Task<T> RegisterAsync<T>(RegisterationRequestDto objToCreate)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = objToCreate,
				Url = villaUrl + "/api/UsersAuth/register"
			});
		}
	}
}
