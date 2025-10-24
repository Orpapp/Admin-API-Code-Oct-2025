using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Social;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class HelpAndTipsController : ApiBaseController
    {
        private readonly IHelpAndTipsService _helpAndTipsService;
        public HelpAndTipsController(IHelpAndTipsService helpAndTipsService)
        {
            _helpAndTipsService = helpAndTipsService;
        }

        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> GetHelpAndTips()
        {
            var response = await _helpAndTipsService.GetHelpAndTips();
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
