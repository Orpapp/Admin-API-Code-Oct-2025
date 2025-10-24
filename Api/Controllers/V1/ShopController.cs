using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Enums;
using Shared.Model.Request.Shop;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ShopController : ApiBaseController
    {

        private readonly IShopService _shopService;
        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [Route("GetShopPriceList")]
        [HttpGet]
        public async Task<IActionResult> GetShopPriceList()
        {
            var response = await _shopService.GetShopProductList();
            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("GetShopItemList")]
        [HttpGet]
        public async Task<IActionResult> GetShopItemList(ItemsTypes itemsType)
        {
            var response = await _shopService.GetShopItemList(itemsType);
            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
        [Route("GetAvailableBalance")]
        [HttpGet]
        public async Task<IActionResult> GetAvailableBalance()
        {
            var response = await _shopService.GetAvailableBalance(UserId);
            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> AddShop([FromBody] List<Shop> request)
        {
              
            var response = await _shopService.AddShop(request,UserId);
            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("GetShop")]
        [HttpGet]
        public async Task<IActionResult> GetShop()
        {
            var response = await _shopService.GetShopByUser(UserId);
            if (response != null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
    }
}
