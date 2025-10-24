using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Resources;
using System;
using System.Reflection;

namespace Business.Services
{
    public class ProgressService : IProgressService
    {

        private readonly IProgressRepository _progressRepository;
        public ProgressService(IProgressRepository progressRepository)
        {
            _progressRepository = progressRepository;
        }


        public async Task<ApiResponse<ProgressResponse>> GetUserProgress(int userId)
        {
            ProgressResponse progressResponse = new();
            TaskCompleted taskCompleted = new();
            List<CategoryList> categoryLists = new();
            TimeSpend timeSpend = new();

            var (categoryList, userProgress, progressTaskLists) = await _progressRepository.GetUserProgress(userId);

            if (userProgress != null)
            {
                progressResponse.TotalWeeks = userProgress.TotalWeeks;
                progressResponse.JoinedDate = userProgress.JoinedDate;
                taskCompleted = new TaskCompleted()
                {
                    TotalTask = ((int)(userProgress.TotalTask == null ? 0 : userProgress.TotalTask)),
                    TodayCompleted = userProgress.TodayCompleted,
                    CreatedForToday = userProgress.CreatedForToday,
                    TotalCompletedTask = userProgress.TotalCompletedTask,
                };

                int hours = (int)(userProgress.TotalHourTaskCreated != null ? userProgress.TotalHourTaskCreated / 60 : 0);
                int minutes = (int)(userProgress.TotalHourTaskCreated != null ? userProgress.TotalHourTaskCreated % 60 : 0);
                if (hours > 0)
                {
                    timeSpend.TotalHourTaskCreated += hours + (hours > 1 ? " hrs " : " hr ");
                }

                if (minutes > 0)
                {
                    timeSpend.TotalHourTaskCreated += minutes + (minutes > 1 ? " mins" : " min");
                }

                if (timeSpend.TotalHourTaskCreated == "")
                {
                    timeSpend.TotalHourTaskCreated = "0 min";
                }

                progressResponse.TaskCompleted = taskCompleted;
            }

            if (categoryList != null)
            {
                foreach (var category in categoryList)
                {
                    CategoryList categories = new CategoryList();
                    categories.CatgoryName = category.CatgoryName;
                    categories.Color = category.Color;
                    categories.TotalHourSubTaskCreated = (int)(category.TotalHourSubTaskCreated != null ? Convert.ToInt64(category.TotalHourSubTaskCreated) : 0);


                    categoryLists.Add(categories);
                }

                timeSpend.Categories = categoryLists;
                progressResponse.TimeSpend = timeSpend;
            }
            progressResponse.ProgressTaskLists = progressTaskLists;
            return new ApiResponse<ProgressResponse>(progressResponse, message: ResourceString.Success, apiName: "GetSpendedTime");
        }

        public async Task<ApiResponse<UserProductivityHoursResponse>> GetUserProductivityHours(int userId, short filterBy)
        {
            UserProductivityHoursResponse response = new UserProductivityHoursResponse();

            var userProductivityHours = await _progressRepository.GetUserProductivityHours(userId, filterBy);

            if (!userProductivityHours.Any())
            {
                return new ApiResponse<UserProductivityHoursResponse>(null, ResourceString.Fail, "GetUserProductivityHours");
            }

            response.TotalDuration = userProductivityHours.Sum(x => Convert.ToInt32(x.Duration)).ToString();
            response.TotalDuration = CommonFunctions.FormatDuration(Convert.ToInt32(response.TotalDuration));

            foreach (var item in userProductivityHours)
            {
                item.Duration = ((Convert.ToDouble(item.Duration)) / 60).ToString("F2");

                if (filterBy == 1) // Filter by Week
                {
                    item.DayName = item.DayName?.Substring(0, 3) ?? null;
                }

                if (filterBy == 2) // Filter by Month
                {
                    item.WeekNumber = $"Week {item.WeekNumber}";
                }
            }

            response.UserProductivity = userProductivityHours;

            return new ApiResponse<UserProductivityHoursResponse>(response, ResourceString.Success, "GetUserProductivityHours");
        }

        public async Task<ApiResponse<List<WeeklySummaryResponse>>> GetUserWeeklySummaryReport(int userId)
        {
            var userWeeklySummary = await _progressRepository.GetUserWeeklySummaryReport(userId);

            if (!userWeeklySummary.Any())
            {
                return new ApiResponse<List<WeeklySummaryResponse>>(null, ResourceString.Fail, "GetUserWeeklySummaryReport");
            }

            var groupedByWeek = userWeeklySummary
                .GroupBy(s => new { s.WeekStart, s.WeekEnd })
                .OrderByDescending(g => g.Key.WeekStart)
                .Select((g, index) => new WeeklySummaryResponse
                {
                    WeekStart = g.Key.WeekStart,
                    WeekEnd = g.Key.WeekEnd,
                    //WeekNumber = $"Week {userWeeklySummary.Count - index}",
                    WeekNumber = $"Week {userWeeklySummary.GroupBy(s => new { s.WeekStart, s.WeekEnd }).Count() - index}",
                    TotalDuration = CommonFunctions.FormatDuration(g.Sum(x => Convert.ToInt32(x.Duration))),
                    TotalCompletedTaskCount = g.Select(x => x.TotalCompletedTaskCount).FirstOrDefault(),
                    Categories = g.Where(x => x.TotalCompletedTaskCount > 0).Select(x => new CategorySummary
                    {
                        CategoryName = x.CategoryName,
                        Duration = CommonFunctions.FormatDuration(Convert.ToInt32(x.Duration)),
                        //CompletedDurationPercentage = (Convert.ToInt32(x.Duration) / Convert.ToInt32(g.Sum(x => Convert.ToInt32(x.Duration)))) * 100
                        CompletedDurationPercentage = x.CompletedDurationPercentage
                    }).ToList()
                }).ToList();

            return new ApiResponse<List<WeeklySummaryResponse>>(groupedByWeek, ResourceString.Success, "GetUserWeeklySummaryReport");
        }

    }
}
