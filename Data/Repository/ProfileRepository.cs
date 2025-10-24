using Dapper;
using DapperParameters;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.Account;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using System.Data;
using static Dapper.SqlMapper;

namespace Data.Repository
{
    public class ProfileRepository : GenericRepository<Profile>, IProfileRepository
    {
        private readonly IDbConnection _dbConnection;
        public ProfileRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<int> AccountDelete(int id)
        {
            var parms = new
            {
                @Id = id
            };
            return await _dbConnection.ExecuteAsync("AccountDelete", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<ProfileDetailsResponse> GetUserProfileById(int id)
        {
            ProfileDetailsResponse response = new ProfileDetailsResponse();
            var parms = new
            {
                @Id = id
            };
            
            using (var result = await _dbConnection.QueryMultipleAsync("GetProfileDetailsById", param: parms, commandType: CommandType.StoredProcedure))
            {
                response.Profile = (await result.ReadAsync<ProfileDetails>()).FirstOrDefault();
                response.Friends = (await result.ReadAsync<UserFriend>()).AsList();
                response.Interests = (await result.ReadAsync<UserInterests>()).AsList();
                response.Stickers = (await result.ReadAsync<UserStickers>()).AsList();
                _dbConnection.Dispose();
                return response;
            }
        }

        public async Task<PreferenceModel> GetPreferenceById(int userId)
        {
            var parms = new
            {
                @Id = userId,
            };
            PreferenceModel preferenceModel = new PreferenceModel();
            using (var result = await _dbConnection.QueryMultipleAsync("[GetPreference]", param: parms, commandType: CommandType.StoredProcedure))
            {
                preferenceModel.Interest = (await result.ReadAsync<Item>()).ToList();
                preferenceModel.Motivate = (await result.ReadAsync<Item>()).ToList();
                preferenceModel.Difficult = (await result.ReadAsync<Item>()).ToList();
                _dbConnection.Dispose();
                return preferenceModel;
            }
        }
        public async Task<List<Item>> SearchPreference(string preference, int type)
        {
            var parms = new DynamicParameters();
            parms.Add("@Preference", preference);
            parms.Add("@Type", type);

            // Execute the stored procedure and map results to a list of Item objects
            var preferenceModel = (await _dbConnection.QueryAsync<Item>("SearchPreferences", param: parms, commandType: CommandType.StoredProcedure)).ToList();

            return preferenceModel;
        }

        public async Task<List<VoucherImages>> GetUserVouchers(int userId)
        {
            var parms = new DynamicParameters();
            parms.Add("@UserId", userId);
            var result = (await _dbConnection.QueryAsync<VoucherImages>("GetUserVouchers", param: parms, commandType: CommandType.StoredProcedure)).ToList();

            return result;
        }

        public async Task<int> UpdatePreference(List<PrefenceRequest> request, int id)
        {
            var parms = new DynamicParameters();
            parms.Add("@Id", id);
            parms.AddTable("@Prefrences", "PrefrenceType", request);
            return await _dbConnection.ExecuteAsync("ManagePreferences", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateUserName(UpdateUserNameRequest request, int userId)
        {
            var parms = new DynamicParameters();
            parms.Add("@UserId", userId);
            parms.Add("@UserName", request.UserName);
            return await _dbConnection.QueryFirstOrDefaultAsync<int>("UpdateUserName", param: parms, commandType: CommandType.StoredProcedure);
        }

    }
}
