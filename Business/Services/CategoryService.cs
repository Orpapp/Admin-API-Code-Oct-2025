using Business.IServices;
using Data.IRepository;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.Category;
using Shared.Model.Request.Admin.Category;
using Shared.Model.Request.Category;
using Shared.Model.Response;
using Shared.Resources;

namespace Business.Services
{
    public class CategoryService : ICategoryServices
    {

        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> AddUpdateCategories(Category request)
        {
            return await _categoryRepository.AddUpdate(request);
        }

        public async Task<int> ChangeStatus(int id, bool isActive)
        {
            return await _categoryRepository.ChangeStatus(id, isActive);
        }

        public async Task<int> Delete(int id)
        {
            return await _categoryRepository.Delete(id);
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetCategories(CategoryDetailsModel request)
        {
            ApiResponse<List<CategoryDto>> response = new();
            var getCategoryList = await _categoryRepository.GetCategories(request);
            response.Data = getCategoryList;
            response.Message = ResourceString.Success; 
            return response;
        }

        public Task<Category> GetCategoryById(int id)
        {
            return _categoryRepository.GetById(id);
        }


        public async Task<ApiResponse<List<CategoryResponse>>> GetCategoriesForApp(CategoryRequest request)
        { 
            var getCategoryList = await _categoryRepository.GetCategoriesForApp(request);
            return new ApiResponse<List<CategoryResponse>>(getCategoryList, message: ResourceString.Success, apiName: "GetCategoriesForApp");
        }

    }
}
