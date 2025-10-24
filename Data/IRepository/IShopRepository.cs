using Shared.Common.Enums;
using Shared.Model.DTO.Admin.Shop;
using Shared.Model.Request.Admin.Shop;
using Shared.Model.Request.Items;
using Shared.Model.Request.Shop;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IShopRepository : IGenericRepository<Shop>
    {
        Task<int> AddUpdateShop(List<Shop> shops,int userId);
        Task<List<ShopItems>> GetItemListByType(ItemsTypes itemType);
        Task<List<Shop>> GetShopListByUserId(int userId); 
        Task<List<ShopDto>> GetShops(ShopDetailsModel request);
        Task<AvailableBalanceResponse> GetAvailableBalance(int userId);
    }
}

