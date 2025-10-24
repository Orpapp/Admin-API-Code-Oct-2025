using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Progress;
using Shared.Model.Request.Social;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProgressController : ApiBaseController
    {
        private readonly IProgressService _progressService;
        public ProgressController(IProgressService rewardService)
        {
            _progressService = rewardService;
        }                


        [Route("GetProgress")]
        [HttpPost]
        public async Task<IActionResult> GetUserProgress()
        {

            var response = await _progressService.GetUserProgress(UserId);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);

        }
        
        [Route("GetProductivityHours")]
        [HttpPost]
        public async Task<IActionResult> GetUserProductivityHours(GetProductivityHours request)
        {
            var response = await _progressService.GetUserProductivityHours(UserId, request.FilterBy);
            return StatusCode((int)HttpStatusCode.OK, response);
        }


        [Route("GetUserWeeklySummaryReport")]
        [HttpGet]
        public async Task<IActionResult> GetUserWeeklySummaryReport()
        {
            var response = await _progressService.GetUserWeeklySummaryReport(UserId);
            if (response.Data == null || response.Data?.Count == 0)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }

            return StatusCode((int)HttpStatusCode.OK, response);
        }


    }
}
