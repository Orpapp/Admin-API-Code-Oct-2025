using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.DTO.Admin.Shop;
using Shared.Model.Request.Admin.Shop;
using Shared.Model.Request.Items;
using Shared.Model.Request.Shop;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class ShopRepository : GenericRepository<Shop>, IShopRepository
    {
        private readonly IDbConnection _dbConnection;
        public ShopRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }
        public async Task<List<ShopItems>> GetItemListByType(ItemsTypes itemType)
        {
            var parms = new
            {
                @itemType = itemType,
            };
            return (await _dbConnection.QueryAsync<ShopItems>("[GetItemListByType]", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<List<Shop>> GetShopListByUserId(int userId)
        {
            var parms = new
            {
                @UserId = userId,
            };

            return (await _dbConnection.QueryAsync<Shop>("[GetShopListByUserId]", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }
        public async Task<int> AddUpdateShop(List<Shop> shops, int userId)
        {
            var parms = new DynamicParameters();
            parms.AddTable("@ItemIds", "ItemIdTableType", shops);
            parms.Add("@UserId", userId);            
            return await _dbConnection.ExecuteScalarAsync<int>($"AddUpdateShop", parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<List<ShopDto>> GetShops(ShopDetailsModel request)
        {
            return (await _dbConnection.QueryAsync<ShopDto>("ShopList", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }
        public async Task<AvailableBalanceResponse> GetAvailableBalance(int userId)
        {
            var parms = new
            {
                @UserId = userId,
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<AvailableBalanceResponse>("[GetAvailableBalance]", param: parms, commandType: CommandType.StoredProcedure);
        }
    }
}