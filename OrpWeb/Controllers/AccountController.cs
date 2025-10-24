using Business.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Request.Account;
using Shared.Resources;

namespace Web.Controllers
{
    [AllowAnonymous, ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (SessionModel.UserDetailSession != null && SessionModel.UserDetailSession.UserTypeId == Convert.ToInt16(UserTypes.Admin))
            {
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else if (SessionModel.UserDetailSession != null && SessionModel.UserDetailSession.UserTypeId == Convert.ToInt16(UserTypes.User))
            {
                return RedirectToAction("Index", "User");
            }

            ViewBag.ShowEmailVerificationPop = false;
            var returnUrl = TempData["ReturnUrl"];
            LoginRequest model = new()
            {
                ReturnUrl = returnUrl == null ? string.Empty : (string)returnUrl,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginViewModel)
        {
            ViewBag.ShowEmailVerificationPop = false;
            var retrurnUrl = loginViewModel.ReturnUrl;

            var context = _httpContextAccessor.HttpContext;
            SiteKeys.UTCOffset = context?.Request.Cookies["timezoneoffset"];

            if (Request.Cookies["timezoneoffset"] != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32("UtcOffsetInSecond", Convert.ToInt32(Request.Cookies["timezoneoffset"]) * 60);
            }

            var getUserDetail = await _accountService.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password);

            if (getUserDetail is null || getUserDetail.ObjUser is null)
            {
                ViewBag.message = ResourceString.InvalidPassword;
                return View(loginViewModel);
            }

            if (getUserDetail.Succeeded)
            {
                if (getUserDetail.ObjUser.IsDeleted)
                {
                    ViewBag.message = ResourceString.UserAccountDeleted;
                }
                else if (!getUserDetail.ObjUser.IsActive)
                {
                    ViewBag.message = ResourceString.UserIsNotActive;
                }
                else if (!getUserDetail.ObjUser.IsEmailVerified)
                {
                    ViewBag.ShowEmailVerificationPop = true;
                    ViewBag.message = ResourceString.EmailNotVerified;
                }
                else
                {
                    //Store value in session
                    UserDetailSessionModel sessionobj = new();
                    sessionobj.UserId = getUserDetail.ObjUser.UserId;
                    sessionobj.Name = getUserDetail.ObjUser.Name;
                    sessionobj.UserTypeId = getUserDetail.ObjUser.UserType;
                    sessionobj.EmailId = getUserDetail.ObjUser.Email;
                    SessionModel.UserDetailSession = sessionobj;

                    if (getUserDetail.ObjUser.UserType == Convert.ToInt16(UserTypes.Admin))
                    {
                        if (retrurnUrl != null)
                        {
                            return Redirect(retrurnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User", new { area = "Admin" });
                        }
                    }
                    if (getUserDetail.ObjUser.UserType == Convert.ToInt16(UserTypes.User))
                    {
                        if (retrurnUrl != null)
                        {
                            return Redirect(retrurnUrl);
                        }
                        else
                        {
                            return RedirectToAction("GetMyProfile", "User", new { area = "" });
                        }
                    }
                    else
                    {
                        ViewBag.message = ResourceString.NotAuthorized;
                    }
                }
            }

            else
            {
                ViewBag.message = ResourceString.InvalidPassword;
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Route("SignUp")]
        public IActionResult Registration()
        {
            RegistrationRequest registrationRequest = new();
            return View(registrationRequest);
        }

        [HttpPost]
        public async Task<ActionResult> Registration(RegistrationRequest request)
        {
            var registrationResponse = await _accountService.Registration(request);

            switch (registrationResponse)
            {
                case ResponseTypes.Success:
                    return Ok(new TResponse<int> { ResponseMessage = ResourceString.SignUp });

                case ResponseTypes.EmailAlreadyExists:
                    return BadRequest(new TResponse<int> { ResponseMessage = ResourceString.EmailExists });

                default:
                    return NotFound(new TResponse<int> { ResponseMessage = ResourceString.SignUp });
            }
        }
        public async Task<IActionResult> EmailVerification(string token)
        {
            TokenRequest request = new();
            request.Token = token;
            var isEmailVerified = await _accountService.VerifyEmail(request);
            if (isEmailVerified.Data)
            {
                ViewBag.EmailVerified = true;
            }
            else
            {
                ViewBag.EmailVerified = false;
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(ForgotPasswordRequestModel model)
        {
            var getUserByEmail = await _accountService.FindByEmailAsync(model.Email);

            if (getUserByEmail != null)
            {
                if (getUserByEmail.UserType == Convert.ToInt64(UserTypes.Admin) || getUserByEmail.UserType == Convert.ToInt64(UserTypes.User))
                {
                    var updateResetToken = await _accountService.ResetPasswordTokenAsync(Convert.ToInt64(getUserByEmail.UserId));

                    if (updateResetToken.ResponsePacket != null)
                    {
                        return Ok(new TResponse<string> { ResponseMessage = updateResetToken.ResponseMessage });
                    }
                    else
                    {
                        return BadRequest(new TResponse<string> { ResponseMessage = updateResetToken.ResponseMessage });
                    }
                }
                else
                {
                    return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.InvalidRequest });
                }
            }
            else
            {
                return NotFound(new TResponse<string> { ResponseMessage = ResourceString.UserNotExist });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            ResetPasswordModel model = new();
            model.Token = token;
            if (!string.IsNullOrEmpty(token))
            {
                var isResetTokenExists = await _accountService.CheckResetPasswordTokenExist(token);
                model.IsInvaildToken = isResetTokenExists;
            }
            else
            {
                model.IsInvaildToken = false;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            var passwordSetup = await _accountService.ResetPassword(model);

            if (passwordSetup == ResponseTypes.Success)
            {
                return Ok(new TResponse<bool> { ResponseMessage = ResourceString.PasswordUpdated });
            }
            else if (passwordSetup == ResponseTypes.OldNewPasswordMatched)
            {
                return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.OldNewPasswordNotSame });
            }
            else
                return NotFound(new TResponse<bool> { ResponseMessage = ResourceString.InvalidOrResetTokenExpired });
        }
        public IActionResult PasswordSuccess()
        {
            return View();
        }


        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> VerifyEmail(VerifyEmailRequestModel model)
        {
            var getUserByEmail = await _accountService.FindByEmailAsync(model.Email);
            if (getUserByEmail != null)
            {
                if (getUserByEmail.UserType == Convert.ToInt64(UserTypes.Admin) || getUserByEmail.UserType == Convert.ToInt64(UserTypes.User))
                {
                    if (getUserByEmail.IsEmailVerified)
                    {
                        return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.EmailAlreadyVerified });
                    }
                    else if (getUserByEmail.IsDeleted ?? false)
                    {
                        return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.UserAccountDeleted });
                    }
                    else if (!(getUserByEmail.IsActive ?? false))
                    {
                        return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.DeactivateUser });
                    }
                    else
                    {
                        var updateEmailVerificationToken = await _accountService.UpdateEmailVerificationToken(Convert.ToInt64(getUserByEmail.UserId), getUserByEmail.Email ?? string.Empty, getUserByEmail.FirstName ?? string.Empty);
                        if (updateEmailVerificationToken != null)
                        {
                            if (updateEmailVerificationToken.Data)
                            {
                                return Ok(new TResponse<string> { ResponseMessage = updateEmailVerificationToken.Message });
                            }
                            else
                            {
                                return BadRequest(new TResponse<string> { ResponseMessage = updateEmailVerificationToken.Message });
                            }
                        }
                        else
                        {
                            return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.VerificationLinkSentFailed });
                        }
                    }
                }
                else
                {
                    return BadRequest(new TResponse<string> { ResponseMessage = ResourceString.InvalidRequest });
                }
            }
            else
            {
                return NotFound(new TResponse<string> { ResponseMessage = ResourceString.UserNotExist });
            }
        }
    }
}
