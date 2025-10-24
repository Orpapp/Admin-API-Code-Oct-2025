using Business.IServices;
using Data.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.Level;
using Shared.Model.Request.Task;
using Shared.Model.Response;
using Shared.Resources;
using System.Data;
using System.Drawing;
using System.IO;
namespace Business.Services
{
    public class LevelService : ILevelService
    {

        private readonly ILevelRepository _levelRepository;
        public LevelService(ILevelRepository taskRepository)
        {
            _levelRepository = taskRepository;
        }


        public async Task<ApiResponse<List<GetStopageListResponse>>> GetStoppageList(int userId)
        {
            List<GetStopageListResponse> getStoppageListResponse = new();

            var stoppageList = await _levelRepository.GetStoppageList(userId);

            if (stoppageList != null && stoppageList.Any())
            {
                foreach (var item in stoppageList)
                {

                    string stoppageImage1 = string.Empty;
                    string stoppageImage2 = string.Empty;

                    if ((item.StoppageImage ?? "").ToLower().Contains("box"))
                    {
                        stoppageImage1 = "box5";
                        stoppageImage2 = "box1";
                    }
                    else if ((item.StoppageImage ?? "").ToLower().Contains("prize"))
                    {
                        stoppageImage1 = "prize1";
                        stoppageImage2 = "prize2";
                    }
                    else if ((item.StoppageImage ?? "").ToLower().Contains("star"))
                    {
                        stoppageImage1 = "star1";
                        stoppageImage2 = "star2";
                    }
                    else
                    {
                        stoppageImage1 = item.StoppageImage ?? "";
                        stoppageImage2 = item.StoppageImage ?? "";
                    }


                    GetStopageListResponse getStoppage = new()
                    {
                        StoppageImage = $"{SiteKeys.SiteUrl}{SiteKeys.StoppageImage}{stoppageImage1}{".png"}",
                        StoppageImage2 = $"{SiteKeys.SiteUrl}{SiteKeys.StoppageImage}{stoppageImage2}{".png"}",
                        Points = item.Points,
                        LevelId = item.LevelId,
                    };
                    getStoppageListResponse.Add(getStoppage);
                }
            }
          
            return new ApiResponse<List<GetStopageListResponse>>(getStoppageListResponse, message: ResourceString.Success, apiName: "GetStoppageList");
        }


        public async Task<ApiResponse<GetUserLevelResponse>> GetUserLevel(int userId)
        {
            GetUserLevelResponse getStoppageListResponse = new();

            var userLevel = await _levelRepository.GetUserLevel(userId);

            if (userLevel != null)
            {
                if (!string.IsNullOrEmpty(userLevel.UserThumbImage) && userLevel.UserThumbImage.Equals("DefaultImage.png", StringComparison.OrdinalIgnoreCase))
                {
                    userLevel.UserThumbImage = string.Empty;
                }


                GetUserLevelResponse getUserLevelResponse = new()
                {
                    TotalPoints = userLevel.TotalPoints,
                    Star = 0,
                    BackgroundImage = !string.IsNullOrEmpty(userLevel.BackgroundImage) ? $"{SiteKeys.SiteUrl}{SiteKeys.GetLevelBackgroundImage}{userLevel.BackgroundImage}" : $"{SiteKeys.SiteUrl}{SiteKeys.DefaultBackgroundImage}",
                    UserThumbImage = !string.IsNullOrEmpty(userLevel.UserThumbImage) ? $"{SiteKeys.SiteUrl}{SiteKeys.UserImageFolderPath}{"_thumb"+userLevel.UserThumbImage}" : $"{SiteKeys.SiteUrl}{SiteKeys.DefaultSmallUserImage}", // userLevel.UserThumbImage,
                    Fire = 0,
                    TodayEarnedPoint = userLevel.TodayEarnedPoint,
                    TodayPoint = userLevel.TodayPoint,
                };
                return new ApiResponse<GetUserLevelResponse>(getUserLevelResponse, message: ResourceString.Success, apiName: "GetUserLevel");
            }
            return new ApiResponse<GetUserLevelResponse>(getStoppageListResponse, message: ResourceString.Success, apiName: "GetUserLevel");
        }

        public async Task<ApiResponse<GetUserRewardResponse>> GetUserReward(int userId)
        {
            var userReward = await _levelRepository.GetUserReward(userId);

            if (userReward != null)
            {
                if (!string.IsNullOrEmpty(userReward.Image) && userReward.Image.Equals("DefaultImage.png", StringComparison.OrdinalIgnoreCase))
                {
                    userReward.Image = string.Empty;
                }

                GetUserRewardResponse getUserRewardResponse = new()
                {
                    Type = userReward.Type,
                    //Image = !string.IsNullOrEmpty(userReward.Image) ? $"{SiteKeys.SiteUrl}{SiteKeys.UserImageFolderPath}{"_thumb" + userReward.Image}" : $"{SiteKeys.SiteUrl}{SiteKeys.DefaultSmallUserImage}", // userLevel.UserThumbImage,
                    NumberOfPuzzles = userReward.NumberOfPuzzles,
                };
                if(getUserRewardResponse.Type == "Voucher")
                {
                    getUserRewardResponse.Image = string.IsNullOrEmpty(userReward.Image) ? $"{SiteKeys.SiteUrl}{SiteKeys.DefaultVoucherImage}" : CommonFunctions.GetRelativeFilePath(userReward.Image, SiteKeys.VoucherImageFolderPath);
                }
                else if(getUserRewardResponse.Type == "Puzzle")
                {
                    getUserRewardResponse.Image = $"{SiteKeys.SiteUrl}{SiteKeys.ItemsImage}{userReward.Image}";
                }
                return new ApiResponse<GetUserRewardResponse>(getUserRewardResponse, message: ResourceString.Success, apiName: "GetUserReward");
            }
            return new ApiResponse<GetUserRewardResponse>(null, message: ResourceString.Fail, apiName: "GetUserReward");
        }



        public async Task<ApiResponse<bool>> UpdateLevelBackgroundImage(IFormFile? file, bool imageDeleted)
        {
            try
            {
                string formFileName = string.Empty;
                if (file != null)
                {
                    formFileName = file.FileName;
                    if (file.FileName.Length > 80)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        formFileName = file.FileName.Substring(0, 80);
                        formFileName += extension;
                    }

                    var path = $"{SiteKeys.SitePhysicalPath}{SiteKeys.LevelBackgroundImage}{"\\"}{formFileName}";

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                var isImageUpdate = await _levelRepository.UpdateLevelBackgroundImage(formFileName, imageDeleted);

                return new ApiResponse<bool>(isImageUpdate, message: isImageUpdate ? ResourceString.LevelImageUpdatedSuccessfully : ResourceString.LevelImageUpdateFailed, apiName: "UpdateLevelBackgroundImage");
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, message: ex.Message != null ? ex.Message.ToString() : string.Empty, apiName: "UpdateLevelBackgroundImage");
            }
            
        }


        public async Task<ApiResponse<LevelImage>> GetLevelImage()
        {
            LevelImage levelImage = await _levelRepository.GetLevelImage();

            if (levelImage != null && !string.IsNullOrEmpty(levelImage.LevelPicture))
            {
                levelImage.LevelPicture = $"{SiteKeys.SiteUrl}{SiteKeys.GetLevelBackgroundImage}{levelImage.LevelPicture}";
            }

            return new ApiResponse<LevelImage>(levelImage, message: ResourceString.Success, apiName: "GetLevelImage");
        }
    }
}