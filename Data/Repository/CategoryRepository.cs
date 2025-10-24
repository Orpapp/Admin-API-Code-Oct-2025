using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO.Admin.Category;
using Shared.Model.Request.Admin.Category;
using Shared.Model.Request.Category;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IDbConnection _dbConnection;
        public CategoryRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<int> ChangeStatus(int id, bool isActive)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @Id = id,
                @IsActive = isActive
            });
            return await _dbConnection.ExecuteAsync("CategoryStatusChange", param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<CategoryDto>> GetCategories(CategoryDetailsModel request)
        { 
            return (await _dbConnection.QueryAsync<CategoryDto>("CategoryList", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<CategoryResponse>> GetCategoriesForApp(CategoryRequest request)
        {
            return (await _dbConnection.QueryAsync<CategoryResponse>("CategoryListForApp", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }


    }
}
