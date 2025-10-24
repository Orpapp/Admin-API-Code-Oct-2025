using Api.Controllers.Base;
using Api.Helper;
using Asp.Versioning;
using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model.Request.Category;
using System.Net;


namespace Api.Controllers.V1
{
    [AttributeWebAPI]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CategoryController : ApiBaseController
    {
        private readonly ICategoryServices _categoryService;
        public CategoryController(ICategoryServices categoryService)
        {
            _categoryService = categoryService;
        }

        [Route("Categories")]
        [HttpPost]
        public async Task<IActionResult> Categories([FromBody] CategoryRequest request)
        {
            var response = await _categoryService.GetCategoriesForApp(request);
            if (response.Data == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent, response);
            }
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
