using Business.IServices;
using Data.IRepository;
using Shared.Model.Base;
using Shared.Model.Notification;
using Shared.Resources;
using Shared.Utility;

namespace Business.Services
{
    public class HangFireService : IHangFireService
    {

        private readonly IHangFireRepository _hangFireRepository;

        public HangFireService(IHangFireRepository hangFireRepository)
        {
            _hangFireRepository = hangFireRepository;

        }

        public async Task<ApiResponse<bool>> ScheduleReminder()
        {
            var reminders = await _hangFireRepository.ScheduleReminder();

            foreach (var item in reminders)
            {
                PushNotificationRequestModel pushNotificationRequestModel = new PushNotificationRequestModel
                {
                    DeviceToken = item.DeviceToken,
                    Title = item.Name,
                    Message = "",
                    MsgId = item.MsgId
                };
                await PushNotifications.SendPushNotification(pushNotificationRequestModel);
            }
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "ScheduleReminder");
        }


        public async Task<ApiResponse<bool>> ScheduleTaskReminder()
        {
            var reminders = await _hangFireRepository.ScheduleTaskReminder();
            foreach (var item in reminders)
            {
                string message = string.Empty;

                if (item.Reminder > 5)
                {
                    message = "Time's ticking! You've got approx " + item.Reminder + " minutes left to start your task " + item.Name;
                }
                else
                {
                    message = "Time's ticking! You've got approx " + item.Reminder + " hour left to start your task " + item.Name;
                }
                PushNotificationRequestModel pushNotificationRequestModel = new PushNotificationRequestModel
                {
                    DeviceToken = item.DeviceToken,
                    Title = item.Name,
                    Message = message,
                    NotificationType = "1"
                };
                await PushNotifications.SendPushNotification(pushNotificationRequestModel);
                await _hangFireRepository.UpdateTask(item.Id);
            }
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "ScheduleReminder");
        }
        public async Task<ApiResponse<bool>> NotAnyTaskCompleteReminder()
        {
            var reminders = await _hangFireRepository.NotAnyTaskCompleteReminder();

            if (reminders.Any())
            {
                foreach (var item in reminders)
                {
                    PushNotificationRequestModel pushNotificationRequestModel = new PushNotificationRequestModel
                    {
                        DeviceToken = item.DeviceToken,
                        Title = "Task Reminder",
                        Message = "You have not completed any tasks for today",
                        MsgId = 0
                    };
                    await PushNotifications.SendPushNotification(pushNotificationRequestModel);
                }
            }
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "NotAnyTaskCompleteReminder");
        }

        public async Task<ApiResponse<bool>> UpdateUserStreak()
        {
            await _hangFireRepository.UpdateUserStreak();
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "UpdateUserStreak");
        }
    }
}
