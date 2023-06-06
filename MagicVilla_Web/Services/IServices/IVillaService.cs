using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(VillaNumberCreateDto dto);
        Task<T> UpdateAsync<T>(VillaUpdateDto dto);
        Task<T> DeleteAsync<T>(int id);
    }
}
