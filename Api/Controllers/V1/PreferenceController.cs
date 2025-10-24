using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Account;
using System.Net;



namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class PreferenceController : ApiBaseController
    {
        private readonly IProfileService _profileService;
        public PreferenceController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> GetPreference()
        {
            var getPreferences = await _profileService.GetPreferenceById(UserId);

            if (getPreferences.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, getPreferences);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.OK, getPreferences);
            }
        }


        [Route("Search")]
        [HttpPost]
        public async Task<IActionResult> SelectSearch(PreferenceSearchRequest request)
        {
            var response = await _profileService.SearchPreference(request.Keyword, request.Type);
            if (response!=null)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
        }


        [Route("Select")]
        [HttpPost]
        public async Task<IActionResult> SelectPreference(List<PrefenceRequest> request)
        {
            var response = await _profileService.UpdatePreference(request, UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
        }

    }
}
