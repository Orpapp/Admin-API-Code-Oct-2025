using Shared.Model.Base;
using Shared.Model.DTO.Admin.UserSubscription;
using Shared.Model.Request.Admin.UserSubscription;
using Shared.Model.Request.UserSubscription;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface ISubscriptionService
    {
        Task<ApiResponse<List<AppSubscriptionPlansResponse>>> GetAppSubscriptionPlans();
        Task<ApiResponse<List<UserSubscriptionDto>>> GetSubscriptions(UserSubscriptionDetailsModel request);
        Task<ApiResponse<bool>> SaveUserSubscription(UserSubscription subscriptionPlan, int userId);
        Task<ApiResponse<UserSubscriptionDetails>> GetUserSubscription(long userId);
        Task<ApiResponse<bool>> CancelUserSubscription(long userId);
        Task<ApiResponse<UserSubscriptionResponse>> CheckUserSubscriptionStatus(int userId);
    }

}
