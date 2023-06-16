using MagicVilla_Utility;
using MagicVilla_VillaAPI.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using NuGet.Protocol;

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



                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(temp.Token);


                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken, temp.Token);
                HttpContext.Session.SetString(SD.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value);


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
			ViewData["Error"] = "None";
			ViewBag.ErrorMessages = "";
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
			if(result.ErrorMessages.Count() > 0)
			{
                ViewData["Error"] = "Has Error";
                ViewBag.ErrorMessages = result.ErrorMessages;
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
