using Shared.Model.Base;
using Shared.Model.Request.Group;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IGroupService
    {
        Task<ApiResponse<List<GroupsDetails>>> GetGroups(GetGroups request);
        Task<ApiResponse<List<Post>>> GetGroupPosts(GroupPosts request);
        Task<ApiResponse<List<ParticipantsDetails>>> GetParticipants(GroupId request);
        Task<ApiResponse<int>> AddUpdateGroup(Group request);
        Task<ApiResponse<bool>> DeleteGroup(GroupId request);
        Task<ApiResponse<bool>> LeftGroup(Participants request,int userId);

        Task<ApiResponse<GroupDetailsWithParticipants>> GetGroupWithParticipants(GroupId request, int userId);
        Task<ApiResponse<bool>> DeleteGroupParticipant(DeleteGroupParticipantRequestModel request);
        Task<ApiResponse<List<ParticipantsDetails>>> GetNotAddedParticipants(GetNotAddedParticipantsRequestModel request, int userId);
        Task<ApiResponse<bool>> AddParticipants(AddParticipantsRequestModel request);
    }
}
