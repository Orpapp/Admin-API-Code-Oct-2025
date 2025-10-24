using Business.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Response;
using System.IO;
using Web.Areas.Admin.Controllers.Base;

namespace Web.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LevelImageController : AdminBaseController
    {
        private readonly ILevelService _levelService;

        public LevelImageController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        public async Task<IActionResult> Index()
        {            
            var level = await _levelService.GetLevelImage();
            return View(level.Data);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateLevelBackgroundImage(IFormFile? formFile, bool imageDeleted = false)
        {
            try
            {
                var result = await _levelService.UpdateLevelBackgroundImage(formFile, imageDeleted);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }               
    }
}