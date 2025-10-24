using Shared.Model.Request.Account;
using Shared.Model.Response;

namespace Data.IRepository
{
    public interface IProfileRepository : IGenericRepository<Profile>
    {
        Task<ProfileDetailsResponse> GetUserProfileById(int id);
        Task<PreferenceModel> GetPreferenceById(int userId);
        Task<List<Item>> SearchPreference(string prefrence, int type);
        Task<int> UpdatePreference(List<PrefenceRequest> request, int id);
        Task<int> UpdateUserName(UpdateUserNameRequest request, int userId);
        Task<int> AccountDelete(int id);

        Task<List<VoucherImages>> GetUserVouchers(int userId);
    }
}

