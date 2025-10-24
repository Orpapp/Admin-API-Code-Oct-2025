using Shared.Model.Request.Task;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface ITaskRepository : IGenericRepository<Job>
    {
        Task<(List<TaskResponse> tasks, List<SubTaskResponse> subTasks)> GetTasksWithSubTasks(int userId, DateTime startDateUtc, DateTime endDateUtc);
        Task<int> AddSubTask(List<SubTask> request);
        Task<int> CompleteSub(int id);
        Task<int> UpdateSubTask(TaskRequest request);
        Task<List<TasksWithAvailabilityResponse>> GetTasksWithAvailability(int userId, DateTime startDateUtc, DateTime endDateUtc);
        Task<int> DeleteTask(int id, DateTime? scheduledDate, bool isDeleteFuture = false);
    }
}

