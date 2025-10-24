using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Common;
using Shared.Model.DTO.Admin.UserSubscription;
using Shared.Model.Request.Admin.UserSubscription;
using Shared.Model.Request.UserSubscription;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class SubscriptionRepository : GenericRepository<UserSubscription>, ISubscriptionRepository
    {
        private readonly IDbConnection _dbConnection;
        public SubscriptionRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<List<AppSubscriptionPlans>> GetAppSubscriptionPlans()
        {
            return (await _dbConnection.QueryAsync<AppSubscriptionPlans>("GetAppSubscriptionPlans", null, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<int> SaveUserSubscription(UserSubscription model)
        {
            var parms = new
            {
                @UserId = model.UserId,
                @ProductId = model.ProductId,
                @ProductType = model.ProductType,
                @PlanMonth = model.PlanMonth,
                @Price = model.Price,
                @Receipt = model.Receipt,
                @TransactionId = model.TransactionId,
                @PurchaseDate = model.PurchaseDate,
                @ExpiryDate = model.ExpiryDate,
                @DeviceType = model.DeviceType,
                @SharedSecret = model.SharedSecret,
                @SubscriptionProductId = model.SubscriptionProductId,
                @InAppSubscriptionProductId = model.InAppSubscriptionProductId,
                @PriceWithCurrencySymbol = model.PriceWithCurrencySymbol,
            };
            return await _dbConnection.ExecuteAsync("[SaveUserSubscription]", param: parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<UserSubscriptionDetails> GetUserSubscription(long userId)
        {
            var parms = new
            {
                userId
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<UserSubscriptionDetails>("GetUserSubscription", param: parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<bool> UpdateUserSubscriptionExpiryDate(int id, long userId, DateTime? expiryDate, bool isExpired)
        {
            var parms = new
            {
                id,
                userId,
                expiryDate,
                isExpired
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<bool>("UpdateUserSubscriptionExpiryDate", param: parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<List<UserSubscriptionDto>> GetSubscriptions(UserSubscriptionDetailsModel request)
        {
            return (await _dbConnection.QueryAsync<UserSubscriptionDto>("SubscriptionList", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }
    }
}