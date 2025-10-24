using Shared.Model.DTO.Admin.Category;
using Shared.Model.Request.Admin.Category;
using Shared.Model.Request.Category;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<CategoryDto>> GetCategories(CategoryDetailsModel request);
        Task<int> ChangeStatus(int id, bool isActive);
        Task<List<CategoryResponse>> GetCategoriesForApp(CategoryRequest request);
    }
}

