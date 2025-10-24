using Business.IServices;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Resources;
using Web.Areas.Admin.Controllers.Base;

namespace Web.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserController : AdminBaseController
    {
        private readonly IManageService _manageService;
        public UserController(IManageService manageService)
        {
            _manageService = manageService;
        }
        public IActionResult Index()
        {
            UserListDto userlst = new UserListDto();
            ViewBag.PageIndex = SiteKeys.DefultPageNumber;
            ViewBag.PageSize = SiteKeys.DefultPageSize;
            return View(userlst);
        }

        [HttpPost]
        public async Task<JsonResult> Index(DataTableParameters param)
        {
            UsersRequestModel model = new UsersRequestModel(param);
            var items = await _manageService.UserList(model);
            var users = items.ResponsePacket;
            var result = new DataTableResult<UserListDto>
            {
                Draw = param.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0
            };
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(long? id)
        {
            if (id > 0)
            {
                var userDetails = await _manageService.GetUserDetails(id.Value);
                if (userDetails.ResponsePacket?.UserId > 0 && userDetails.ResponsePacket?.UserType == (int)UserTypes.User)
                    return View(userDetails.ResponsePacket);
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public async Task<IActionResult> Detail(UserDetailsDto requestmodel)
        {
            var updateUser = await _manageService.UpdateUserDetail(requestmodel);
            return Json(updateUser);
        }
        public IActionResult GetTechnoEnumValues(string term)
        {
            var technoEnumValues = Enum.GetNames(typeof(AutoCompleteEnum))
                                      .Where(item => item.Contains(term, StringComparison.OrdinalIgnoreCase))
                                      .OrderBy(item => item)
                                      .ToList();

            return Json(technoEnumValues);
        }

        [HttpGet]
        public IActionResult AddForm()
        {
            AddFormRequestModel addFormRequestModel = new();
            addFormRequestModel.RadioButtonValue = true;

            addFormRequestModel = new AddFormRequestModel
            {
                SubjectsList = Enum.GetNames(typeof(CheckboxOptionsEnum)).ToList(),

                SelectedProfile = new List<string> { MultiselectDropdownEnum.Devloper.ToString(), MultiselectDropdownEnum.Designer.ToString() }
            };

            return View(addFormRequestModel);
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
            var changeAdminPassword = await _manageService.ChangePassword(new ChangePasswordModel
            {
                ConfirmPassword = model.ConfirmPassword,
                NewPassword = model.NewPassword,
                OldPassword = model.OldPassword
            }, UserId);

            return changeAdminPassword switch
            {
                ResponseTypes.Success => Ok(new TResponse<bool> { ResponseMessage = ResourceString.PasswordUpdated }),
                ResponseTypes.OldPasswordWrong => BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.WrongOldPassword }),
                _ => BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.FailedToSetNewPassword }),
            };
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus(long userId, bool activeStatus, bool deleteStatus, bool isActiveStatusChange)
        {
            var changeUserStatus = await _manageService.ChangeUserStatus(userId, activeStatus, deleteStatus);
            if (changeUserStatus > 0)
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return Ok(new TResponse<bool> { ResponseMessage = ResourceString.UserActivated });
                    }
                    else
                    {
                        return Ok(new TResponse<bool> { ResponseMessage = ResourceString.UserDeactivate });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return Ok(new TResponse<bool> { ResponseMessage = ResourceString.DeleteUser });
                    }
                    else
                    {
                        return Ok(new TResponse<bool> { ResponseMessage = ResourceString.RecoverUser });
                    }
                }
            }
            else
            {
                if (isActiveStatusChange)
                {
                    if (activeStatus)
                    {
                        return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.UserActivateFailed });
                    }
                    else
                    {
                        return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.UserDeactivateFailed });
                    }
                }
                else
                {
                    if (deleteStatus)
                    {
                        return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.UserDeleteFailed });
                    }
                    else
                    {
                        return BadRequest(new TResponse<bool> { ResponseMessage = ResourceString.UserAccountRecoverFailed });
                    }
                }
            }
        }
    }
}
