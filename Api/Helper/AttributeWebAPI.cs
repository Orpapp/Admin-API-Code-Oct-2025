using Business.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Common;
using Shared.Model.Base;
using Shared.Resources;

namespace Api.Helper
{
    public class AttributeWebAPI : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var svc = context.HttpContext.RequestServices;
            var userAccountService = svc.GetService<IAccountService>();
            ApiResponse<JsonResult> jsonResponse = new ApiResponse<JsonResult>();

            string? authorizationToken = context.HttpContext.Request.Headers["Authorization"];
            string? utcOffsetInSecond = context.HttpContext.Request.Headers["UtcOffsetInSecond"];
            string? accessToken = context.HttpContext.Request.Headers["AccessToken"];
            string? appVersion = context.HttpContext.Request.Headers["AppVersion"];

            #region Header check
            bool hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                 .Any(em => em.GetType() == typeof(AllowAnonymousAttribute)); //< -- Here it is

            if (!hasAllowAnonymous && authorizationToken == null)
            {
                jsonResponse.Data = null;
                jsonResponse.Message = ResourceString.JWTtokenRequired;
                context.Result = new UnauthorizedObjectResult(jsonResponse);
                return;
            }

            if (utcOffsetInSecond == null)
            {
                jsonResponse.Data = null;
                jsonResponse.Message = ResourceString.UtcOffsetInSecond;
                context.Result = new BadRequestObjectResult(jsonResponse);
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(utcOffsetInSecond))
                {
                    SiteKeys.UtcOffsetInSecond_API = Convert.ToInt32(utcOffsetInSecond);
                }
            }

            if (appVersion == null)
            {
                jsonResponse.Data = null;
                jsonResponse.Message = ResourceString.AppVersion;
                context.Result = new BadRequestObjectResult(jsonResponse);
                return;
            }
            else
            {
                var isLatestAppVersion = userAccountService?.CheckAppVersion(appVersion);
                if (!(isLatestAppVersion ?? true))
                {
                    jsonResponse.Data = null;
                    jsonResponse.Message = ResourceString.OldAppVersion;
                    context.Result = new UnauthorizedObjectResult(jsonResponse);
                    return;
                }
            }
            #endregion
            #region Access Token check here
            if (!hasAllowAnonymous)
            {
                if (accessToken == null)
                {
                    jsonResponse.Data = null;
                    jsonResponse.Message = ResourceString.AccessTokenRequired;
                    context.Result = new BadRequestObjectResult(jsonResponse);
                    return;
                }
                else
                {
                    var objUserTokenModel = userAccountService?.CheckUserAccessToken(accessToken);
                    if (objUserTokenModel != null)
                    {
                        if (!objUserTokenModel.IsTokenExists)
                        {
                            jsonResponse.Data = null;
                            jsonResponse.Message = ResourceString.AnotherDeviceLogin;
                            context.Result = new UnauthorizedObjectResult(jsonResponse);
                            return;
                        }
                        else if (!objUserTokenModel.IsActive)
                        {
                            jsonResponse.Data = null;
                            jsonResponse.Message = ResourceString.UserIsNotActive;
                            context.Result = new UnauthorizedObjectResult(jsonResponse);
                            return;
                        }
                        else if (objUserTokenModel.IsDeleted)
                        {
                            jsonResponse.Data = null;
                            jsonResponse.Message = ResourceString.UserAccountDeleted;
                            context.Result = new UnauthorizedObjectResult(jsonResponse);
                            return;
                        }
                    }
                    else
                    {
                        jsonResponse.Data = null;
                        jsonResponse.Message = ResourceString.SessionExpired;
                        context.Result = new UnauthorizedObjectResult(jsonResponse);
                        return;
                    }
                }
            }

            #endregion

            base.OnActionExecuting(context);
        }
    }
}
