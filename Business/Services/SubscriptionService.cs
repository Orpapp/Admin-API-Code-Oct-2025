using Business.IServices;
using Data.IRepository;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.UserSubscription;
using Shared.Model.Request.Admin.UserSubscription;
using Shared.Model.Request.UserSubscription;
using Shared.Model.Response;
using Shared.Resources;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
namespace Business.Services
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAccountRepositry _accountRepository;

        private readonly string _androidInAppAccountEmail;
        private readonly string _androidInAppPackage;
        private readonly string _androidInAppCertificatePath;
        private readonly string _androidInAppGoogleApisURL;
        private readonly string _androidInAppCertificatePassword;
        private readonly string _androidInAppApplicationName;
        private readonly string _iOSInAppSharedSecret;
        public SubscriptionService(ISubscriptionRepository subscriptionRepository, IAccountRepositry accountRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _androidInAppAccountEmail = SiteKeys.AndroidInAppAccountEmail ?? "";
            _androidInAppPackage = SiteKeys.AndroidInAppPackage ?? "";
            _androidInAppCertificatePath = SiteKeys.AndroidInAppCertificatePath ?? "";
            _androidInAppGoogleApisURL = SiteKeys.AndroidInAppGoogleApisURL ?? "";
            _androidInAppCertificatePassword = SiteKeys.AndroidInAppCertificatePassword ?? "";
            _androidInAppApplicationName = SiteKeys.AndroidInAppApplicationName ?? "";
            _iOSInAppSharedSecret = SiteKeys.IOSInAppSharedSecret ?? "";
            _accountRepository = accountRepository;
        }


        public async Task<ApiResponse<List<AppSubscriptionPlansResponse>>> GetAppSubscriptionPlans()
        {
            var plans = await _subscriptionRepository.GetAppSubscriptionPlans();

            if (!plans.Any())
            {
                return new ApiResponse<List<AppSubscriptionPlansResponse>>(null, ResourceString.Fail, "GetAppSubscriptionPlans");
            }

            var response = plans.Select(plan => new AppSubscriptionPlansResponse
            {
                Id = plan.Id,
                Price = plan.Price,
                Title = plan.Title,
                Description = plan.Description?.Split(",  ").Select(d => d.Trim()).ToList() ?? new List<string>(),
                LimitedFeaturesTitle = plan.LimitedFeaturesTitle,
                LimitedFeaturesDescription = plan.LimitedFeaturesDescription?.Split(",  ").Select(d => d.Trim()).ToList() ?? new List<string>(),
                Note = plan.Note,
                ProductId = plan.ProductId,
                ProductType = plan.ProductType,
            }).ToList();

            return new ApiResponse<List<AppSubscriptionPlansResponse>>(response, ResourceString.Success, "GetAppSubscriptionPlans");
        }

        public async Task<ApiResponse<List<UserSubscriptionDto>>> GetSubscriptions(UserSubscriptionDetailsModel request)
        {
            ApiResponse<List<UserSubscriptionDto>> response = new();
            var getUserSubscriptionList = await _subscriptionRepository.GetSubscriptions(request);
            getUserSubscriptionList.ForEach(u =>
            {
                u.PurchaseDate = u.PurchaseOn.ToShortDateString();
                u.ExpiryDate = u.ExpiryOn.ToShortDateString();
            });

            response.Data = getUserSubscriptionList;
            response.Message = ResourceString.Success;
            return response;
        }
        public async Task<ApiResponse<bool>> SaveUserSubscription(UserSubscription subscriptionPlan, int userId)
        {

            try
            {
                subscriptionPlan.SharedSecret = _iOSInAppSharedSecret;
                var objReceipt = await GetReceiptData((ProductTypes)subscriptionPlan.ProductType, subscriptionPlan.Receipt, subscriptionPlan.ProductId, subscriptionPlan.DeviceType, subscriptionPlan.SharedSecret);
                if (!string.IsNullOrEmpty(objReceipt.TransactionId))
                {
                    byte planMonth = 0;
                    UserSubscription userSubscriptionDetail = new()
                    {
                        UserId = userId,
                        ProductType = subscriptionPlan.ProductType,
                        ProductId = objReceipt.ProductId,
                        Receipt = subscriptionPlan.Receipt,
                        TransactionId = objReceipt.TransactionId,
                        PurchaseDate = objReceipt.OriginalPurchaseDate,
                        ExpiryDate = objReceipt.ExpiryDate,
                        PlanMonth = planMonth,
                        Price = subscriptionPlan.Price,
                        DeviceType = subscriptionPlan.DeviceType,
                        SharedSecret = subscriptionPlan.SharedSecret,
                        SubscriptionProductId = subscriptionPlan.SubscriptionProductId,
                        InAppSubscriptionProductId = subscriptionPlan.InAppSubscriptionProductId,
                        IsActive = true,
                        PriceWithCurrencySymbol = subscriptionPlan.PriceWithCurrencySymbol,
                    };
                    var result = await _subscriptionRepository.SaveUserSubscription(userSubscriptionDetail);
                    if (result > 0)
                        return new ApiResponse<bool>(true, message: (ProductTypes)subscriptionPlan.ProductType == ProductTypes.Purchase ?
                           ResourceString.PurchaseStars : ResourceString.PurchaseSubscription, apiName: "SaveUserSubscription");
                    else
                        return new ApiResponse<bool>(false, message: (ProductTypes)subscriptionPlan.ProductType == ProductTypes.Purchase ?
                           ResourceString.FailedPurchaseStars : ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
                }
                else
                {
                    return new ApiResponse<bool>(false, message: (ProductTypes)subscriptionPlan.ProductType == ProductTypes.Purchase ?
                           ResourceString.FailedPurchaseStars : ResourceString.FailedSubscription, apiName: "SaveUserSubscription");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, message: ex.Message, apiName: "SaveUserSubscription");
            }
        }
        public async Task<ApiResponse<UserSubscriptionDetails>> GetUserSubscription(long userId)
        {
            try
            {
                await CheckSubscriptionAvailability(userId);
                var response = await _subscriptionRepository.GetUserSubscription(userId);
                if (response != null)
                {
                    response.Description = response.Desc?.Split(",  ").Select(d => d.Trim()).ToList() ?? new List<string>();
                    response.LimitedFeaturesDescription = response.LimitedFeaturesDesc?.Split(",  ").Select(d => d.Trim()).ToList() ?? new List<string>();

                    return new ApiResponse<UserSubscriptionDetails>(response, message: ResourceString.SubscriptionFetchedSuccessfully, apiName: "GetUserSubscription");
                }
                else
                    return new ApiResponse<UserSubscriptionDetails>(new UserSubscriptionDetails(), message: ResourceString.NoRecordFound, apiName: "GetUserSubscription");
            }
            catch (Exception)
            {
                return new ApiResponse<UserSubscriptionDetails>(null, message: ResourceString.Error, apiName: "GetUserSubscription");
            }
        }
        public async Task<AppAccessibleReponse> CheckSubscriptionAvailability(long userId)
        {
            AppAccessibleReponse appAccessibleReponse = new();
            try
            {
                var userSubscription = await _subscriptionRepository.GetUserSubscription(userId);
                if (userSubscription != null)
                {
                    appAccessibleReponse.ProductType = userSubscription.ProductType;
                    appAccessibleReponse.PurchaseDate = userSubscription.PurchaseDate;
                    var objReceipt = await GetReceiptData((ProductTypes)userSubscription.ProductType, userSubscription.Receipt, userSubscription.ProductId, userSubscription.DeviceType, userSubscription.SharedSecret);
                    //if new expiry date from apple or play store is less than current date then don't allow the users to access the app
                    if (objReceipt?.ExpiryDate < CommonFunctions.GetCurrentDateTime())
                    {
                        objReceipt.ExpiryDate = string.IsNullOrEmpty(objReceipt.TransactionId) ? CommonFunctions.GetCurrentDateTime().AddDays(-1) : objReceipt.ExpiryDate;
                        await _subscriptionRepository.UpdateUserSubscriptionExpiryDate(userSubscription.Id, userId, objReceipt.ExpiryDate.Value, true);
                    }
                    else
                    {
                        if (objReceipt?.ExpiryDate > userSubscription.ExpiryDate)
                        {
                            await _subscriptionRepository.UpdateUserSubscriptionExpiryDate(userSubscription.Id, userId, objReceipt.ExpiryDate.Value, false);
                        }
                        appAccessibleReponse.IsAccessAllowed = true;
                    }
                    appAccessibleReponse.CancelReason = objReceipt?.CancelReason;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return appAccessibleReponse;
        }
        public async Task<ApiResponse<UserSubscriptionResponse>> CheckUserSubscriptionStatus(int userId)
        {
            UserSubscriptionResponse subscriptionResponse = new();
            try
            {
                var user = await _accountRepository.FindByIdAsync(userId);
                if (user != null)
                {
                    var userActiveSubscription = await _subscriptionRepository.GetUserSubscription(userId);
                    if (userActiveSubscription != null)
                    {
                        subscriptionResponse.Id = userActiveSubscription.Id;
                        subscriptionResponse.SubscriptionType = userActiveSubscription.ProductType;
                        subscriptionResponse.ExpiryDate = userActiveSubscription.ExpiryDate;
                        subscriptionResponse.PurchaseDate = userActiveSubscription.PurchaseDate;
                        subscriptionResponse.IsActive = userActiveSubscription.IsActive;
                        subscriptionResponse.InAppSubscriptionProductId = userActiveSubscription.InAppSubscriptionProductId ?? 0;

                        // Check Subscription Trial Period expired or not
                        if (userActiveSubscription.TransactionId == null && userActiveSubscription.ExpiryDate >= DateTime.UtcNow)
                        {
                            subscriptionResponse.IsActive = false;
                            return new ApiResponse<UserSubscriptionResponse>(subscriptionResponse, message: ResourceString.SubscriptionFetchedSuccessfully, apiName: "check_SubscriptionStatus");
                        }
                        else if (userActiveSubscription.TransactionId == null && userActiveSubscription.ExpiryDate < DateTime.UtcNow)
                        {
                            subscriptionResponse.IsActive = false;
                            await _subscriptionRepository.UpdateUserSubscriptionExpiryDate(userActiveSubscription.Id, userId, userActiveSubscription.ExpiryDate, true);
                            return new ApiResponse<UserSubscriptionResponse>(subscriptionResponse, message: ResourceString.UserSubscriptionExpired, apiName: "check_SubscriptionStatus");
                        }
                        else
                        {
                            var objReceipt = await GetReceiptData((ProductTypes)userActiveSubscription.ProductType, userActiveSubscription.Receipt, userActiveSubscription.ProductId, userActiveSubscription.DeviceType, userActiveSubscription.SharedSecret);

                            subscriptionResponse.IsCancelled = !objReceipt.AutoRenew;
                            subscriptionResponse.ExpiryDate = objReceipt.ExpiryDate;
                            subscriptionResponse.PurchaseDate = objReceipt.OriginalPurchaseDate;

                            if (objReceipt.ExpiryDate < DateTime.UtcNow || DateTime.TryParse(objReceipt.ExpiryDate.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate) && parsedDate.Ticks == DateTime.MinValue.Ticks)
                            {
                                subscriptionResponse.IsActive = false;
                                await _subscriptionRepository.UpdateUserSubscriptionExpiryDate(userActiveSubscription.Id, userId, objReceipt.ExpiryDate, true);
                                return new ApiResponse<UserSubscriptionResponse>(subscriptionResponse, message: ResourceString.UserSubscriptionExpired, apiName: "check_SubscriptionStatus");
                            }
                            else
                            {
                                subscriptionResponse.IsActive = true;
                                await _subscriptionRepository.UpdateUserSubscriptionExpiryDate(userActiveSubscription.Id, userId, objReceipt.ExpiryDate, false);
                                return new ApiResponse<UserSubscriptionResponse>(subscriptionResponse, message: ResourceString.SubscriptionFetchedSuccessfully, apiName: "check_SubscriptionStatus");
                            }
                        }
                    }
                    else
                    {
                        return new ApiResponse<UserSubscriptionResponse>(new UserSubscriptionResponse(), message: ResourceString.UserNotSubscribed, apiName: "check_SubscriptionStatus");
                    }
                }
                else
                {
                    return new ApiResponse<UserSubscriptionResponse>(null, message: ResourceString.UserNotExist, apiName: "check_SubscriptionStatus");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserSubscriptionResponse>(null, message: ex.Message, apiName: "check_SubscriptionStatus");
            }
        }
        public async Task<ApiResponse<bool>> CancelUserSubscription(long userId)
        {
            try
            {
                var userSubscription = await _subscriptionRepository.GetUserSubscription(userId);
                if (userSubscription != null)
                {
                    var responseCancel = GetReceiptDataForCancel(userSubscription.Receipt, userSubscription.ProductId, userSubscription.DeviceType);

                    if (responseCancel != null && responseCancel.Length == 0)
                    {
                        return new ApiResponse<bool>(true, message: ResourceString.SubscriptionCanceledSuccessfully, apiName: "CancelUserSubscription");
                    }
                    else
                        return new ApiResponse<bool>(false, message: ResourceString.SubscriptionCancellationFailed, apiName: "CancelUserSubscription");
                }
                else
                    return new ApiResponse<bool>(false, message: ResourceString.NoRecordFound, apiName: "CancelUserSubscription");
            }
            catch (Exception)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "CancelUserSubscription");
            }
        }
        private async Task<ReceiptResult> GetReceiptData(ProductTypes productType, string receiptData, string subscriptionId, int deviceType, string sharedSecret)
        {
            ReceiptResult obj = new();

            if (deviceType == 1) //android
            {
                obj = await GetAndroidInAppReceiptDetail(productType, subscriptionId, receiptData);
            }
            else if (deviceType == 2 && !string.IsNullOrEmpty(receiptData)) //iOS
            {
                // make sandbox false before going live
                Receipt receipt = ReceiptVerification.GetReceipt(false, receiptData, sharedSecret);

                //Do the actual verification // this makes the https post to itunes verification servers
                if (ReceiptVerification.IsReceiptValid(receipt) && receipt != null)
                {
                    //split the receipt information

                    if (string.IsNullOrEmpty(Convert.ToString(receipt.PurchaseDate)))
                    {
#pragma warning disable S112 // General exceptions should never be thrown
                        throw new Exception("Invalid purchase date");
#pragma warning restore S112 // General exceptions should never be thrown
                    }

                    obj.OriginalPurchaseDate = Convert.ToDateTime(receipt.PurchaseDate);
                    obj.ExpiryDate = receipt.expires_date == null ? null : Convert.ToDateTime(receipt.expires_date);
                    obj.ProductId = receipt.ProductId;
                    obj.TransactionId = receipt.TransactionId;
                    obj.AutoRenew = receipt.AutoRenew;
                    obj.IsTrial = receipt.Is_Trial_Period;
                }
            }
            return obj;
        }
        private async Task<ReceiptResult> GetAndroidInAppReceiptDetail(ProductTypes productType, string productId, string receiptToken)
        {
            ReceiptResult obj = new();
            try
            {
                var certificate = new X509Certificate2(_androidInAppCertificatePath, _androidInAppCertificatePassword, X509KeyStorageFlags.MachineKeySet);

                ServiceAccountCredential credential = new(
                   new ServiceAccountCredential.Initializer(_androidInAppAccountEmail)
                   {
                       Scopes = new[] { _androidInAppGoogleApisURL }
                   }.FromCertificate(certificate));

                var service = new AndroidPublisherService(
               new BaseClientService.Initializer()
               {
                   HttpClientInitializer = credential,
                   ApplicationName = _androidInAppApplicationName,
               });
                var pur = service.Purchases;

                if (productType == ProductTypes.Purchase)
                {
                    var purchase = await pur.Products.Get(_androidInAppPackage, productId, receiptToken).ExecuteAsync();

                    if (purchase.PurchaseState == 0)
                    {
                        double ticks = double.Parse(Convert.ToString(purchase.PurchaseTimeMillis ?? 0));//InitiationTimestampMsec
                        TimeSpan time = TimeSpan.FromMilliseconds(ticks);
                        DateTime dtPurchase = new DateTime(1970, 1, 1) + time;
                        obj.OriginalPurchaseDate = dtPurchase;
                        obj.ExpiryDate = null;
                        obj.ProductId = productId;
                        obj.TransactionId = purchase.OrderId;// sub.Kind;
                        obj.CancelReason = 0;
                    }
                }
                else
                {
                    var sub = pur.Subscriptions.Get(_androidInAppPackage, productId, receiptToken).Execute();
                    double ticks = double.Parse(Convert.ToString(sub.StartTimeMillis ?? 0));//InitiationTimestampMsec
                    TimeSpan time = TimeSpan.FromMilliseconds(ticks);
                    DateTime dtPurchase = new DateTime(1970, 1, 1) + time;

                    ticks = double.Parse(Convert.ToString(sub.ExpiryTimeMillis ?? 0));//ValidUntilTimestampMsec
                    time = TimeSpan.FromMilliseconds(ticks);
                    DateTime dtExpiry = new DateTime(1970, 1, 1) + time;

                    obj.OriginalPurchaseDate = dtPurchase;
                    obj.ExpiryDate = dtExpiry;
                    obj.ProductId = productId;
                    obj.TransactionId = sub.OrderId;// sub.Kind;
                    obj.CancelReason = sub.CancelReason;
                    obj.AutoRenew = sub.AutoRenewing ?? false;
                    obj.IsTrial = sub.PaymentState == 2;
                }
            }
            catch (Exception ex)
            {
#pragma warning disable S112 // General exceptions should never be thrown
                throw new Exception("GetAndroidInAppReceiptDetail_" + ex.Message);
#pragma warning restore S112 // General exceptions should never be thrown

            }
            return obj;
        }
        private string GetReceiptDataForCancel(string receiptData, string productId, int deviceType)
        {
            string response = string.Empty;
            if (deviceType == 1) //android
            {
                response = GetAndroidInAppReceiptDetailForCancel(productId ?? "", receiptData);
            }
            return response;
        }
        private string GetAndroidInAppReceiptDetailForCancel(string subscriptionId, string receiptToken)
        {
            string response = string.Empty;
            try
            {
                var certificate = new X509Certificate2(_androidInAppCertificatePath, _androidInAppCertificatePassword, X509KeyStorageFlags.MachineKeySet);

                ServiceAccountCredential credential = new(
                   new ServiceAccountCredential.Initializer(_androidInAppAccountEmail)
                   {
                       Scopes = new[] { _androidInAppGoogleApisURL }
                   }.FromCertificate(certificate));

                var service = new AndroidPublisherService(
               new BaseClientService.Initializer()
               {
                   HttpClientInitializer = credential,
                   ApplicationName = _androidInAppApplicationName,
               });
                var pur = service.Purchases;
                var sub = pur.Subscriptions.Cancel(_androidInAppPackage, subscriptionId, receiptToken).Execute();
                response = sub;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return response;
        }
    }
}
