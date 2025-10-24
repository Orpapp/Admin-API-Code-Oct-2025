using Shared.Model.Base;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IFriendService
    {
        Task<ApiResponse<List<Friends>>> FindFriends(Find request);
        Task<ApiResponse<int>> GetRequestCount(int id);
        Task<ApiResponse<bool>> RemoveFriend(SendRequest request);
        Task<ApiResponse<List<Friends>>> GetRequest(GetRequest request);
        Task<ApiResponse<bool>> SendRequest(SendRequest request);
        Task<ApiResponse<bool>> RequestAction(RequestAction request);
    }
}
