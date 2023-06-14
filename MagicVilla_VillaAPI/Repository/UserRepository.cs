using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IConfiguration configuration, UserManager<AppUser> _userManager, IMapper mapper)
		{
			_db = context;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
			this._userManager = _userManager;
			_mapper = mapper;
		}
		public bool IsUniqueUser(string username)
		{
            var user = _db.AppUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }
		public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
		{
			var user = _db.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());


			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
			if(user == null)
			{
				return new LoginResponseDto()
				{
					Token = "",
					User = null
				};
			}
			var roles = await _userManager.GetRolesAsync(user);
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
			   {
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, roles.FirstOrDefault())
			   }),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			LoginResponseDto loginRequestDto = new LoginResponseDto()
			{
				Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDto>(user),
                Role = roles.FirstOrDefault()
            };


			return loginRequestDto;
		}
		public async Task<UserDto> Register(RegisterationRequestDto registerationRequestDTO)
		{
			var checkUser = IsUniqueUser(registerationRequestDTO.UserName);
			if (checkUser == false)
			{
				return null;
			}
			AppUser user = new ()
			{
				UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
                Name = registerationRequestDTO.Name
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "admin");
                    var userToReturn = _db.AppUsers
                        .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                    return _mapper.Map<UserDto>(userToReturn);

                }
            }
            catch (Exception e)
            {

            }
           
			return new UserDto();
		}
	}
}
