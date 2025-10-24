using Shared.Model.Base;
using Shared.Model.Request.Task;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface ITaskService
    {
        Task<ApiResponse<List<TaskResponse>>> GetTasksWithSubTasks(int userId, GetTasksRequest request);
        Task<ApiResponse<bool>> DeleteSub(int id, string scheduledDate, bool isDeleteFuture);
        Task<ApiResponse<bool>> CompleteSub(int id);
        Task<ApiResponse<ResponsewithId>> AddTask(Job request,int userId);
        Task<ApiResponse<bool>> AddSubTask(List<SubTask> request);
        Task<ApiResponse<bool>> UpdateSubTask(TaskRequest request);

        Task<ApiResponse<CompleteAndIncompleteTaskList>> GetPendingAndCompletedTask(int userId, GetTasksRequest request);
        Task<ApiResponse<List<TasksWithAvailabilityResponse>>> GetTasksWithAvailability(int userId, GetTasksWithAvailabilityRequest request);

    }

}
