using Dapper;
using Data.IFactory;
using Data.IRepository;
using Newtonsoft.Json.Serialization;
using Shared.Common.Enums;
using Shared.Model.DTO.Account;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Response;
using Shared.Utility;
using System.Data;
using System.Data.Common;

namespace Data.Repository
{
    public class AccountRepository(IDbConnectionFactory dbConnection) : IAccountRepositry
    {
        public async Task<UserDetailsDto> FindByIdAsync(long id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @UserId = id
            });
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<UserDetailsDto>("GetUserById", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> Add(UserDetailsDto request)
        {

            var parameters = new DynamicParameters(new
            {
                @FirstName = request.FirstName,
                @LastName = request.LastName,
                @Email = request.Email,
                @Password = Encryption.ComputeHash(request.Password),
                @UserType = UserTypes.User,
            });
            return await dbConnection.CreateDBConnection().ExecuteScalarAsync<int>("AddUser", param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> Update(UserDetailsDto request)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @UserId = request.UserId,
                @IsActive = request.IsActive,
                
                @Name = request.FirstName,
                @PhoneNumber = request.PhoneNumber,
                @Token = request.Token,
                @Email = request.Email,
                @ResetToken = request.ResetToken,
                @EmailVerificationToken = request.EmailVerificationToken,
               
            });

            return await dbConnection.CreateDBConnection().ExecuteAsync("UpdateUser", param: parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserDetailsDto> FindByEmailAsync(string email)
        {
            var parms = new
            {
                @Email = email
            };
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<UserDetailsDto>("GetUserByEmail", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserDetailsDto> SocialLogin(SocialLoginRequest socialLoginRequest, string accessToken)
        {
            var parms = new
            {
                DeviceToken = socialLoginRequest.DeviceToken,
                DeviceType = socialLoginRequest.DeviceType,
                Email = socialLoginRequest.Email,
                FirstName = socialLoginRequest.FirstName,
                LastName = socialLoginRequest.LastName,
                SocialId = socialLoginRequest.SocialId,
                SocialType = socialLoginRequest.SocialType,
                AccessToken = accessToken,
            };
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<UserDetailsDto>("SocialLogin", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckIfUserExistForSocialLogin(string? socialId, string email)
        {
            var parameters = new 
            { 
                @SocialId = socialId, 
                @Email = email 
            };
            
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<int>("CheckUserForSocialLogin", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateDeviceToken(UpdateDeviceTokenRequest request)
        {
            var parms = new
            {
                @Email = request.Email,
                @DeviceType = request.DeviceType,
                @DeviceToken = request.DeviceToken,
                @AccessToken = request.AccessToken
            };
            return await dbConnection.CreateDBConnection().ExecuteAsync("ManageDeviceToken", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<UserListDto>> UserList(UsersRequestModel request)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.AddDynamicParams(new
            {
                @PageNo = request.PageStart,
                @PageSize = request.PageSize,
                @SearchKeyword = request.SearchKeyword,
                @SortColumn = request.SortColumn,
                @SortOrder = request.SortOrder,
                @Role = (int)UserTypes.User
            });
            return (await dbConnection.CreateDBConnection().QueryAsync<UserListDto>("GetUserByRole", parameters, commandType: CommandType.StoredProcedure)).AsList();
        }

        public async Task<int> ChangePassword(ChangePasswordModel model, long userId)
        {
            return await dbConnection.CreateDBConnection().ExecuteAsync("ChangePassword", new
            {
                UserId = userId,
                Password = Encryption.ComputeHash(model.NewPassword),
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> Logout(int userId)
        {
            var parms = new
            {
                @Id = userId
            };
            return await dbConnection.CreateDBConnection().ExecuteAsync("LogoutAccount", param: parms, commandType: CommandType.StoredProcedure);
        }
        public async Task<ForgotPasswordDto> ResetPasswordTokenAsync(long userId, string forgotPasswordToken)
        {
            var parms = new
            {
                @UserId = userId,
                @ForgotPasswordToken = forgotPasswordToken
            };
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<ForgotPasswordDto>("UpdateForgotPasswordToken", param: parms, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> CheckResetPasswordTokenExist(string token)
        {
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<bool>("CheckResetPasswordTokenExistByToken", new { Token = token }, commandType: CommandType.StoredProcedure);
        }

        public async Task<UserDetailsDto> GetUserDetailByToken(string token)
        {
            return await dbConnection.CreateDBConnection().QueryFirstOrDefaultAsync<UserDetailsDto>("GetUserDetailByToken", new { Token = token }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> ResetPassword(ResetPasswordModel model)
        {
            return await dbConnection.CreateDBConnection().ExecuteAsync("UpdateUserByToken", new
            {
                ForgotPasswordToken = model.Token,
                Password = Encryption.ComputeHash(model.Password),
            }, commandType: CommandType.StoredProcedure);
        }

        public CheckUserAccessTokenDto CheckUserAccessToken(string accessToken)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AccessToken", accessToken);
            return dbConnection.CreateDBConnection().QueryFirstOrDefault<CheckUserAccessTokenDto>("CheckAccessTokenExists", parameters, commandType: CommandType.StoredProcedure);
        }

        public bool CheckAppVersion(string appVersion)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AppVersion", appVersion);
            return dbConnection.CreateDBConnection().QueryFirstOrDefault<bool>("CheckAppVersion", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateLevel(List<LevelInfo> levelInfos)
        {
            DynamicParameters parameters = new DynamicParameters();

            var levels = new DataTable();
            levels.Columns.Add("Id", typeof(int));
            levels.Columns.Add("LevelId", typeof(int));
            levels.Columns.Add("TotalPoints", typeof(int));
            levels.Columns.Add("Points", typeof(int));
            levels.Columns.Add("StoppageType", typeof(string));
            levels.Columns.Add("StoppageImage", typeof(string));

            foreach (var item in levelInfos)
            {
                levels.Rows.Add(item.Id,item.LevelId,item.TotalPoints, item.Points, item.StoppageType, item.StoppageImage);
            }

            parameters.AddDynamicParams(new
            {
                @StopageLevel = levels
            });

            return await dbConnection.CreateDBConnection().ExecuteAsync("UpdateLevel", param: parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
