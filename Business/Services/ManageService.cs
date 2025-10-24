using Business.IServices;
using Data.IRepository;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Response;
using Shared.Resources;
using Shared.Utility;

namespace Business.Services
{
    public class ManageService : IManageService
    {
        private readonly IAccountRepositry _accountRepository;
        public ManageService(IAccountRepositry accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<TResponse<List<UserListDto>>> UserList(UsersRequestModel request)
        {
            TResponse<List<UserListDto>> response = new();
            var getUserList = await _accountRepository.UserList(request);

            if (getUserList is null)
            {
                response.ResponsePacket = new List<UserListDto>();
                response.ResponseMessage = ResourceString.Error;
                return response;
            }

            getUserList.ForEach(u =>
            {
                u.PhoneNumber = u.PhoneNumber ?? "N/A";
                u.RegisterDate = u.CreatedOn.ToShortDateString();
            });

            response.ResponsePacket = getUserList;
            response.ResponseMessage = ResourceString.Success;

            return response;
        }

        public async Task<TResponse<UserDetailsDto>> GetUserDetails(long userId)
        {
            TResponse<UserDetailsDto> response = new();
            UserDetailsDto userDetailsObj = new();
            var getUserDetail = await _accountRepository.FindByIdAsync(userId);

            if (getUserDetail is null)
            {
                response.ResponsePacket = userDetailsObj;
                response.ResponseMessage = ResourceString.Fail;
                return response;
            }
            userDetailsObj.UserId = getUserDetail.UserId;
            userDetailsObj.UserType = getUserDetail.UserType;
            userDetailsObj.FirstName = getUserDetail.FirstName ?? "N/A";
            userDetailsObj.PhoneNumber = getUserDetail.PhoneNumber;
            userDetailsObj.Email = getUserDetail.Email ?? "N/A";
            userDetailsObj.IsActive = getUserDetail.IsActive;
            userDetailsObj.IsDeleted = getUserDetail.IsDeleted;
            userDetailsObj.ProfileImage = getUserDetail.ProfileImage;
            userDetailsObj.ProfileImageUrl = CommonFunctions.GetRelativeFilePath(String.IsNullOrEmpty(getUserDetail.ProfileImage) ? SiteKeys.DefaultUserPng : getUserDetail.ProfileImage, SiteKeys.UserImageFolderPath);

            response.ResponsePacket = userDetailsObj;
            response.ResponseMessage = ResourceString.Success;
            return response;
        }

        public async Task<TResponse<bool>> UpdateUserDetail(UserDetailsDto requestModel)
        {
            TResponse<bool> response = new();
            bool activeDeactiveObj = false;

            string newImageName = string.Empty;
            if (requestModel.Image != null)
            {
                newImageName = Guid.NewGuid() + Path.GetExtension(requestModel.Image.FileName);

                var folderPath = string.Format("{0}/{1}/{2}", SiteKeys.SitePhysicalPath, "wwwroot", SiteKeys.UserImageFolderPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    requestModel.Image.CopyTo(stream);
                }
            }

            if (!string.IsNullOrEmpty(newImageName))
            {
                requestModel.ProfileImage = newImageName;
            }
            else
            {
                requestModel.ProfileImage = null;
            }

            var updateUser = await _accountRepository.Update(requestModel);

            if (updateUser > 0)
            {
                response.ResponseMessage = ResourceString.ProfileUpdateSuccess;
                response.ResponsePacket = true;
            }
            else
            {
                response.ResponseMessage = ResourceString.ProfileUpdateFailed;
                response.ResponsePacket = activeDeactiveObj;
            }
            return response;
        }

        public async Task<ResponseTypes> ChangePassword(ChangePasswordModel model, long userId)
        {
            var getUserDetail = await _accountRepository.FindByIdAsync(userId);

            if (getUserDetail is null)
            {
                return ResponseTypes.Error;
            }

            if (!Encryption.VerifyHash(model.OldPassword, getUserDetail.PasswordHash))
            {
                return ResponseTypes.OldPasswordWrong;
            }
            else
            {
                if (Encryption.VerifyHash(model.NewPassword, getUserDetail.PasswordHash))
                {
                    return ResponseTypes.OldNewPasswordMatched;
                }
                else
                {
                    var updatePassword = await _accountRepository.ChangePassword(model, userId);
                    if (updatePassword > 0)
                    {
                        return ResponseTypes.Success;
                    }
                    else
                    {
                        return ResponseTypes.Error;
                    }
                }

            }
        }
        public async Task<int> ChangeUserStatus(long userId, bool activeStatus, bool deleteStatus)
        {
            UserDetailsDto requestModel = new();
            requestModel.UserId = userId;
            requestModel.IsActive = activeStatus;
            requestModel.IsDeleted = deleteStatus;
            return await _accountRepository.Update(requestModel);
        }


        public async Task<TResponse<bool>> UpdateLevel(List<LevelInfo> levelInfos)
        {
            TResponse<bool> response = new();
            bool activeDeactiveObj = false;
                        
            var updateUser = await _accountRepository.UpdateLevel(levelInfos);

            if (updateUser > 0)
            {
                response.ResponseMessage = ResourceString.LevelUpdatedSuccessfully;
                response.ResponsePacket = true;
            }
            else
            {
                response.ResponseMessage = ResourceString.LevelUpdateFailed;
                response.ResponsePacket = activeDeactiveObj;
            }
            return response;
        }



    }
}
