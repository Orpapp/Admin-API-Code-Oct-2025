using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IHangFireRepository : IGenericRepository<Reminder>
    {
        Task<IEnumerable<Reminder>> ScheduleReminder();
        Task<IEnumerable<TaskReminder>> ScheduleTaskReminder();
        Task<int> UpdateTask(int taskId); 
        Task<IEnumerable<Reminder>> NotAnyTaskCompleteReminder();
        Task<int> UpdateUserStreak();
    }
}

