using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class TaskRepository : GenericRepository<Job>, ITaskRepository
    {
        private readonly IDbConnection _dbConnection;
        public TaskRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }


        public async Task<(List<TaskResponse> tasks, List<SubTaskResponse> subTasks)> GetTasksWithSubTasks(int userId, DateTime startDateUtc, DateTime endDateUtc)
        {
            var parms = new
            {
                @UserId = userId,
                @StartDate = startDateUtc,
                @EndDate = endDateUtc
            };

            using (var result = await _dbConnection.QueryMultipleAsync("[GetTasksWithSubTasks]", param: parms, commandType: CommandType.StoredProcedure))
            {
                var task = (await result.ReadAsync<TaskResponse>()).ToList();
                var subTask = (await result.ReadAsync<SubTaskResponse>()).ToList();
                _dbConnection.Dispose();
                return (task, subTask);
            }


        }


        public async Task<int> AddSubTask(List<SubTask> request)
        {

            var parms = new DynamicParameters();
            parms.AddTable("@Tasks", "SubUpdateTaskType", request);
            return await _dbConnection.ExecuteAsync("AddSubTask", param: parms, commandType: CommandType.StoredProcedure);

        }
        public async Task<int> UpdateSubTask(TaskRequest request)
        {
            var parms = new DynamicParameters();
            parms.Add("@Id", request.Id);
            parms.Add("@Name", request.Name);
            parms.Add("@StartDateUTC", request.StartDateUTC);
            parms.Add("@EndDateUTC", request.EndDateUTC);
            parms.Add("@Type", request.Type);
            parms.Add("@Notes", request.Notes);
            parms.Add("@Duration", request.Duration);
            parms.Add("@Priority", request.Priority);
            parms.Add("@Reminder", request.Reminder);
            parms.Add("@CategoryId", request.CategoryId);
            parms.Add("@UpdateFromDate", request.UpdateFromDateUTC);
            parms.AddTable("@SubTasks", "SubUpdateTaskType", request.SubTask);

            return await _dbConnection.ExecuteAsync("UpdateSubTask_V1", param: parms, commandType: CommandType.StoredProcedure);


        }
        public async Task<int> CompleteSub(int id)
        {
            var parameters = new { Id = id };
            return await _dbConnection.ExecuteAsync("CompleteSubTask", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<TasksWithAvailabilityResponse>> GetTasksWithAvailability(int userId, DateTime startDateUtc, DateTime endDateUtc)
        {
            var parms = new
            {
                @UserId = userId,
                @StartDate = startDateUtc,
                @EndDate = endDateUtc
            };
            using (var result = await _dbConnection.QueryMultipleAsync("[GetTasksWithAvailability]", param: parms, commandType: CommandType.StoredProcedure))
            {
                var task = (await result.ReadAsync<TasksWithAvailabilityResponse>()).ToList();
                return task;
            }
        }
        public async Task<int> DeleteTask(int id, DateTime? scheduledDate, bool isDeleteFuture = false)
        {

            var parms = new
            {
                @Id = id,
                @ScheduledDate = scheduledDate,
                @IsDeleteFuture = isDeleteFuture
            };
            try
            {
                return await _dbConnection.ExecuteAsync($"DeleteJobById", parms, commandType: CommandType.StoredProcedure);

            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}