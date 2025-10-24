using Business.IServices;
using Data.IRepository;
using Data.Repository;
using Google.Apis.AndroidPublisher.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.Shop;
using Shared.Model.Request.Admin.Shop;
using Shared.Model.Request.Shop;
using Shared.Model.Response;
using Shared.Resources;
namespace Business.Services
{
    public class ShopService : IShopService
    {

        private readonly IShopRepository _shopRepository;
        private readonly ISubscriptionProductRepository _subscriptionProductRepository;
        public ShopService(IShopRepository shopRepository, ISubscriptionProductRepository subscriptionProductRepository)
        {
            _shopRepository = shopRepository;
            _subscriptionProductRepository = subscriptionProductRepository;
        }
        public async Task<ApiResponse<AvailableBalanceResponse>> GetAvailableBalance(int userId)
        {
            var availableBalance = await _shopRepository.GetAvailableBalance(userId);
            if (availableBalance is null)
            {
                return new ApiResponse<AvailableBalanceResponse>(new AvailableBalanceResponse(), message: ResourceString.Success, apiName: "GetAvailableBalance");
            }
            return new ApiResponse<AvailableBalanceResponse>(availableBalance, message: ResourceString.Success, apiName: "GetAvailableBalance");
        }
        public async Task<ApiResponse<List<ShopProductResponse>>> GetShopProductList()
        {
            var shopPriceResponses = await _subscriptionProductRepository.SubscriptionProductListByType(ProductTypes.Purchase);
            if (shopPriceResponses is null)
            {
                return new ApiResponse<List<ShopProductResponse>>([], message: ResourceString.Success, apiName: "GetShopPriceList");
            }
            return new ApiResponse<List<ShopProductResponse>>(shopPriceResponses, message: ResourceString.Success, apiName: "GetShopPriceList");
        }
        public async Task<ApiResponse<List<ShopItemResponse>>> GetShopItemList(ItemsTypes itemsType)
        {
            List<ShopItemResponse> shopItemResponse = [];
            var response = await _shopRepository.GetItemListByType(itemsType);
            if (response is null)
            {
                return new ApiResponse<List<ShopItemResponse>>(shopItemResponse, message: ResourceString.Success, apiName: "GetShopPriceList");
            }
            var categories = response.Select(p => p.Category).Distinct();
            foreach (var category in categories)
            {
                shopItemResponse.Add(new ShopItemResponse
                {
                    Name = category,
                    Items = response.Where(p => p.Category == category).OrderBy(p => p.DisplayOrder).Select(p => new ShopCategoryItem
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ImagePath = $"{SiteKeys.SiteUrl}{SiteKeys.ItemsImage}{p.ImagePath}",
                        Stars = p.Stars,
                        ItemType = p.ItemType,
                        GreenPieces = p.GreenPieces,
                        SkyPieces = p.SkyPieces,
                        PurplePieces = p.PurplePieces,
                        YellowPieces = p.YellowPieces,
                        IsOpen = p.IsOpen
                    }).ToList()
                });
            }
            return new ApiResponse<List<ShopItemResponse>>(shopItemResponse, message: ResourceString.Success, apiName: "GetShopItemList");
        }

        public async Task<ApiResponse<List<Shop>>> GetShopByUser(int userId)
        {
            try
            {
                var result = await _shopRepository.GetShopListByUserId(userId);
                if (result.Count > 0)
                {
                    return new ApiResponse<List<Shop>>(result, message: ResourceString.PointsPurchaseSuccess, apiName: "AddShop");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Shop>>(null, message: ex.Message, apiName: "AddShop");

            }
            return new ApiResponse<List<Shop>>(null, message: ResourceString.Error, apiName: "AddShop");
        }
        public async Task<ApiResponse<bool>> AddShop(List<Shop> request, int userId)
        {
            try
            {
                int result = await _shopRepository.AddUpdateShop(request, userId);
                if (result == -1)
                {
                    return new ApiResponse<bool>(false, message: ResourceString.StarsNotAvailable, apiName: "AddShop");
                }
                else if (result > 0)
                {
                    return new ApiResponse<bool>(true, message: ResourceString.PointsPurchaseSuccess, apiName: "AddShop");
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, message: ex.Message, apiName: "AddShop");

            }
            return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "AddShop");
        }

        public async Task<ApiResponse<List<ShopDto>>> GetShops(ShopDetailsModel request)
        {
            ApiResponse<List<ShopDto>> response = new();
            var getShopList = await _shopRepository.GetShops(request);
            getShopList.ForEach(u =>
            {
                u.PurchaseDate = u.AddedOnUTC.ToShortDateString();
            });
            response.Data = getShopList;
            response.Message = ResourceString.Success;
            return response;
        }
    }
}
