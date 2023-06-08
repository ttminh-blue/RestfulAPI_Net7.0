using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaService, IMapper mapper)
        {
            _villaNumberService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDto> list = new();

            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }


        public async Task<IActionResult> CreateVillaNumber()
        {
            return View();
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateDto model)
		{
			if (ModelState.IsValid)
			{

				var response = await _villaNumberService.CreateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
			}
			return View(model);
		}
        
        public async Task<IActionResult> UpdateVillaNumber(int id)
        {
			VillaDto element = new();
			var response = await _villaNumberService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess) {
				element = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDto>(element));
            }
            return NotFound();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateDto model)
        {
            if (ModelState.IsValid)
            {

                var response = await _villaNumberService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteVillaNumber(int id)
        {

            VillaNumberDto element = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                element = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaDto>(element));
            }
            return NotFound();

        }
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVillaNumber(VillaNumberDto model)
		{

			try
			{
				var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNo);

				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					return View(null);
				}
			}
			catch (Exception ex)
			{
				return View(null);
			}
		}

	}
}
