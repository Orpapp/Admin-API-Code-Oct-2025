using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Resources;
using Web.Controllers.Base;

namespace Web.Controllers
{
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : UserBaseController
    {
        private readonly IManageService _manageService;
        public UserController(IManageService manageService)
        {
            _manageService = manageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetMyProfile()
        {
            if (UserId > 0)
            {
                var customerDetail = await _manageService.GetUserDetails(UserId);
                return View(customerDetail.ResponsePacket);
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
        }

        public async Task<IActionResult> UpdateUserDetail(UserDetailsDto request)
        {
            var response = await _manageService.UpdateUserDetail(request);
            return Json(response);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordModelWeb model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModelWeb model)
        {
            var changePasswordResponse = await _manageService.ChangePassword(new ChangePasswordModel
            {
                ConfirmPassword = model.ConfirmPassword,
                NewPassword = model.NewPassword,
                OldPassword = model.OldPassword
            }, UserId);

            if (changePasswordResponse == ResponseTypes.Success)
            {
                return Ok(new TResponse<bool> { ResponseMessage = ResourceString.PasswordUpdated });
            }
            else if (changePasswordResponse == ResponseTypes.OldPasswordWrong)
            {
                return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.WrongOldPassword });
            }
            else if (changePasswordResponse == ResponseTypes.OldNewPasswordMatched)
            {
                return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.OldNewPasswordNotSame });
            }
            else
            {
                return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.FailedToSetNewPassword });
            }
        }
    }
}
