using Microsoft.AspNetCore.Http;
using Shared.Model.Base;
using Shared.Model.Request.Account;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IProfileService
    {
        Task<ApiResponse<Profile>> GetUserDetails(int userId);
        Task<ApiResponse<ProfileDetailsResponse>> GetUserProfileDetails(int userId);
        Task<ApiResponse<List<Item>>> SearchPreference(string prefrence,int type);
        Task<ApiResponse<PreferenceModel>> GetPreferenceById(int userId);
        Task<ApiResponse<bool>> UpdateProfile(Profile profileRequest, int userId);
        Task<ApiResponse<bool>> AccountDelete(int userId);
        Task<ApiResponse<bool>> UpdatePreference(List<PrefenceRequest> request, int id);

        Task<ApiResponse<bool>> UpdateUserName(UpdateUserNameRequest request, int userId);
        ApiResponse<string> ImageUpload(IFormFile? file);
        Task<ApiResponse<UserStreak>> GetStreakCount(int userId);

        Task<ApiResponse<List<VoucherImages>>> GetUserVouchers(int userId);
    }
}
