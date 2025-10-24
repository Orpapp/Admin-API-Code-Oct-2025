using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Social;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class RewardController : ApiBaseController
    {
        private readonly IRewardService _rewardService;
        private readonly IProfileService _profileService;
        public RewardController(IRewardService rewardService, IProfileService profileService)
        {
            _rewardService = rewardService;
            _profileService = profileService;
        }

        [Route("AddUpdate")]
        [HttpPost]
        public async Task<IActionResult> AddUpdateReward(Reward request)
        {
            request.UserId = UserId;
            var response = await _rewardService.UpdateReward(request);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, response);
        }


        [Route("GetTime")]
        [HttpPost]
        public async Task<IActionResult> GetSpendTime()
        {

            var response = await _rewardService.GetSpendedTime(UserId);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);

        }

        [Route("GetStreakCount")]
        [HttpGet]
        public async Task<IActionResult> GetStreakCount()
        {

            var response = await _profileService.GetStreakCount(UserId);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);

        }
    }
}
