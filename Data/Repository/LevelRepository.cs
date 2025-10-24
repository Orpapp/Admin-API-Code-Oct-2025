using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        private readonly string _getLevelImage = @"Select [ImageName] as LevelPicture From LevelBackgroundImage";

        private readonly IDbConnection _dbConnection;

        public LevelRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<List<GetStopageListResponse>> GetStoppageList(int userId)
        {
            var parms = new
            {
                @UserId = userId
            };

            return (await _dbConnection.QueryAsync<GetStopageListResponse>("GetStoppageList", param: parms, commandType: CommandType.StoredProcedure)).ToList();

        }


        public async Task<GetUserLevelResponse> GetUserLevel(int userId)
        {
            var parms = new
            {
                @UserId = userId
            };

            return (await _dbConnection.QueryFirstOrDefaultAsync<GetUserLevelResponse>("GetUserLevel", param: parms, commandType: CommandType.StoredProcedure));
        }

        public async Task<GetUserRewardResponse> GetUserReward(int userId)
        {
            var parms = new
            {
                @UserId = userId
            };
            return (await _dbConnection.QueryFirstOrDefaultAsync<GetUserRewardResponse>("GetUserReward", param: parms, commandType: CommandType.StoredProcedure));
        }

        public async Task<bool> UpdateLevelBackgroundImage(string? fileName, bool imageDeleted)
        {
            var parms = new
            {
                @ImageName = fileName,
                @IsImageDeleted= imageDeleted
            };

            return (await _dbConnection.QueryFirstOrDefaultAsync<bool>("UpdateLevelBackgroundImage", param: parms, commandType: CommandType.StoredProcedure));
        }

        public async Task<LevelImage> GetLevelImage()
        {
            var parms = new
            {
            };

            return (await _dbConnection.QueryFirstOrDefaultAsync<LevelImage>(_getLevelImage, param: parms, commandType: CommandType.Text));
        }
    }
}