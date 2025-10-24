using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Business.Services;
using Data.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Enums;
using Shared.Common;
using Shared.Model.Request.Account;
using Shared.Resources;
using System.Net;
using Shared.Model.Base;



namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProfileController : ApiBaseController
    {
        private readonly IProfileService _profileService;
        private readonly IManageService _manageService;


        public ProfileController(IManageService manageService, IProfileService profileService)
        {
            _profileService = profileService;
            _manageService = manageService;
        }

        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int id=0)
        {
            if(id == 0)
            {
                id = UserId;
            }
            var getUserDetail = await _profileService.GetUserDetails(id);

            if (getUserDetail.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, getUserDetail);
            }

            return StatusCode((int)HttpStatusCode.OK, getUserDetail);

        }

        [Route("ViewProfile")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfileDetails(int id=0)
        {

            if (id == 0)
            {
                id = UserId;
            }
            var getUserDetail = await _profileService.GetUserProfileDetails(id);

            if (getUserDetail.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, getUserDetail);
            }

            return StatusCode((int)HttpStatusCode.OK, getUserDetail);

        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Profile profileRequest)
        {
            var response = await _profileService.UpdateProfile(profileRequest, UserId);

            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }

            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("ChangePassword")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel request)
        {
            request.ConfirmPassword = request.NewPassword;
            var message = await _manageService.ChangePassword(request, UserId);

            if (message == ResponseTypes.Success)
            {
                return StatusCode(StatusCodes.Status200OK, new ApiResponse<bool>(false, message: ResourceString.PasswordUpdated, apiName: "ChangePassword"));

            }
            else if (message == ResponseTypes.OldPasswordWrong)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse<bool>(false, message: ResourceString.WrongOldPassword, apiName: "ChangePassword"));

            }
            else if (message == ResponseTypes.OldNewPasswordMatched)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse<bool>(false, message: ResourceString.OldNewPasswordNotSame, apiName: "ChangePassword"));

            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse<bool>(false, message: ResourceString.FailedToSetNewPassword, apiName: "ChangePassword"));

            }
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> AccountDelete()
        {
            var response = await _profileService.AccountDelete(UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }

            return StatusCode((int)HttpStatusCode.NoContent, response);
        }

        [Route("ImageUpload")]
        [HttpPost]
        public IActionResult ImageUpload([FromForm] ImageFile file)
        {
            var response = _profileService.ImageUpload(file.File);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);

            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [Route("UpdateUserName")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserName(UpdateUserNameRequest request)
        {
            var response = await _profileService.UpdateUserName(request, UserId);
            if (response.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, response);
            }
        }

        [Route("GetUserVouchers")]
        [HttpPost]
        public async Task<IActionResult> GetUserVouchers(int id=0)
        {
            if (id == 0)
            {
                id = UserId;
            }
            var response = await _profileService.GetUserVouchers(id);
            if (response != null)
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
