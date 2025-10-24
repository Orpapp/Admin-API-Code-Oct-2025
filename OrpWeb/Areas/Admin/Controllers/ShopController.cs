using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common; 
using Shared.Model.DTO.Admin.Shop; 
using Shared.Model.Request.Admin.Shop;
using Web.Areas.Admin.Controllers.Base;

namespace OrpWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ShopController : AdminBaseController
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }
        public IActionResult Index()
        {
            ShopDto categories = new ShopDto();
            ViewBag.PageIndex = SiteKeys.DefultPageNumber;
            ViewBag.PageSize = SiteKeys.DefultPageSize;
            return View(categories);
        }

        [HttpPost]
        public async Task<JsonResult> Index(DataTableParameters param)
        {
            ShopDetailsModel model = new ShopDetailsModel(param);
            var items = await _shopService.GetShops(model);
            var shops = items.Data;
            var result = new DataTableResult<ShopDto>
            {
                Draw = param.Draw,
                Data = shops,
                RecordsFiltered = shops?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = shops?.Count() ?? 0
            };
            return Json(result);
        } 
    }
}
