using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models
{
    public class VillaNumberDto
    {
        [Required]
        public int VillaNo { get; set; }

        public string SpecialDetails { get; set; }
    }
}
