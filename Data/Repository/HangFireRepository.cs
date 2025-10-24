using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO.Account;
using Shared.Model.Response;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class HangFireRepository : GenericRepository<Reminder>, IHangFireRepository
    {
        private readonly IDbConnection _dbConnection;
        public HangFireRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<IEnumerable<Reminder>> ScheduleReminder()
        {
            return await _dbConnection.QueryAsync<Reminder>("GetNextFiveMinutesNotificationData", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TaskReminder>> ScheduleTaskReminder()
        {
            return await _dbConnection.QueryAsync<TaskReminder>("GetNextFiveMinutesNotificationDataForTask", commandType: CommandType.StoredProcedure);
        }
        public async Task<int> UpdateTask(int taskId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @TaskId = taskId
            }); 
            return await _dbConnection.ExecuteAsync("UpdateTaskNotification", param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Reminder>> NotAnyTaskCompleteReminder()
        {
            return await _dbConnection.QueryAsync<Reminder>("GetNotAnyTaskCompleteReminder", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateUserStreak()
        {
            return await _dbConnection.ExecuteAsync("UpdateUserStreak", commandType: CommandType.StoredProcedure);
        }
    }
}
