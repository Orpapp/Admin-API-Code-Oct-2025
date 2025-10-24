using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Progress;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Model.Result;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class ProgressRepository : GenericRepository<Progress>, IProgressRepository
    {
        private readonly IDbConnection _dbConnection;
        public ProgressRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<(List<CategoryList> categoryList, UserProgressInfo userProgress,List<ProgressTaskList>  progressTaskLists)> GetUserProgress(int userId)
        {
            var parms = new
            {
                @Id = userId
            };

            using (var result = await _dbConnection.QueryMultipleAsync("[GetUserProgress]", param: parms, commandType: CommandType.StoredProcedure))
            {
                var userProgress = (await result.ReadFirstOrDefaultAsync<UserProgressInfo>());
                var categoryList = (await result.ReadAsync<CategoryList>()).ToList();
                var progressTaskLists = (await result.ReadAsync<ProgressTaskList>()).ToList();
                _dbConnection.Dispose();

                return (categoryList, userProgress, progressTaskLists);
            }
        }

        public async Task<List<UserProductivityHours>> GetUserProductivityHours(int userId, short filterBy)
        {
            var parms = new
            {
                @UserId = userId,
                @FilterBy = filterBy
            };

            return (await _dbConnection.QueryAsync<UserProductivityHours>("[GetUserProductivityHours]", parms, commandType: CommandType.StoredProcedure)).AsList();
        }


        public async Task<List<UserWeeklySummaryReport>> GetUserWeeklySummaryReport(int userId)
        {
            var parms = new
            {
                @UserId = userId
            };

            return (await _dbConnection.QueryAsync<UserWeeklySummaryReport>("[GetUserWeeklySummaryReport]", parms, commandType: CommandType.StoredProcedure)).AsList();
        }

    }
}