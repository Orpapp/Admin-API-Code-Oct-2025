using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Progress;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Model.Result;
using System;

namespace Data.IRepository
{
    public interface IProgressRepository : IGenericRepository<Progress>
    {
        Task<(List<CategoryList> categoryList, UserProgressInfo userProgress,List<ProgressTaskList> progressTaskLists)> GetUserProgress(int userId);

        Task<List<UserProductivityHours>> GetUserProductivityHours(int userId, short filterBy);

        Task<List<UserWeeklySummaryReport>> GetUserWeeklySummaryReport(int userId);
    }
}