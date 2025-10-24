using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Resources;

namespace Business.Services
{
    public class RewardService : IRewardService
    {

        private readonly IRewardRepository _rewardRepository;
        public RewardService(IRewardRepository rewardRepository)
        {
            _rewardRepository = rewardRepository;
        }

        public async Task<ApiResponse<bool>> UpdateReward(Reward request)
        {
            var response = await _rewardRepository.AddUpdate(request);
            if (response == 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "UpdateReward");
            }
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "UpdateReward");
        }

        public async Task<ApiResponse<SpendTime>> GetSpendedTime(int request)
        {
            var response = await _rewardRepository.GetSpendedTime(request);
            if (response == null)
            {
                SpendTime spendTime = new SpendTime();
                spendTime.Time = "";
                spendTime.Id = 0;
                return new ApiResponse<SpendTime>(spendTime, message: ResourceString.Success, apiName: "GetSpendedTime");
            }
            int hours = response.SpendedTime / 60;
            int remainingMinutes = response.SpendedTime % 60;
            response.Time = $"{hours}h {remainingMinutes:D2}m";
            return new ApiResponse<SpendTime>(response, message: ResourceString.Success, apiName: "GetSpendedTime");
        }
        public async Task<ApiResponse<List<LevelInfo>>> GetLevelInformation(int request)
        {
            var response = await _rewardRepository.GetLevelInformation(request);
            return new ApiResponse<List<LevelInfo>>(response, message: ResourceString.Success, apiName: "GetLevelInformation");

        }

        public async Task<TResponse<List<ManageLevelDto>>> GetAllLevel(LevelDetailsModel request)
        {
            TResponse<List<ManageLevelDto>> response = new();
            var geList = await _rewardRepository.GetAllLevel(request);

            if (geList is null)
            {
                response.ResponsePacket = new List<ManageLevelDto>();
                response.ResponseMessage = ResourceString.Error;
                return response;
            }
            response.ResponsePacket = geList;
            response.ResponseMessage = ResourceString.Success;

            return response;
        }
    }
}
