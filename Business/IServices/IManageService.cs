using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IManageService
    {
        Task<TResponse<List<UserListDto>>> UserList(UsersRequestModel request);
        Task<TResponse<UserDetailsDto>> GetUserDetails(long userId);
        Task<TResponse<bool>> UpdateUserDetail(UserDetailsDto requestModel);
        Task<TResponse<bool>> UpdateLevel(List<LevelInfo> levelInfos);
        Task<ResponseTypes> ChangePassword(ChangePasswordModel model, long userId);
        Task<int> ChangeUserStatus(long userId, bool activeStatus, bool deleteStatus);
    }
}
