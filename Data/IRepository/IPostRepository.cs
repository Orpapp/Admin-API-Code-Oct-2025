using Shared.Model.Request.Social;
using Shared.Model.Response;
namespace Data.IRepository
{
    public interface IPostRepository : IGenericRepository<Posts>
    {
        Task<int> AddPost(Posts request);
        Task<int> LikePost(Like request);
        Task<int> PostComment(Comment request);
        Task<Comments> GetComments(GetComments request);
        Task<int> DeletePost(PostId request);
        Task<int> DeleteComment(PostId request);
        Task<int> CheckUserExists(int userId, int groupId, int postId);
    }
}

