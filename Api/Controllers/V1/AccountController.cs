using Api.Authorization.JWT;
using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Common;
using Shared.Model.JWT;
using Shared.Model.Request.Account;
using Shared.Resources;
using System.Net;

namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AccountController : ApiBaseController
    {
        private readonly IAccountService _accountService;
        private readonly JwtTokenSettings _jwtTokenSettings;

        public AccountController(IAccountService accountService, IOptions<JwtTokenSettings> jwtOptions)
        {
            _accountService = accountService;
            _jwtTokenSettings = jwtOptions.Value;
        }

        [Route("Login")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] ApiLoginRequest request)
        {
            var loginUserDetail = await _accountService.Login(request);
            if (loginUserDetail.Data == null)
            {
                if (loginUserDetail.Message == ResourceString.EmailNotVerified)
                {
                    return StatusCode((int)HttpStatusCode.ExpectationFailed, loginUserDetail);
                }
                return StatusCode((int)HttpStatusCode.NotFound, loginUserDetail);
            }
            else
            {
                JwtTokenBuilder tokenBuilder = new();
                var token = tokenBuilder.GetToken(_jwtTokenSettings, loginUserDetail.Data.UserId);
                loginUserDetail.Data.AuthorizationToken = token.Value;
                return StatusCode((int)HttpStatusCode.OK, loginUserDetail);
            }
        }

        [Route("Login/Social")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequest request)
        {
            var loginUserDetail = await _accountService.SocialLogin(request);
            if (loginUserDetail.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, loginUserDetail);
            }
            else
            {
                JwtTokenBuilder tokenBuilder = new();
                var token = tokenBuilder.GetToken(_jwtTokenSettings, loginUserDetail.Data.UserId);
                loginUserDetail.Data.AuthorizationToken = token.Value;
                return StatusCode((int)HttpStatusCode.OK, loginUserDetail);
            }
        }

        [Route("SignUp")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] RegistrationRequest request)
        {
            var userDetail = await _accountService.SignUp(request);

            if (userDetail.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, userDetail);

            }
            return StatusCode((int)HttpStatusCode.BadRequest, userDetail);


        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var updateUserProfile = await _accountService.Logout(UserId);
            if (updateUserProfile.Data)
            {
                return StatusCode((int)HttpStatusCode.OK, updateUserProfile);
            }
            return StatusCode((int)HttpStatusCode.BadRequest, updateUserProfile);
        }

        [Route("ForgetPassword")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            var updateUser = await _accountService.ForgetPassword(request);
            if (updateUser.Data)
            {
                return StatusCode(StatusCodes.Status200OK, updateUser);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, updateUser);
            }
        }



        [Route("EmailVerify")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> EmailVerify([FromBody] EmailRequest request)
        {
            var verifyEmail = await _accountService.ResendEmailVerificationMail(request.Email);
            return StatusCode(StatusCodes.Status200OK, verifyEmail);
        }
    }
}
