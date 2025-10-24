using Business.IServices;
using Data.IRepository;
using Data.Repository;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.SqlServer.Server;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.Notification;
using Shared.Model.Request.SendTasks;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using Shared.Resources;
using Shared.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class SendTasksService : ISendTasksService
    {
        private readonly ISendTasksRepository _sendTasksRepository;
        private readonly IAccountRepositry _accountRepositry;
        private readonly ITaskRepository _taskRepository;
        private readonly ICommonService _session;
        public SendTasksService(ISendTasksRepository sendTasksRepository, ICommonService session, IAccountRepositry accountRepositry, ITaskRepository taskRepository)
        {
            _sendTasksRepository = sendTasksRepository;
            _session = session;
            _accountRepositry = accountRepositry;
            _taskRepository = taskRepository;
        }

        public async Task<ApiResponse<bool>> SendTaskToFriend(SendTask request, int senderId)
        {
            var result = await _sendTasksRepository.SendTaskToFriend(request, senderId);

            if (result <= 0)
            {
                return new ApiResponse<bool>(false, ResourceString.TaskSendingFailed, "SendTaskToFriend");
            }

            var sender = await _accountRepositry.FindByIdAsync(senderId);
            var user = await _accountRepositry.FindByIdAsync(request.ReceiverId);
            var task = await _taskRepository.GetById(request.TaskId);
            PushNotificationRequestModel pushNotificationRequestModel = new PushNotificationRequestModel
            {
                DeviceToken = user.DeviceToken ?? "",
                Title = "New Task Alert!",
                Message = $"{sender?.Name ?? ""} has assigned a task to you: '{task?.Name ?? ""}'. Check it out and get started!",
                NotificationType = "2"
            };
            await PushNotifications.SendPushNotification(pushNotificationRequestModel);

            return new ApiResponse<bool>(true, ResourceString.TaskSentToFriend, "SendTaskToFriend");
        }

        public async Task<ApiResponse<bool>> AcceptReceivedTask(AcceptTask request, int receiverId)
        {
            var result = await _sendTasksRepository.AcceptReceivedTask(request, receiverId);

            if (result <= 0)
            {
                return new ApiResponse<bool>(false, ResourceString.TaskAcceptingFailed, "AcceptReceivedTask");
            }

            return new ApiResponse<bool>(true, ResourceString.TaskAccepted, "AcceptReceivedTask");
        }

        public async Task<ApiResponse<List<ReceivedTaskResponse>>> GetReceivedTasksWithSubTasks(int userId)
        {
            try
            {
                var tasks = await _sendTasksRepository.GetReceivedTasksWithSubTasks(userId);
                if (tasks.subTasks.Any() && tasks.tasks.Any())
                {
                    foreach (var taskDetail in from ReceivedTaskResponse taskDetail in tasks.tasks
                                               where tasks.subTasks.Any()
                                               select taskDetail)
                    {
                        taskDetail.FriendProfileImage = CommonFunctions.GetRelativeFilePath(taskDetail.FriendProfileImage, SiteKeys.UserImageFolderPath);
                        taskDetail.SubTask = tasks.subTasks.Where(x => x.TaskId == taskDetail.Id && x.ScheduledDate == taskDetail.StartDate).ToList();
                        var availableMinutes = taskDetail.SubTask.Sum(x => x.Duration);

                        DateTime endDate = Convert.ToDateTime(taskDetail.StartDate, CultureInfo.InvariantCulture).AddMinutes(availableMinutes);

                        if (taskDetail.IsAnyTime)
                        {
                            DateTime startDate = Convert.ToDateTime(taskDetail.StartDate, CultureInfo.InvariantCulture);
                            taskDetail.StartDate = startDate.ToString("M/d/yyyy");
                            taskDetail.EndDate = endDate.ToString("M/d/yyyy");
                            taskDetail.TaskStartDate = taskDetail.StartDateUTC.ToString();
                            taskDetail.TaskEndDate = taskDetail.EndDateUTC.AddMinutes(availableMinutes).ToString();
                        }
                        else
                        {
                            taskDetail.TaskStartDate = _session.GetTimeAfterAddOffSet(taskDetail.StartDateUTC).ToString();
                            taskDetail.TaskEndDate = _session.GetTimeAfterAddOffSet(taskDetail.EndDateUTC).AddMinutes(availableMinutes).ToString();
                            taskDetail.EndDate = _session.GetTimeAfterAddOffSet(endDate).ToString();
                            taskDetail.StartDate = _session.GetTimeAfterAddOffSet(Convert.ToDateTime(taskDetail.StartDate, CultureInfo.InvariantCulture)).ToString();
                        }
                    }
                }
                return new ApiResponse<List<ReceivedTaskResponse>>(tasks.tasks, message: ResourceString.Success, apiName: "ReceivedTaskLIst");
            }
            catch (Exception)
            {
                return new ApiResponse<List<ReceivedTaskResponse>>(null, message: ResourceString.Success, apiName: "ReceivedTaskLIst");
            }
        }
    }
}
