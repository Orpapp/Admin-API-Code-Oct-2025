using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.UserSubscription;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SubscriptionController : ApiBaseController
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }


        [Route("GetAppSubscriptionPlans")]
        [HttpGet]
        public async Task<IActionResult> GetAppSubscriptionPlans()
        {
            var response = await _subscriptionService.GetAppSubscriptionPlans();
            if (response.Data is not null || response.Data?.Count > 0)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }


        [Route("SaveUserSubscription")]
        [HttpPost]
        public async Task<IActionResult> SaveUserSubscription(UserSubscription request)
        {
            var response = await _subscriptionService.SaveUserSubscription(request, UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }


        [Route("GetUserSubscription")]
        [HttpGet]
        public async Task<IActionResult> GetUserSubscription()
        {
            var response = await _subscriptionService.GetUserSubscription(UserId);
            if (response.Data is not null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }

        [Route("CancelUserSubscription")]
        [HttpGet]
        public async Task<IActionResult> CancelUserSubscription()
        {
            var response = await _subscriptionService.CancelUserSubscription(UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
        [Route("CheckSubscriptionStatus")]
        [HttpGet]
        public async Task<IActionResult> CheckSubscriptionStatus()
        {
            var response = await _subscriptionService.CheckUserSubscriptionStatus(UserId);
            if (response.Data is not null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }
    }
}
