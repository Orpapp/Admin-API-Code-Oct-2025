using Shared.Model.Base;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IPostService
    {

        Task<ApiResponse<bool>> AddPost(Posts request);
        Task<ApiResponse<bool>> LikePost(Like request);
        Task<ApiResponse<int>> PostComment(Comment request);
        Task<ApiResponse<Comments>> GetComments(GetComments request);
        Task<ApiResponse<bool>> DeletePost(PostId request);
        Task<ApiResponse<bool>> DeleteComment(PostId request, int userId);


        Task<ApiResponse<bool>> CheckUserExists(int userid, int groupId, int postId);
    }
}
