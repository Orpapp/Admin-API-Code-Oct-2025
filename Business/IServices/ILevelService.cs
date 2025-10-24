using Microsoft.AspNetCore.Http;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Task;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface ILevelService
    {
        Task<ApiResponse<List<GetStopageListResponse>>> GetStoppageList(int userId);
        Task<ApiResponse<GetUserLevelResponse>> GetUserLevel(int userId);
        Task<ApiResponse<bool>> UpdateLevelBackgroundImage(IFormFile? file,bool imageDeleted);
        Task<ApiResponse<LevelImage>> GetLevelImage();
        Task<ApiResponse<GetUserRewardResponse>> GetUserReward(int userId);
    }
}