using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Task;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]

    public class LevelController : ApiBaseController
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }
                

        [Route("GetStoppageList")]
        [HttpGet]
        public async Task<IActionResult> GetStoppageList()
        {
            var response = await _levelService.GetStoppageList(UserId);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }


        [Route("GetUserLevel")]
        [HttpGet]
        public async Task<IActionResult> GetUserLevel()
        {
            var response = await _levelService.GetUserLevel(UserId);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [Route("GetUserReward")]
        [HttpGet]
        public async Task<IActionResult> GetUserReward()
        {
            var response = await _levelService.GetUserReward(UserId);
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }

    }
}
