using Shared.Model.Base;
using Shared.Model.Request.Admin;

namespace Business.IServices
{
    public interface ILevelVoucherService
    {
        Task<ApiResponse<VoucherModel>> GetVoucher(int levelNumber);
        Task<List<VoucherModel>> GetAllVouchers();
        Task<int> UpdateVoucherImage(VoucherModel voucherModel);
    }
}
