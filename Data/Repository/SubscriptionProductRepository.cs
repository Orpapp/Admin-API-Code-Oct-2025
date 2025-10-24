using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.Request.Shop;
using Shared.Model.Response; 
using System.Data; 

namespace Data.Repository
{
    public class SubscriptionProductRepository : GenericRepository<Shop>, ISubscriptionProductRepository
    {
        private readonly IDbConnection _dbConnection;
        public SubscriptionProductRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }
        public async Task<List<ShopProductResponse>> SubscriptionProductListByType(ProductTypes productType)
        {
            var parms = new
            {
                @ProductType = (byte)productType,
            };
            return (await _dbConnection.QueryAsync<ShopProductResponse>("[SubscriptionProductListByType]", param: parms, commandType: CommandType.StoredProcedure)).AsList();
        }
    }
}
