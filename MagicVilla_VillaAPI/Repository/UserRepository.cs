using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _db;
		private string secretKey;
		public UserRepository(AppDbContext context, IConfiguration configuration)
		{
			_db = context;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
		}
		public bool IsUniqueUser(string username)
		{
			var user = _db.localUser.FirstOrDefault(u => u.UserName == username);
			if (user == null)
			{
				return true;
			}
			return false;
		}
		public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
		{
			var checkUser = _db.localUser.FirstOrDefault(u => u.UserName == loginRequestDTO.UserName && u.Password == loginRequestDTO.Password);
			if(checkUser == null)
			{
				return new LoginResponseDto()
				{
					Token = "",
					User = null
				};
			}
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
			   {
					new Claim(ClaimTypes.Name, checkUser.Id.ToString()),
					new Claim(ClaimTypes.Role, checkUser.Role)
			   }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			LoginResponseDto loginRequestDto = new LoginResponseDto()
			{
				Token = tokenHandler.WriteToken(token),
				User = checkUser
			};


			return loginRequestDto;
		}
		public async Task<LocalUser> Register(RegisterationRequestDto registerationRequestDTO)
		{
			var checkUser = IsUniqueUser(registerationRequestDTO.UserName);
			if (checkUser == false)
			{
				return null;
			}
			LocalUser user = new LocalUser()
			{
				UserName = registerationRequestDTO.UserName,
				Password = registerationRequestDTO.Password,
				Name = registerationRequestDTO.Name,
				Role = registerationRequestDTO.Role
			};
			_db.localUser.Add(user);
			await _db.SaveChangesAsync();
			user.Password = "";
			return user;
		}
	}
}
