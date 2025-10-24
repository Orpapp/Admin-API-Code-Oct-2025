using Business.IServices;
using Data.IRepository;
using Shared.Common;
using Shared.Model.Base;
using Shared.Model.Request.Admin;
using Shared.Resources;

namespace Business.Services
{
    public class LevelVoucherService : ILevelVoucherService
    {

        private readonly ILevelVoucherRepository _levelVoucherRepository;
        public LevelVoucherService(ILevelVoucherRepository levelVoucherRepository)
        {
            _levelVoucherRepository = levelVoucherRepository;
        }
        public async Task<ApiResponse<VoucherModel>> GetVoucher(int levelNumber)
        {
            var voucher = await _levelVoucherRepository.GetVoucher(levelNumber);
            if (voucher is null)
            {
                return new ApiResponse<VoucherModel>(new VoucherModel(), message: ResourceString.Success, apiName: "GetVoucher");
            }
            voucher.ImagePath = string.IsNullOrEmpty(voucher.ImagePath) ? $"{SiteKeys.SiteUrl}{SiteKeys.DefaultVoucherImage}" : CommonFunctions.GetRelativeFilePath(voucher.ImagePath, SiteKeys.VoucherImageFolderPath);
            return new ApiResponse<VoucherModel>(voucher, message: ResourceString.Success, apiName: "GetVoucher");
        }

        public async Task<List<VoucherModel>> GetAllVouchers()
        {
            var vouchers = await _levelVoucherRepository.GetAllVouchers();
            vouchers.ForEach(voucher =>
            {
                voucher.ImagePath = string.IsNullOrEmpty(voucher.ImagePath) ?$"{SiteKeys.SiteUrl}{SiteKeys.DefaultVoucherImage}": CommonFunctions.GetRelativeFilePath(voucher.ImagePath, SiteKeys.VoucherImageFolderPath);
            });
            return vouchers;
        }

        public async Task<int> UpdateVoucherImage(VoucherModel voucherModel)
        {
            string newImageName = string.Empty;
            if (voucherModel.Image != null)
            {
                newImageName = Guid.NewGuid() + Path.GetExtension(voucherModel.Image.FileName);

                var folderPath = string.Format("{0}/{1}", SiteKeys.SitePhysicalPath, SiteKeys.VoucherImagePhysical);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var filePath = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    voucherModel.Image.CopyTo(stream);
                }
            }

            if (!string.IsNullOrEmpty(newImageName))
            {
                voucherModel.ImagePath = newImageName;
            }
            else
            {
                voucherModel.ImagePath = null;
            }


            return await _levelVoucherRepository.UpdateVoucherImage(voucherModel);
        }
    }
}
