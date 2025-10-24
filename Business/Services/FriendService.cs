using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Google.Apis.AndroidPublisher.v3.Data;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.Notification;
using Shared.Model.Request.Social;
using Shared.Model.Response;
using Shared.Resources;
using Shared.Utility;

namespace Business.Services
{
    public class FriendService : IFriendService
    {
        private readonly ICommonService _session;
        private readonly IFriendRepository _friendRepository;
        private readonly IAccountRepositry _accountRepositry;
        public FriendService(IFriendRepository friendRepository, ICommonService session, IAccountRepositry accountRepositry)
        {
            _friendRepository = friendRepository;
            _session = session;
            _accountRepositry = accountRepositry;
        }
        public async Task<ApiResponse<List<Friends>>> FindFriends(Find request)
        {
            var response = await _friendRepository.FindFriends(request);
            if (response is null)
            {
                return new ApiResponse<List<Friends>>(null, message: ResourceString.Error, apiName: "FindFriends");
            }
            foreach (var item in response)
            {
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<Friends>>(response, message: ResourceString.Success, apiName: "FindFriends");
        }
        public async Task<ApiResponse<bool>> SendRequest(SendRequest request)
        {
            var response = await _friendRepository.SendRequest(request);
            if (response > 0)
            {
                var sender = await _accountRepositry.FindByIdAsync(request.Sender);
                var receiver = await _accountRepositry.FindByIdAsync(request.Receiver);
                PushNotificationRequestModel pushNotificationRequestModel = new PushNotificationRequestModel
                {
                    DeviceToken = receiver.DeviceToken ?? "",
                    Title = "Friend Request",
                    Message = $"You've got a new friend request! {sender.Name} wants to connect with you. Accept or Decline now to manage your connections.",
                    NotificationType = "3"
                };
                await PushNotifications.SendPushNotification(pushNotificationRequestModel);

                return new ApiResponse<bool>(true, message: ResourceString.RequestSentSuccessfully, apiName: "SendRequest");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "SendRequest");
        }
        public async Task<ApiResponse<bool>> RequestAction(RequestAction request)
        {
            var response = await _friendRepository.RequestAction(request);
            if (response > 0)
            {
                if (request.IsAccept)
                {
                    return new ApiResponse<bool>(true, message: ResourceString.Requestacceptedsuccessfully, apiName: "RequestAction");
                }
                return new ApiResponse<bool>(true, message: ResourceString.Requestrejectedsuccessfully, apiName: "RequestAction");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "RequestAction");
        }
        public async Task<ApiResponse<List<Friends>>> GetRequest(GetRequest request)
        {
            var response = await _friendRepository.GetRequest(request);
            if (response is null)
            {
                return new ApiResponse<List<Friends>>(null, message: ResourceString.Error, apiName: "GetRequest");
            }
            foreach (var item in response)
            {
                item.SendedOn = _session.GetTimeAfterAddOffSet(DateTime.Parse(item.SendedOn)).ToString();
                item.ProfileImage = CommonFunctions.GetRelativeFilePath(item.ProfileImage, SiteKeys.UserImageFolderPath);
            }
            return new ApiResponse<List<Friends>>(response, message: ResourceString.Success, apiName: "GetRequest");
        }
        public async Task<ApiResponse<int>> GetRequestCount(int id)
        {
            var response = await _friendRepository.GetRequestCount(id);
            return new ApiResponse<int>(response, message: ResourceString.Success, apiName: "GetRequestCount");
        }
        public async Task<ApiResponse<bool>> RemoveFriend(SendRequest request)
        {
            var response = await _friendRepository.RemoveFriend(request);
            if (response > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.FriendRemovedsuccessfully, apiName: "RemoveFriend");
            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "RemoveFriend");
        }
    }
}
