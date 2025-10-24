using Business.IServices;
using Data.IRepository;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using Shared.Resources;
using System.Data;
using System.Globalization;
namespace Business.Services
{
    public class TaskService : ITaskService
    {

        private readonly ITaskRepository _taskRepository;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICommonService _session;
        private readonly IProgressRepository _progressRepository;
        public TaskService(ITaskRepository taskRepository, ICommonService session, ISubscriptionService subscriptionService, IProgressRepository progressRepository)
        {
            _taskRepository = taskRepository;
            _session = session;
            _subscriptionService = subscriptionService;
            _progressRepository = progressRepository;
        }
        public async Task<ApiResponse<List<TaskResponse>>> GetTasksWithSubTasks(int userId, GetTasksRequest request)
        {
            DateTime startDateUTC = Convert.ToDateTime(request.StartDate);
            DateTime endDateUTC = Convert.ToDateTime(request.EndDate);

            var tasks = await _taskRepository.GetTasksWithSubTasks(userId, startDateUTC, endDateUTC);
            if (tasks.subTasks.Any() && tasks.tasks.Any())
            {
                foreach (var taskDetail in from TaskResponse taskDetail in tasks.tasks
                                           where tasks.subTasks.Any()
                                           select taskDetail)
                {

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
            return new ApiResponse<List<TaskResponse>>(tasks.tasks, message: ResourceString.Success, apiName: "TaskLIst");
        }

        public async Task<ApiResponse<bool>> AddSubTask(List<SubTask> request)
        {
            int rowCount = await _taskRepository.AddSubTask(request);
            if (rowCount > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.TaskAddedSuccess, apiName: "AddSubTask");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "AddSubTask");
        }

        public async Task<ApiResponse<ResponsewithId>> AddTask(Job request, int userId)
        {
            var result = new ResponsewithId();
            var response = await _subscriptionService.CheckUserSubscriptionStatus(userId) ?? new ApiResponse<UserSubscriptionResponse>();
          

            if (response.Data is not null && !(response.Data.IsActive))
            {
                var (categoryList, userProgress, progressTaskLists) = await _progressRepository.GetUserProgress(userId);
                if (userProgress.CreatedTaskByToday >= 7)
                {
                    return new ApiResponse<ResponsewithId>(result, message: ResourceString.DaliyTaskLimit, apiName: "AddTask");
                }
            }

            request.Offset = SiteKeys.UtcOffsetInSecond_API;

            if (!string.IsNullOrEmpty(request.StartTime))
            {
                request.StartDateUTC = _session.Convertdate(request.StartDate, request.StartTime);
                request.EndDateUTC = _session.Convertdate(request.EndDate, request.StartTime);
                request.IsAnyTime = false;
            }
            else
            {
                request.StartDateUTC = DateTime.SpecifyKind(Convert.ToDateTime(request.StartDate), DateTimeKind.Utc);
                request.EndDateUTC = DateTime.SpecifyKind(Convert.ToDateTime(request.EndDate), DateTimeKind.Utc);
                request.IsAnyTime = true;
            }

            try
            {
                int lastIdCount = await _taskRepository.AddUpdate(request);
                if (lastIdCount > 0)
                {
                    result.Id = lastIdCount;
                    result.Data = true;
                    return new ApiResponse<ResponsewithId>(result, message: ResourceString.TaskAddedSuccess, apiName: "AddTask");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResponsewithId>(result, message: ex.Message, apiName: "AddTak");

            }



            return new ApiResponse<ResponsewithId>(result, message: ResourceString.Error, apiName: "AddTak");
        }

        public async Task<ApiResponse<bool>> DeleteSub(int id, string scheduledDate, bool isDeleteFuture)
        {
            DateTime? scheduledDateUTC = string.IsNullOrEmpty(scheduledDate) ? null : Convert.ToDateTime(scheduledDate);
            int rowCount = await _taskRepository.DeleteTask(id, scheduledDateUTC, isDeleteFuture);
            if (rowCount > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.SubTaskDeleteSuccess, apiName: "DeleteSub");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeleteSub");
        }
        public async Task<ApiResponse<bool>> CompleteSub(int id)
        {
            int rowCount = await _taskRepository.CompleteSub(id);
            if (rowCount > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.SubTaskCompletedSuccess, apiName: "CompleteSub");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "CompleteSub");
        }

        public async Task<ApiResponse<bool>> UpdateSubTask(TaskRequest request)
        {
            request.StartDateUTC = _session.Convertdate(request.StartDate, request.StartTime);
            request.EndDateUTC = _session.Convertdate(request.EndDate, request.StartTime);
            request.UpdateFromDateUTC = string.IsNullOrEmpty(request.UpdateFromDate) ? null : _session.Convertdate(request.UpdateFromDate, request.StartTime);
            int rowCount = await _taskRepository.UpdateSubTask(request);
            if (rowCount > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.SubTaskUpdatedSuccess, apiName: "UpdateSubTask");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "UpdateSubTask");
        }


        public async Task<ApiResponse<CompleteAndIncompleteTaskList>> GetPendingAndCompletedTask(int userId, GetTasksRequest request)
        {
            CompleteAndIncompleteTaskList response = new CompleteAndIncompleteTaskList();
            DateTime startDateUTC = Convert.ToDateTime(request.StartDate);
            DateTime endDateUTC = Convert.ToDateTime(request.EndDate);

            var tasks = await _taskRepository.GetTasksWithSubTasks(userId, startDateUTC, endDateUTC);
            if (tasks.subTasks.Any() && tasks.tasks.Any())
            {
                foreach (var taskDetail in from TaskResponse taskDetail in tasks.tasks
                                           where tasks.subTasks.Any()
                                           select taskDetail)
                {

                    taskDetail.SubTask = tasks.subTasks.Where(x => x.TaskId == taskDetail.Id && x.ScheduledDate == taskDetail.StartDate).ToList();
                    var availableMinutes = taskDetail.SubTask.Sum(x => x.Duration);

                    DateTime endDate = DateTime.Parse(taskDetail.StartDate).AddMinutes(availableMinutes);
                    if (taskDetail.IsAnyTime)
                    {
                        DateTime startDate = DateTime.Parse(taskDetail.StartDate);
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
                        taskDetail.StartDate = _session.GetTimeAfterAddOffSet(DateTime.Parse(taskDetail.StartDate)).ToString();
                    }
                }
                response.PendingTaskList = tasks.tasks.Where(x => x.IsCompleted == 0).ToList();
                response.CompletedTaskList = tasks.tasks.Where(x => x.IsCompleted > 0).ToList();
            }


            return new ApiResponse<CompleteAndIncompleteTaskList>(response, message: ResourceString.Success, apiName: "GetPendingAndCompletedTask");
        }


        public async Task<ApiResponse<List<TasksWithAvailabilityResponse>>> GetTasksWithAvailability(int userId, GetTasksWithAvailabilityRequest request)
        {
            DateTime startDateUTC = Convert.ToDateTime(request.StartDate);
            DateTime endDateUTC = Convert.ToDateTime(request.EndDate);
            var response = await _taskRepository.GetTasksWithAvailability(userId, startDateUTC, endDateUTC);

            return new ApiResponse<List<TasksWithAvailabilityResponse>>(response, message: ResourceString.Success, apiName: "GetTasksWithAvailability");
        }
    }
}
