﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class VillaNumberCreateDto
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int villaID { get; set; }


        public string SpecialDetails { get; set; }
    }
}
