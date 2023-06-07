﻿using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDto> list = new();

            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }


        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVilla(VillaCreateDto model)
		{
			if (ModelState.IsValid)
			{

				var response = await _villaService.CreateAsync<APIResponse>(model);
				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVilla));
				}
			}
			return View(model);
		}
        
        public async Task<IActionResult> UpdateVilla(int id)
        {
			VillaDto element = new();
			var response = await _villaService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess) {
				element = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaUpdateDto>(element));
            }
            return NotFound();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDto model)
        {
            if (ModelState.IsValid)
            {

                var response = await _villaService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteVilla(int id)
        {

            VillaDto element = new();
            var response = await _villaService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                element = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
                return View(_mapper.Map<VillaDto>(element));
            }
            return NotFound();

        }
        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVilla(VillaDto model)
		{

			try
			{
				var response = await _villaService.DeleteAsync<APIResponse>(model.Id);

				if (response != null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVilla));
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
