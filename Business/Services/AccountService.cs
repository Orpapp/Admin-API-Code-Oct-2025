using Business.Communication;
using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Shared.Common;
using Shared.Common.Enums;

using Shared.Model.Base;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.WebUser;
using Shared.Resources;
using Shared.Utility;

namespace Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepositry _accountRepository;
        private readonly INotificationService _notificationService;
        public AccountService(IAccountRepositry accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<ApiLoginDto>> Login(ApiLoginRequest request)
        {
            var objUserContext = await _accountRepository.FindByEmailAsync(request.Email);

            if (objUserContext == null || objUserContext.UserType == 1)
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (!objUserContext.IsEmailVerified)
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.EmailNotVerified, apiName: "Login");
            }
            else if (!(objUserContext.IsActive ?? false))
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.UserIsNotActive, apiName: "Login");
            }
            else if (!Encryption.VerifyHash(request.Password, objUserContext.PasswordHash))
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.InvalidPassword, apiName: "Login");
            }
            else if (objUserContext.UserType != (int)UserTypes.User)
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            var accessToken = Guid.NewGuid().ToString();
            await _accountRepository.UpdateDeviceToken(new UpdateDeviceTokenRequest()
            {
                Email = request.Email,
                Password = request.Password,
                DeviceToken = request.DeviceToken,
                DeviceType = request.DeviceType,
                AccessToken = accessToken,
            });
            ApiLoginDto apiLoginDto = new ApiLoginDto
            {
                UserId = Convert.ToInt64(objUserContext.UserId),
                Name = objUserContext.FirstName+" "+ objUserContext.LastName,
                Email = objUserContext.Email,
                ProfileImage = objUserContext.ProfileImage,
                AccessToken = accessToken,
                UserName= objUserContext.UserName,
                UserType = Convert.ToInt16(objUserContext.UserType),
            };
            apiLoginDto.ProfileImage = CommonFunctions.GetRelativeFilePath(apiLoginDto.ProfileImage, SiteKeys.UserImageFolderPath);
            return new ApiResponse<ApiLoginDto>(apiLoginDto, message: ResourceString.Userloggedinsuccess, apiName: "Login");
        }

        public async Task<ApiResponse<ApiLoginDto>> SocialLogin(SocialLoginRequest request)
        {
            var accessToken = Guid.NewGuid().ToString();

            var objUserContext = await _accountRepository.SocialLogin(request, accessToken);

            if (objUserContext is null)
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.UserNotExist, apiName: "SocialLogin");
            }

            else if (!objUserContext.IsActive ?? false)
            {
                return new ApiResponse<ApiLoginDto>(null, message: ResourceString.UserIsNotActive, apiName: "SocialLogin");
            }

            ApiLoginDto apiLoginDto = new ApiLoginDto
            {
                UserId = Convert.ToInt64(objUserContext.UserId),
                Name = objUserContext.FirstName + " " + objUserContext.LastName,
                Email = objUserContext.Email,
                ProfileImage = objUserContext.ProfileImage,
                AccessToken = accessToken,
                UserName = objUserContext.UserName,
                UserType = Convert.ToInt16(objUserContext.UserType),
            };
            apiLoginDto.ProfileImage = CommonFunctions.GetRelativeFilePath(apiLoginDto.ProfileImage, SiteKeys.UserImageFolderPath);

            return new ApiResponse<ApiLoginDto>(apiLoginDto, message: ResourceString.Userloggedinsuccess, apiName: "SocialLogin");
        }

        public Task<UserDetailsDto> FindByEmailAsync(string email) => _accountRepository.FindByEmailAsync(email);

        public async Task<ApiResponse<bool>> SignUp(RegistrationRequest request)
        {
            var getUserDetail = await _accountRepository.FindByEmailAsync(request.Email);
            if (getUserDetail is not null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.EmailExists, apiName: "Signup");
            }
            var newUser = await _accountRepository.Add(new UserDetailsDto()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
            });
            if (newUser <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "Signup");
            }
            var token = Guid.NewGuid().ToString();
            await _accountRepository.Update(new UserDetailsDto()
            {
                UserId = newUser,
                Token = token,
            });
            _notificationService.EmailVerification(request.Email, request.FirstName, token, ResourceString.RegistrationSubject);
            return new ApiResponse<bool>(true, message: ResourceString.SignUp, apiName: "Signup");
        }

        public async Task<ApiResponse<bool>> ResendEmailVerificationMail(string request)
        {
            var getUserDetail = await _accountRepository.FindByEmailAsync(request);

            if (getUserDetail is null || getUserDetail.FirstName is null || getUserDetail.EmailVerificationToken is null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserNotExist, apiName: "ResendEmailVerificationMail");
            }

            _notificationService.EmailVerification(request, getUserDetail.FirstName, getUserDetail.EmailVerificationToken, ResourceString.RegistrationSubject);

            return new ApiResponse<bool>(true, message: ResourceString.SignUp, apiName: "ResendEmailVerificationMail");
        }
        public async Task<ApiResponse<bool>> VerifyEmail(TokenRequest request)
        {
            var verifyEmail = await _accountRepository.Update(new UserDetailsDto()
            {
                EmailVerificationToken = request.Token,
            });

            if (verifyEmail <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.InvalidConfirmationToken, apiName: "VerifyEmail");
            }

            return new ApiResponse<bool>(true, message: ResourceString.EmailVerified, apiName: "VerifyEmail");
        }

        public async Task<ApiResponse<bool>> ForgetPassword(ForgetPasswordRequest request)
        {
            var getUserByEmail = await _accountRepository.FindByEmailAsync(request.Email);
            if (getUserByEmail == null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserNotExist, apiName: "ForgetPassword");
            }

            ForgetPasswordTokenRequest tokenRequest = new();
            tokenRequest.Email = request.Email;
            tokenRequest.Token = Guid.NewGuid().ToString();

            var updateUser = await _accountRepository.Update(new UserDetailsDto()
            {
                Email = tokenRequest.Email,
                ResetToken = tokenRequest.Token,
            });

            if (updateUser <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "ForgetPassword");
            }

            _notificationService.SendResetPasswordEmail(ResourceString.ForgetPasswordSubject, tokenRequest.Token, tokenRequest.Email, getUserByEmail.FirstName);
            return new ApiResponse<bool>(true, message: ResourceString.ForgetPassword, apiName: "ForgetPassword");
        }

        public async Task<UserDetailsDto> FindByIdAsync(long userId) => await _accountRepository.FindByIdAsync(userId);

        public async Task<LoginResponseModel> PasswordSignInAsync(string email, string password)
        {
            LoginResponseModel loginResp = new();
            var getUserDetail = await _accountRepository.FindByEmailAsync(email);

            if (getUserDetail is null)
            {
                loginResp.Succeeded = false;
                loginResp.Message = ResourceString.UserNotExist;
                return loginResp;
            }


            UserLoginDetailDto userLoginDetailDto = new UserLoginDetailDto
            {
                UserId = Convert.ToInt64(getUserDetail.UserId),
                Email = getUserDetail.Email,
                PasswordHash = getUserDetail.PasswordHash,
                UserType = Convert.ToInt16(getUserDetail.UserType),
                Name = getUserDetail.FirstName,
                IsActive = getUserDetail.IsActive ?? false,
                IsDeleted = getUserDetail.IsDeleted ?? false,
                IsEmailVerified = getUserDetail.IsEmailVerified,
            };

            if (Encryption.VerifyHash(password, userLoginDetailDto.PasswordHash))
            {
                loginResp.Succeeded = true;
                loginResp.ObjUser = userLoginDetailDto;
            }
            else
            {
                loginResp.Succeeded = false;
                loginResp.Message = ResourceString.InvalidPassword;
                loginResp.ObjUser = userLoginDetailDto;
            }

            return loginResp;
        }

        public async Task<TResponse<ForgotPasswordDto>> ResetPasswordTokenAsync(long userId)
        {
            TResponse<ForgotPasswordDto> response = new();
            var userDetail = await _accountRepository.ResetPasswordTokenAsync(userId, Guid.NewGuid().ToString());

            if (userDetail is null)
            {
                response.ResponseMessage = ResourceString.UserNotExist;
                return response;
            }

            switch ((ForgotPasswordResponseTypes)userDetail.IsValid)
            {
                case ForgotPasswordResponseTypes.TokenUpdatedSuccess:
                    response.ResponseMessage = ResourceString.ForgetPassword;
                    response.ResponsePacket = userDetail;
                    _notificationService.SendResetPasswordEmailToWebUser("Forgot Password", userDetail.ForgotPasswordToken, userDetail.Email, userDetail.UserName);
                    break;

                case ForgotPasswordResponseTypes.ForgotEmailAlreadySent:
                    response.ResponseMessage = ResourceString.ForgotEmailAlreadySent;
                    break;

                default:
                    if (userDetail.IsDeleted)
                    {
                        response.ResponseMessage = ResourceString.UserAccountDeleted;
                    }
                    else if (!userDetail.IsActive)
                    {
                        response.ResponseMessage = ResourceString.UserIsNotActive;
                    }
                    else
                    {
                        response.ResponseMessage = ResourceString.EmailNotVerified;
                    }
                    break;
            }

            return response;
        }

        public async Task<bool> CheckResetPasswordTokenExist(string token) => await _accountRepository.CheckResetPasswordTokenExist(token);

        public async Task<ResponseTypes> ResetPassword(ResetPasswordModel model)
        {
            var getUserDetail = await _accountRepository.GetUserDetailByToken(model.Token);

            if (getUserDetail is null)
            {
                return ResponseTypes.Error;
            }

            if (Encryption.VerifyHash(model.Password, getUserDetail.PasswordHash))
            {
                return ResponseTypes.OldNewPasswordMatched;
            }

            var resetPasswordObj = await _accountRepository.ResetPassword(model);
            return resetPasswordObj > 0 ? ResponseTypes.Success : ResponseTypes.Error;
        }

        public async Task<ApiResponse<bool>> Logout(int userId)
        {
            var result = await _accountRepository.Logout(userId);
            if (result > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.LogoutSuccess, apiName: "Logout");
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.LogoutFailed, apiName: "Logout");
            }
        }

        public async Task<ResponseTypes> Registration(RegistrationRequest model)
        {
            var getUserDetail = await _accountRepository.FindByEmailAsync(model.Email);
            if (getUserDetail is not null)
            {
                return ResponseTypes.EmailAlreadyExists;
            }

            var token = Guid.NewGuid().ToString();
            var response = await _accountRepository.Add(new UserDetailsDto()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                EmailVerificationToken = token
            });

            if (response <= 0)
            {
                return ResponseTypes.Error;
            }
            _notificationService.EmailVerification(model.Email, model.FirstName, token, ResourceString.RegistrationSubject);
            return ResponseTypes.Success;
        }

        public ApiResponse<bool> SaveContactUsDetails(ContactUsRequestModel requestModel)
        {
            // save details
            _notificationService.SendContactUsMailToAdmin(ResourceString.ContactUsSubject, requestModel.Name, requestModel.Email, requestModel.Query);
            return new ApiResponse<bool>(true, message: ResourceString.ContactUsMailSuccess, apiName: "ContactUs");
        }

        public async Task<ApiResponse<bool>> UpdateEmailVerificationToken(long userId, string email, string name)
        {
            var emailVerificationToken = Guid.NewGuid().ToString();
            var verifyEmail = await _accountRepository.Update(new UserDetailsDto()
            {
                UserId = userId,
                Token = emailVerificationToken,
            });
            if (verifyEmail <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.VerificationLinkSentFailed, apiName: "");
            }

            _notificationService.EmailVerification(email, name, emailVerificationToken, ResourceString.VerifyEmailSubject);
            return new ApiResponse<bool>(true, message: ResourceString.VerificationLinkSent, apiName: "");
        }


        public CheckUserAccessTokenDto CheckUserAccessToken(string accessToken) => _accountRepository.CheckUserAccessToken(accessToken);

        public bool CheckAppVersion(string appVersion) => _accountRepository.CheckAppVersion(appVersion);

       
    }
}
