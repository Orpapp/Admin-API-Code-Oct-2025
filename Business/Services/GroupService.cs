using Business.IServices;
using Data.IRepository;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.Request.Group;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Resources;
using System.Net.Mail;

namespace Business.Services
{

    public class GroupService : IGroupService
    {
        private readonly ICommonService _session;
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository, ICommonService session)
        {
            _groupRepository = groupRepository;
            _session = session;
        }

        public async Task<ApiResponse<int>> AddUpdateGroup(Group request)
        {
            var response = await _groupRepository.AddUpdateGroup(request);
            if (response < 0)
            {
                return new ApiResponse<int>(0, message: ResourceString.Error, apiName: "AddUpdateGroup");
            }
            if (request.Id == 0)
            {
                return new ApiResponse<int>(response, message: ResourceString.GroupAddedSuccess, apiName: "AddUpdateGroup");
            }
            return new ApiResponse<int>(response, message: ResourceString.GroupUpdatedSuccess, apiName: "AddUpdateGroup");
        }
        public async Task<ApiResponse<bool>> DeleteGroup(GroupId request)
        {
            var response = await _groupRepository.DeleteGroup(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeleteGroup");
            }
            return new ApiResponse<bool>(true, message: ResourceString.GroupDeleteSuccess, apiName: "DeleteGroup");
        }
        public async Task<ApiResponse<bool>> LeftGroup(Participants request,int userId)
        {
            var response = await _groupRepository.LeftGroup(request, userId);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "LeftGroup");
            }
            return new ApiResponse<bool>(true, message: ResourceString.GroupLeftSuccess, apiName: "LeftGroup");
        }

        public async Task<ApiResponse<List<GroupsDetails>>> GetGroups(GetGroups request)
        {
            var response = await _groupRepository.GetGroups(request);
            if (response is null)
            {
                return new ApiResponse<List<GroupsDetails>>(null, message: ResourceString.Error, apiName: "GetGroups");
            }
            foreach (var item in response)
            {
                item.Attachment = CommonFunctions.GetRelativeFilePath(item.Attachment, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<GroupsDetails>>(response, message: ResourceString.Success, apiName: "GetGroups");
        }
        public async Task<ApiResponse<List<Post>>> GetGroupPosts(GroupPosts request)
        {
            var response = await _groupRepository.GetGroupPosts(request);
            if (response.post is null)
            {
                return new ApiResponse<List<Post>>(null, message: ResourceString.Error, apiName: "GetGroupPosts");
            }
            foreach (var item in response.post)
            {
                item.PostImages = response.images.Where(x => x.PostId == item.Id).ToList();

                foreach (var image in item.PostImages)
                {
                    image.Attachment = CommonFunctions.GetRelativeFilePath(image.Attachment, SiteKeys.UserImageFolderPath);
                }
                item.CreatedOn = _session.GetTimeAfterAddOffSet(DateTime.Parse(item.CreatedOn)).ToString();
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<Post>>(response.post, message: ResourceString.Success, apiName: "GetGroupPosts");
        }

        public async Task<ApiResponse<List<ParticipantsDetails>>> GetParticipants(GroupId request)
        {
            var response = await _groupRepository.GetParticipants(request);
            if (response is null)
            {
                return new ApiResponse<List<ParticipantsDetails>>(null, message: ResourceString.Error, apiName: "GetParticipants");
            }
            foreach (var item in response)
            {
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<ParticipantsDetails>>(response, message: ResourceString.Success, apiName: "GetParticipants");
        }

        public async Task<ApiResponse<GroupDetailsWithParticipants>> GetGroupWithParticipants(GroupId request,int userId)
        {
            GroupDetailsWithParticipants getGroupDetail;
            getGroupDetail = await _groupRepository.GetGroupWithParticipants(request, userId);
            if (getGroupDetail is null)
            {
                return new ApiResponse<GroupDetailsWithParticipants>(getGroupDetail, message: ResourceString.Fail, apiName: "GetGroupWithParticipants");
            }
            getGroupDetail.Attachment = CommonFunctions.GetRelativeFilePath(getGroupDetail.Attachment, SiteKeys.UserImageFolderPath);
            if (getGroupDetail.ParticipantsDetails.Any())
            {
                foreach (var u in getGroupDetail.ParticipantsDetails)
                {
                    u.ProfileImage = CommonFunctions.GetRelativeFilePath(u.ProfileImage, SiteKeys.UserImageFolderPath);
                }
            }
            return new ApiResponse<GroupDetailsWithParticipants>(getGroupDetail, message: ResourceString.Success, apiName: "GetGroupWithParticipants");
        }

        public async Task<ApiResponse<bool>> DeleteGroupParticipant(DeleteGroupParticipantRequestModel request)
        {
            var response = await _groupRepository.DeleteGroupParticipant(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeleteGroupParticipant");
            }
            return new ApiResponse<bool>(true, message: ResourceString.GroupLeftSuccess, apiName: "DeleteGroupParticipant");
        }


        public async Task<ApiResponse<List<ParticipantsDetails>>> GetNotAddedParticipants(GetNotAddedParticipantsRequestModel request,int userId)
        {
            var response = await _groupRepository.GetNotAddedParticipants(request, userId);
            if (response is null)
            {
                return new ApiResponse<List<ParticipantsDetails>>(null, message: ResourceString.Error, apiName: "GetNotAddedParticipants");
            }
            foreach (var item in response)
            {
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<ParticipantsDetails>>(response, message: ResourceString.Success, apiName: "GetNotAddedParticipants");
        }

        public async Task<ApiResponse<bool>> AddParticipants(AddParticipantsRequestModel request)
        {
            var response = await _groupRepository.AddParticipants(request);
            if (response < 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "AddParticipants");
            }
            return new ApiResponse<bool>(true, message: ResourceString.ParticipantsAddeddSuccess, apiName: "AddParticipants");
        }
    }
}
