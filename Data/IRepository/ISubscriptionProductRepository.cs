using Shared.Common.Enums;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.IRepository
{
    public interface ISubscriptionProductRepository
    {
        Task<List<ShopProductResponse>> SubscriptionProductListByType(ProductTypes productType);
    }
}
