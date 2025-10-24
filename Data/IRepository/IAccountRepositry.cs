using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IAccountRepositry : IBaseRepository<UserDetailsDto>
    {
        Task<int> UpdateDeviceToken(UpdateDeviceTokenRequest request);
        Task<UserDetailsDto> FindByEmailAsync(string email);
        Task<int> CheckIfUserExistForSocialLogin(string? socialId, string email);
        Task<UserDetailsDto> SocialLogin(SocialLoginRequest socialLoginRequest, string accessToken);
        Task<List<UserListDto>> UserList(UsersRequestModel request);
        Task<int> ChangePassword(ChangePasswordModel model, long userId);
        Task<int> Logout(int userId);

        #region Web User

        Task<ForgotPasswordDto> ResetPasswordTokenAsync(long userId, string forgotPasswordToken);
        Task<bool> CheckResetPasswordTokenExist(string token);
        Task<UserDetailsDto> GetUserDetailByToken(string token);
        Task<int> ResetPassword(ResetPasswordModel model);
        #endregion
        CheckUserAccessTokenDto CheckUserAccessToken(string accessToken);
        bool CheckAppVersion(string appVersion);
        Task<int> UpdateLevel(List<LevelInfo> levelInfos);
    }
}
