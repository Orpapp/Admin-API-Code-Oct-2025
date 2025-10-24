using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO;

using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using Shared.Resources;
using Web.Areas.Admin.Controllers.Base;

namespace Web.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LevelController : AdminBaseController
    {
        private readonly IManageService _manageService;
        private readonly IRewardService _rewardService;
        public LevelController(IManageService manageService,IRewardService rewardService)
        {
            _manageService = manageService;
            _rewardService = rewardService;
        }
        public IActionResult Index()
        {
            ManageLevelDto level = new ManageLevelDto();
            ViewBag.PageIndex = SiteKeys.DefultPageNumber;
            ViewBag.PageSize = SiteKeys.DefultPageSize;
            return View(level);
        }

        [HttpPost]
        public async Task<JsonResult> Index(DataTableParameters param)
        {
            LevelDetailsModel model = new LevelDetailsModel(param);
            var items = await _rewardService.GetAllLevel(model);
            var levels = items.ResponsePacket;
            var result = new DataTableResult<ManageLevelDto>
            {
                Draw = param.Draw,
                Data = levels,
                RecordsFiltered = levels?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = levels?.Count() ?? 0
            };
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {
            if (id > 0)
            {
                ViewBag.LevelId = id;
                var result = await _rewardService.GetLevelInformation(id);
               
                    return View(result.Data);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Detail(List<LevelInfo> levelInfos)
        {
            foreach (var levelInfo in levelInfos)
            {

                levelInfo.StoppageImage = Request.Form[levelInfo.Id.ToString()].ToString();
            }

            var updateUser = await _manageService.UpdateLevel(levelInfos);
            return Json(updateUser);
        }

        public IActionResult GetTechnoEnumValues(string term)
        {
            var technoEnumValues = Enum.GetNames(typeof(AutoCompleteEnum))
                                      .Where(item => item.Contains(term, StringComparison.OrdinalIgnoreCase))
                                      .OrderBy(item => item)
                                      .ToList();

            return Json(technoEnumValues);
        }

      
        [HttpPost]
        public Task<PartialViewResult> StoppageInfo(LevelInfo level)
        {
            return Task.FromResult(PartialView(level.StoppageType, level));
        }

    }
}
