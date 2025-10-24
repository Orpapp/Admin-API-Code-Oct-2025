using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IRewardService
    {
        Task<ApiResponse<bool>> UpdateReward(Reward request);
        Task<ApiResponse<SpendTime>> GetSpendedTime(int request);
        Task<ApiResponse<List<LevelInfo>>> GetLevelInformation(int request);
        Task<TResponse<List<ManageLevelDto>>> GetAllLevel(LevelDetailsModel request);
    }
}
