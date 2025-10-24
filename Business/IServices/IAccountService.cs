using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.WebUser;

namespace Business.IServices
{
    public interface IAccountService
    {
        Task<ApiResponse<ApiLoginDto>> Login(ApiLoginRequest request);

        Task<ApiResponse<ApiLoginDto>> SocialLogin(SocialLoginRequest request);
        Task<UserDetailsDto> FindByEmailAsync(string email);
        Task<ApiResponse<bool>> SignUp(RegistrationRequest request);
        Task<ApiResponse<bool>> VerifyEmail(TokenRequest request);
        Task<ApiResponse<bool>> ForgetPassword(ForgetPasswordRequest request);

        ApiResponse<bool> SaveContactUsDetails(ContactUsRequestModel requestModel);
        Task<UserDetailsDto> FindByIdAsync(long userId);
        Task<ApiResponse<bool>> ResendEmailVerificationMail(string request);


        #region Web User
        Task<LoginResponseModel> PasswordSignInAsync(string email, string password);
        Task<TResponse<ForgotPasswordDto>> ResetPasswordTokenAsync(long userId);
        Task<bool> CheckResetPasswordTokenExist(string token);
        Task<ResponseTypes> ResetPassword(ResetPasswordModel model);

        Task<ResponseTypes> Registration(RegistrationRequest model);
        Task<ApiResponse<bool>> UpdateEmailVerificationToken(long userId, string email, string name);
        #endregion
        CheckUserAccessTokenDto CheckUserAccessToken(string accessToken);
        bool CheckAppVersion(string appVersion);
        Task<ApiResponse<bool>> Logout(int userId);
    }
}
