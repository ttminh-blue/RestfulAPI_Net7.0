using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
	public interface IUserRepository
	{
		bool IsUniqueUser(string username);
		Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);
		Task<LocalUser> Register(RegisterationRequestDto registerationRequestDTO);
	}
}
