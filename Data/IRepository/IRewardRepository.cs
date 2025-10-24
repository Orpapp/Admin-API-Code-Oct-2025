using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IRewardRepository : IGenericRepository<Reward>
    {

        Task<SpendTime> GetSpendedTime(int request);
        Task<List<LevelInfo>> GetLevelInformation(int request);
        Task<List<ManageLevelDto>> GetAllLevel(LevelDetailsModel request);
        
    }
}

