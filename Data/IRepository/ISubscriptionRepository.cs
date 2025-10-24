using Shared.Model.DTO.Admin.UserSubscription;
using Shared.Model.Request.Admin.UserSubscription;
using Shared.Model.Request.UserSubscription;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface ISubscriptionRepository : IGenericRepository<UserSubscription>
    {
        Task<List<AppSubscriptionPlans>> GetAppSubscriptionPlans();
        Task<int> SaveUserSubscription(UserSubscription model);
        Task<UserSubscriptionDetails> GetUserSubscription(long userId);
        Task<List<UserSubscriptionDto>> GetSubscriptions(UserSubscriptionDetailsModel request);
        Task<bool> UpdateUserSubscriptionExpiryDate(int id, long userId, DateTime? expiryDate, bool isExpired);
    }
}

