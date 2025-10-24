using Shared.Model.Base;

namespace Business.IServices
{
    public interface IHangFireService
    {
        Task<ApiResponse<bool>> ScheduleReminder();
        Task<ApiResponse<bool>> ScheduleTaskReminder();
        Task<ApiResponse<bool>> NotAnyTaskCompleteReminder();
        Task<ApiResponse<bool>> UpdateUserStreak();
    }
}
