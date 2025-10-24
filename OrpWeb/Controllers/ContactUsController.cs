using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Model.WebUser;

namespace Web.Controllers
{
    [AllowAnonymous, ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ContactUsController : Controller
    {
        private readonly IAccountService _accountService;
        public ContactUsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveContactUsDetails(ContactUsRequestModel requestModel)
        {
            var res = _accountService.SaveContactUsDetails(requestModel);
            return Json(res);
        }
    }
}
