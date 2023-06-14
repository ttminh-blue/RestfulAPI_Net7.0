using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
			_authService = authService;
        }
        public IActionResult Login()
		{
			LoginRequestDto data = new();
			return View(data);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDto model)
		{
            APIResponse result = await _authService.LoginAsync<APIResponse>(model);
            if (result != null && result.IsSuccess)
            {
                LoginResponseDto temp = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(result.Result));

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, temp.User.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, temp.User.Role));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken, temp.Token);
                HttpContext.Session.SetString(SD.Role, temp.User.Role);


                return RedirectToAction("Index", "Home");
            }
			else
			{
				ModelState.AddModelError("CustomError", result.ErrorMessages.FirstOrDefault());
				return View(model);
			}
		}


		public IActionResult Register()
		{
			RegisterationRequestDto data = new();
			return View(data);
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterationRequestDto model)
		{
			APIResponse result = await _authService.RegisterAsync<APIResponse>(model);
			if (result != null && result.IsSuccess)
			{
				return RedirectToAction("Login");
			}

			return View();
		}


		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
		{
			return View();
		}

	}
}
