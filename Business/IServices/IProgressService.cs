using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IProgressService
    {
        Task<ApiResponse<ProgressResponse>> GetUserProgress(int userId);

        Task<ApiResponse<UserProductivityHoursResponse>> GetUserProductivityHours(int userId, short filterBy);

        Task<ApiResponse<List<WeeklySummaryResponse>>> GetUserWeeklySummaryReport(int userId);
    }
}
