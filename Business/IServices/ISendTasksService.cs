using Shared.Model.Base;
using Shared.Model.Request.SendTasks;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.IServices
{
    public interface ISendTasksService
    {
        Task<ApiResponse<bool>> SendTaskToFriend(SendTask request, int senderId);

        Task<ApiResponse<bool>> AcceptReceivedTask(AcceptTask request, int receiverId);

        Task<ApiResponse<List<ReceivedTaskResponse>>> GetReceivedTasksWithSubTasks(int userId);
    }
}
