using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Resources;

namespace Business.Services
{
    public class PostService : IPostService
    {
        private readonly ICommonService _session;
        private readonly IPostRepository _postRepository;
        public PostService(IPostRepository postRepository, ICommonService session)
        {
            _postRepository = postRepository;
            _session = session;
        }

        public async Task<ApiResponse<bool>> AddPost(Posts request)
        {
            var exists = await _postRepository.CheckUserExists(request.CreatedBy,request.GroupId,0);
            if(exists == 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserCanNotPerformAnyAction, apiName: "AddPost");
            }
            var response = await _postRepository.AddPost(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "AddPost");
            }
            return new ApiResponse<bool>(true, message: ResourceString.PostAddedSuccess, apiName: "AddPost");
        }
        public async Task<ApiResponse<bool>> LikePost(Like request)
        {
            var exists = await _postRepository.CheckUserExists(request.LikedBy, 0, request.PostId);
            if (exists == 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserCanNotPerformAnyAction, apiName: "LikePost");
            }
            var response = await _postRepository.LikePost(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "LikePost");
            }
            return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "LikePost");
        }

        public async Task<ApiResponse<int>> PostComment(Comment request)
        {
            var exists = await _postRepository.CheckUserExists(request.CommentedBy, 0, request.PostId);
            if (exists == 0)
            {
                return new ApiResponse<int>(0, message: ResourceString.UserCanNotPerformAnyAction, apiName: "PostComment");
            }
            var response = await _postRepository.PostComment(request);
            if (response < 0)
            {
                return new ApiResponse<int>(response, message: ResourceString.Error, apiName: "PostComment");
            }
            return new ApiResponse<int>(response, message: ResourceString.Success, apiName: "PostComment");
        }

        public async Task<ApiResponse<Comments>> GetComments(GetComments request)
        {
            var response = await _postRepository.GetComments(request);
            if (response == null)
            {
                return new ApiResponse<Comments>(response, message: ResourceString.Error, apiName: "GetComments");
            }
            foreach (var item in response.AllComments)
            {
                item.CreatedOn = _session.GetTimeAfterAddOffSet(DateTime.Parse(item.CreatedOn)).ToString();
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<Comments>(response, message: ResourceString.Success, apiName: "GetComments");
        }
       
        public async Task<ApiResponse<bool>> DeletePost(PostId request)
        {
            
            var response = await _postRepository.DeletePost(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeletePost");
            }
            return new ApiResponse<bool>(true, message: ResourceString.PostDeleteSuccess, apiName: "DeletePost");
        }

        public async Task<ApiResponse<bool>> DeleteComment(PostId request,int userId)
        {
            var exists = await _postRepository.CheckUserExists(userId, 0, request.Id);
            if (exists == 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserCanNotPerformAnyAction, apiName: "DeleteComment");
            }
            var response = await _postRepository.DeleteComment(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeleteComment");
            }
            return new ApiResponse<bool>(true, message: ResourceString.CommentDeleteSuccess, apiName: "DeleteComment");
        }

        public async Task<ApiResponse<bool>> CheckUserExists(int userid,int groupId,int postId)
        {
            var res = await _postRepository.CheckUserExists(userid, groupId, postId);
            if (res > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.Success, apiName: "CheckUserExists");
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserNotExist, apiName: "CheckUserExists");
            }
        }
    }
}
