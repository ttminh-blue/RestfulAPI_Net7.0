using MagicVilla_VillaAPI.Models;
namespace MagicVilla_VillaAPI.Data
{
    public static class VillaStore
    {
       public static List<VillaDto> listvalla = new List<VillaDto> { 
           new VillaDto { Id = 1, Name = "Villa 1" },
           new VillaDto { Id = 2, Name = "Villa 2"}
       };
    }
}
