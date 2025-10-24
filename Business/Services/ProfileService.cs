using Business.IServices;
using Data.IRepository;
using FirebaseAdmin.Auth;
using Google.Apis.AndroidPublisher.v3.Data;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.Request.Account;
using Shared.Model.Response;
using Shared.Resources;
using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Business.Services
{
    public class ProfileService : IProfileService
    {

        private readonly IProfileRepository _profileRepository;
        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<ApiResponse<Profile>> GetUserDetails(int userId)
        {
            var response = await _profileRepository.GetById(userId);
            if (response is null)
            {
                return new ApiResponse<Profile>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetUserDetails");
            }
            response.ProfileImage = CommonFunctions.GetRelativeFilePath(response.ProfileImage, SiteKeys.UserImageFolderPath);
            if (response.CoverPicture != null) { response.CoverPicture = CommonFunctions.GetRelativeFilePath(response.CoverPicture, SiteKeys.UserImageFolderPath); }
            return new ApiResponse<Profile>(response, message: ResourceString.GetUserDetails, apiName: "GetUserDetails");
        }


        public async Task<ApiResponse<ProfileDetailsResponse>> GetUserProfileDetails(int userId)
        {
            var response = await _profileRepository.GetUserProfileById(userId);
            if (response.Profile is null)
            {
                return new ApiResponse<ProfileDetailsResponse>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetUserDetails");
            }
            response.Profile.ProfileImage = CommonFunctions.GetRelativeFilePath(response.Profile.ProfileImage, SiteKeys.UserImageFolderPath);
            if (response.Profile.CoverPicture != null) { response.Profile.CoverPicture = CommonFunctions.GetRelativeFilePath(response.Profile.CoverPicture, SiteKeys.UserImageFolderPath); }

            if (response.Friends.Any())
            {
                response.Friends.ForEach(friend =>
                {
                    friend.ProfileImage = CommonFunctions.GetRelativeFilePath(friend.ProfileImage, SiteKeys.UserImageFolderPath);
                });
                response.FriendCount = response.Friends.Count;
            }

            if (response.Stickers.Any())
            {
                response.Stickers.ForEach(sticker =>
                {
                    sticker.ImagePath = $"{SiteKeys.SiteUrl}{SiteKeys.ItemsImage}{sticker.ImagePath}";
                });
            }

            return new ApiResponse<ProfileDetailsResponse>(response, message: ResourceString.GetUserDetails, apiName: "GetUserDetails");
        }

        public async Task<ApiResponse<PreferenceModel>> GetPreferenceById(int userId)
        {
            var response = await _profileRepository.GetPreferenceById(userId);
            if (response is null)
            {
                return new ApiResponse<PreferenceModel>(null, message: ResourceString.Fail, apiName: "GetPreference");
            }
            return new ApiResponse<PreferenceModel>(response, message: ResourceString.Success, apiName: "GetPreference");
        }


        public async Task<ApiResponse<List<Item>>> SearchPreference(string preference, int type)
        {
            var response = await _profileRepository.SearchPreference(preference, type);
            if (response is null)
            {
                return new ApiResponse<List<Item>>(null, message: ResourceString.Fail, apiName: "SearchPreference");
            }
            return new ApiResponse<List<Item>>(response, message: ResourceString.Success, apiName: "SearchPreference");
        }

        public async Task<ApiResponse<List<VoucherImages>>> GetUserVouchers(int userId)
        {
            var response = await _profileRepository.GetUserVouchers(userId);
            if (response is null)
            {
                return new ApiResponse<List<VoucherImages>>(null, message: ResourceString.Fail, apiName: "GetUserVouchers");
            }
            foreach(var item in response)
            {
                item.Image = string.IsNullOrEmpty(item.Image) ? $"{SiteKeys.SiteUrl}{SiteKeys.DefaultVoucherImage}" : CommonFunctions.GetRelativeFilePath(item.Image, SiteKeys.VoucherImageFolderPath);
            }
            return new ApiResponse<List<VoucherImages>>(response, message: ResourceString.Success, apiName: "GetUserVouchers");
        }

        public async Task<ApiResponse<bool>> UpdateProfile(Profile profileRequest, int userId)
        {
            profileRequest.Id = userId;
            var response = await _profileRepository.AddUpdate(profileRequest);
            if (response <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UpdateNotProfile, apiName: "UpdateProfile");
            }
            return new ApiResponse<bool>(true, message: ResourceString.UpdateProfile, apiName: "UpdateProfile");
        }

        public async Task<ApiResponse<bool>> AccountDelete(int userId)
        {
            var response = await _profileRepository.AccountDelete(userId);
            if (response <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "DeleteAccount");
            }
            return new ApiResponse<bool>(true, message: ResourceString.DeleteAccount, apiName: "DeleteAccount");
        }

        public async Task<ApiResponse<bool>> UpdatePreference(List<PrefenceRequest> request, int id)
        {
            var response = await _profileRepository.UpdatePreference(request, id);
            if (response <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Fail, apiName: "UpdatePrefence");
            }
            return new ApiResponse<bool>(true, message: ResourceString.UpdatePrefrence, apiName: "UpdatePrefence");
        }

        public async Task<ApiResponse<bool>> UpdateUserName(UpdateUserNameRequest request, int userId)
        {
            var response = await _profileRepository.UpdateUserName(request, userId);
            if (response == 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Fail, apiName: "UpdateUserName");
            }

            if (response == -1)
            {
                return new ApiResponse<bool>(false, message: ResourceString.InsufficientStars, apiName: "UpdateUserName");
            }
            else if (response == -2)
            {
                return new ApiResponse<bool>(false, message: ResourceString.SameAsOldUsername, apiName: "UpdateUserName");
            }
            else if (response == -3)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UsernameAlreadyTaken, apiName: "UpdateUserName");
            }

            return new ApiResponse<bool>(true, message: ResourceString.UsernameUpdated, apiName: "UpdateUserName");
        }


        public ApiResponse<string> ImageUpload(IFormFile? file)
        {
            string filePath = string.Empty;
            if (file == null || file.Length == 0)
            {
                return new ApiResponse<string>(filePath, message: "", apiName: "ImageUpload");
            }
            string newImageName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var folderPath = string.Format("{0}{1}", SiteKeys.SitePhysicalPath, SiteKeys.UserImagePhysical);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            filePath = Path.Combine(folderPath, newImageName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var stream1 = file.OpenReadStream();
            var photo = CommonFunctions.GetReducedCircleImage(20, 20, stream1);
            if (photo != null)
            {
                var thumbPath = string.Format("{0}{1}", SiteKeys.SitePhysicalPath, SiteKeys.UserImagePhysical);
                filePath = Path.Combine(thumbPath, "_thumb" + newImageName);

                // Save the SKBitmap to the file
                using (var image = SKImage.FromBitmap(photo))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100)) // or SKEncodedImageFormat.Png, etc.
                using (var stream = File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
            return new ApiResponse<string>(CommonFunctions.GetRelativeFilePath(newImageName, SiteKeys.UserImageFolderPath), message: "", apiName: "ImageUpload");
        }

        public async Task<ApiResponse<UserStreak>> GetStreakCount(int userId)
        {
            var response = await _profileRepository.GetById(userId);
            if (response is null)
            {
                return new ApiResponse<UserStreak>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetStreakCount");
            }

            UserStreak userStreak = new UserStreak
            {
                TotalStars = response.TotalStars,
                Streak = response.Streak
            };
            return new ApiResponse<UserStreak>(userStreak, message: ResourceString.GetUserDetails, apiName: "GetStreakCount");
        }


    }
}