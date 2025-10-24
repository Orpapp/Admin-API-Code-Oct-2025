using Shared.Model.Request.SendTasks;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface ISendTasksRepository
    {
        Task<int> SendTaskToFriend(SendTask request, int senderId);

        Task<int> AcceptReceivedTask(AcceptTask request, int receiverId);

        Task<(List<ReceivedTaskResponse> tasks, List<SubTaskResponse> subTasks)> GetReceivedTasksWithSubTasks(int userId);
    }
}
