using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Task;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface ILevelRepository : IGenericRepository<Level>
    {
        Task<List<GetStopageListResponse>> GetStoppageList(int userId);       
        Task<GetUserLevelResponse> GetUserLevel(int userId);
        Task<bool> UpdateLevelBackgroundImage(string? fileName, bool imageDeleted);
        Task<LevelImage> GetLevelImage();
        Task<GetUserRewardResponse> GetUserReward(int userId);
    }
}