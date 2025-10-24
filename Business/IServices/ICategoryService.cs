using Shared.Model.Base;
using Shared.Model.DTO.Admin.Category;
using Shared.Model.Request.Admin.Category;
using Shared.Model.Request.Category;
using Shared.Model.Response;


namespace Business.IServices
{
    public interface ICategoryServices
    {
        Task<ApiResponse<List<CategoryDto>>> GetCategories(CategoryDetailsModel request);
        Task<int> Delete(int id);
        Task<int> AddUpdateCategories(Category request);
        Task<int> ChangeStatus(int id, bool isActive);
        Task<Category> GetCategoryById(int id);
        Task<ApiResponse<List<CategoryResponse>>> GetCategoriesForApp(CategoryRequest request);
    }
}
