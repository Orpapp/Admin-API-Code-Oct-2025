using Shared.Model.Request.Admin;

namespace Data.IRepository
{
    public interface ILevelVoucherRepository
    {
        Task<VoucherModel> GetVoucher(int levelNumber);
        Task<List<VoucherModel>> GetAllVouchers();
        Task<int> UpdateVoucherImage(VoucherModel voucherModel);
    }
}
