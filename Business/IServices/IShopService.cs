using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO.Admin.Shop;
using Shared.Model.Request.Admin.Shop;
using Shared.Model.Request.Shop;
using Shared.Model.Response;

namespace Business.IServices
{
    public interface IShopService
    {
        Task<ApiResponse<AvailableBalanceResponse>> GetAvailableBalance(int userId);
        Task<ApiResponse<bool>> AddShop(List<Shop> request,int userId);
        Task<ApiResponse<List<Shop>>> GetShopByUser(int userId);
        Task<ApiResponse<List<ShopDto>>> GetShops(ShopDetailsModel request);
        Task<ApiResponse<List<ShopProductResponse>>> GetShopProductList();
        Task<ApiResponse<List<ShopItemResponse>>> GetShopItemList(ItemsTypes itemsType);

    }

}
