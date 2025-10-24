using Dapper;
using Data.IFactory;
using Data.IRepository;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using System.Data;

namespace Data.Repository
{
    public class FriendRepository : GenericRepository<Find>, IFriendRepository
    {
        private readonly IDbConnection _dbConnection;
        public FriendRepository(IDbConnectionFactory dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection.CreateDBConnection();
        }

        public async Task<List<Friends>> FindFriends(Find request)
        {
            return (await _dbConnection.QueryAsync<Friends>("FindFriends", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<int> SendRequest(SendRequest request)
        {
            return await _dbConnection.ExecuteAsync("SendRequest", param: request, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> RemoveFriend(SendRequest request)
        {
            return await _dbConnection.ExecuteAsync("RemoveFriend", param: request, commandType: CommandType.StoredProcedure);
        }
        public async Task<int> RequestAction(RequestAction request)
        {
            return await _dbConnection.ExecuteAsync("RequestAction", param: request, commandType: CommandType.StoredProcedure);
        }



        public async Task<List<Friends>> GetRequest(GetRequest request)
        {
            return (await _dbConnection.QueryAsync<Friends>("GetRequest", param: request, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<int> GetRequestCount(int id)
        {
            var parms = new
            {
                @Id = id
            };
            return (await _dbConnection.ExecuteScalarAsync<int>("GetRequestCount", param: parms, commandType: CommandType.StoredProcedure));
        }
    }
}
