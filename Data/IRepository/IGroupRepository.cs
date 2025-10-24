using Shared.Model.Request.Group;
using Shared.Model.Request.Social;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<List<GroupsDetails>> GetGroups(GetGroups request);
        Task<(List<Post> post, List<PostImages> images)> GetGroupPosts(GroupPosts request);
        Task<List<ParticipantsDetails>> GetParticipants(GroupId request);
        Task<int> AddUpdateGroup(Group request);
        Task<int> DeleteGroup(GroupId request);
        Task<int> LeftGroup(Participants request, int userId);

        Task<GroupDetailsWithParticipants> GetGroupWithParticipants(GroupId request, int userId);
        Task<int> DeleteGroupParticipant(DeleteGroupParticipantRequestModel request);
        Task<List<ParticipantsDetails>> GetNotAddedParticipants(GetNotAddedParticipantsRequestModel request, int userId);

        Task<int> AddParticipants(AddParticipantsRequestModel request);
    }
}

