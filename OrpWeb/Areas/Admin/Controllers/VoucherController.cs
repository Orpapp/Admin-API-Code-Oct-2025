using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Model.Request.Admin;
using Shared.Resources;
using Web.Areas.Admin.Controllers.Base;

namespace OrpWeb.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class VoucherController : AdminBaseController
    {
        private readonly ILevelVoucherService _levelVoucherService;

        public VoucherController(ILevelVoucherService levelVoucherService)
        {
            _levelVoucherService = levelVoucherService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _levelVoucherService.GetAllVouchers();
            return View(result);
        }

        public async Task<IActionResult> UpdateVoucherImage(VoucherModel voucherModel)
        {
            TResponse<int> response = new();
            response.ResponsePacket = await _levelVoucherService.UpdateVoucherImage(voucherModel);

            if (response.ResponsePacket <= 0)
            {
                response.ResponseMessage = ResourceString.Error;
            }
            else
            {
                response.ResponseMessage = ResourceString.VoucherImageUpdatedSuccess;
            }
            return Json(response);
        }
    }
}
