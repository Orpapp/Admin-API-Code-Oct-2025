using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.SendTasks;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class SendTasksRepository : ISendTasksRepository
    {
        private readonly IDbConnection _dbConnection;
        public SendTasksRepository(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<int> SendTaskToFriend(SendTask request, int senderId)
        {
            var parms = new
            {
                @TaskId = request.TaskId,
                @ReceiverId = request.ReceiverId,
                @SenderId = senderId
            };

            return await _dbConnection.ExecuteAsync("SendTaskToFriend", parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AcceptReceivedTask(AcceptTask request, int receiverId)
        {
            var parms = new
            {
                @TaskId = request.TaskId,
                @ReceiverId = receiverId
            };

            return await _dbConnection.ExecuteAsync("AcceptReceivedTask", parms, commandType: CommandType.StoredProcedure);
        }


        public async Task<(List<ReceivedTaskResponse> tasks, List<SubTaskResponse> subTasks)> GetReceivedTasksWithSubTasks(int userId)
        {
            var parms = new
            {
                @UserId = userId
            };

            using (var result = await _dbConnection.QueryMultipleAsync("[GetReceivedTasksWithSubTasks]", param: parms, commandType: CommandType.StoredProcedure))
            {
                var task = (await result.ReadAsync<ReceivedTaskResponse>()).ToList();
                var subTask = (await result.ReadAsync<SubTaskResponse>()).ToList();
                _dbConnection.Dispose();
                return (task, subTask);
            }
        }
    }
}
