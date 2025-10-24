using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class VoucherController : ApiBaseController
    {

        private readonly ILevelVoucherService _levelVoucherService;
        public VoucherController(ILevelVoucherService levelVoucherService)
        {
            _levelVoucherService = levelVoucherService;
        }

        [Route("GetVoucher")]
        [HttpGet]
        public async Task<IActionResult> GetVoucher(int levelNumber)
        {

            var response = await _levelVoucherService.GetVoucher(levelNumber);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);

        }
    }
}
