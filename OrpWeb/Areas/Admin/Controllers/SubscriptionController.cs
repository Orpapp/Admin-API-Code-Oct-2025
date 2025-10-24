using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Admin.UserSubscription;
using Shared.Model.Request.Admin.UserSubscription;
using Web.Areas.Admin.Controllers.Base;

namespace OrpWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class SubscriptionController : AdminBaseController
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public IActionResult Index()
        {
            UserSubscriptionDto categories = new UserSubscriptionDto();
            ViewBag.PageIndex = SiteKeys.DefultPageNumber;
            ViewBag.PageSize = SiteKeys.DefultPageSize;
            return View(categories);
        }

        [HttpPost]
        public async Task<JsonResult> Index(DataTableParameters param)
        {
            UserSubscriptionDetailsModel model = new(param)
            {
                ProductType = (byte)ProductTypes.Subscription
            };
            var items = await _subscriptionService.GetSubscriptions(model);
            var shops = items.Data;
            var result = new DataTableResult<UserSubscriptionDto>
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
