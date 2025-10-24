using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using System.Data;
using System.Data.Common;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class RewardRepository : GenericRepository<Reward>, IRewardRepository
    {
        private readonly IDbConnection _dbConnection;
        public RewardRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<SpendTime> GetSpendedTime(int request)
        {
            var parms = new
            {
                @Id = request
            };

            return await _dbConnection.QueryFirstOrDefaultAsync<SpendTime>("GetSpendedTime", parms, commandType: CommandType.StoredProcedure);

        }
       public async Task<List<LevelInfo>> GetLevelInformation(int request)
        {
            var parms = new
            {
                @LevelID = request
            };

            return (await _dbConnection.QueryAsync<LevelInfo>("GetLevelInformation", parms, commandType: CommandType.StoredProcedure)).AsList();

        }


        public async Task<List<ManageLevelDto>> GetAllLevel(LevelDetailsModel request)
        {if (request.PageStart == 0)
            {
                request.PageStart = 1;
            }
            
            var parms = new
            {
                @PageNo = request.PageStart,
                @PageSize = request.PageSize,
                @SearchKeyword = request.SearchKeyword,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder
               
            };
            return (await _dbConnection.QueryAsync<ManageLevelDto>("GetAllManageLevels", parms, commandType: CommandType.StoredProcedure)).AsList();
        }

    }
}
