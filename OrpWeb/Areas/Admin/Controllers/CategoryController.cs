using Business.IServices;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shared.Common;
using Shared.Model.DTO.Admin.Category;
using Shared.Model.Request.Admin.Category;
using Shared.Resources;
using Web.Areas.Admin.Controllers.Base;

namespace Web.Areas.Admin.Controllers
{
    [Area(areaName: "Admin")]
    [ValidateModel]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class CategoryController : AdminBaseController
    {
        private readonly ICategoryServices _category;
        public CategoryController(ICategoryServices category)
        {
            _category = category;
        }
        public IActionResult Index()
        {
            CategoryDto categories = new CategoryDto();
            ViewBag.PageIndex = SiteKeys.DefultPageNumber;
            ViewBag.PageSize = SiteKeys.DefultPageSize;
            return View(categories);
        }

        [HttpPost]
        public async Task<JsonResult> Index(DataTableParameters param)
        {
            CategoryDetailsModel model = new CategoryDetailsModel(param);
            var items = await _category.GetCategories(model);
            var users = items.Data;
            var result = new DataTableResult<CategoryDto>
            {
                Draw = param.Draw,
                Data = users,
                RecordsFiltered = users?.FirstOrDefault()?.TotalRecord ?? 0,
                RecordsTotal = users?.Count() ?? 0
            };
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> AddUpdate(int id = 0)
        {
            Category categoryDetail = new Category();
            if (id > 0)
            {
                categoryDetail = await _category.GetCategoryById(id);
                return View(categoryDetail);
            }
            return View(categoryDetail);
        }

        [HttpPost]
        public async Task<IActionResult> AddUpdate(Category request)
        {
            TResponse<int> response = new();
            response.ResponsePacket = await _category.AddUpdateCategories(request);

            if (response.ResponsePacket <= 0)
            {
                response.ResponseMessage = ResourceString.Error;
            }
            else if (request.Id == 0)
            {
                response.ResponseMessage = ResourceString.CategoryAddedSuccess;
            }
            else
            {
                response.ResponseMessage = ResourceString.CategoryUpdatedSuccess;
            }
            return Json(response);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, bool isActive)
        {
            await _category.ChangeStatus(id, isActive);
            if (isActive)
            {
                return Ok(new TResponse<bool> { ResponseMessage = ResourceString.CategoryActivated });
            }
            else
            {
                return Ok(new TResponse<bool> { ResponseMessage = ResourceString.CategoryDeactivate });
            }
        }
    }
}
