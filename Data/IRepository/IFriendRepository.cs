using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IFriendRepository : IGenericRepository<Find>
    {
        Task<List<Friends>> FindFriends(Find request);
        Task<List<Friends>> GetRequest(GetRequest request);
        Task<int> GetRequestCount(int id);
        Task<int> RemoveFriend(SendRequest request);
        Task<int> SendRequest(SendRequest request);
        Task<int> RequestAction(RequestAction request);

    }
}

