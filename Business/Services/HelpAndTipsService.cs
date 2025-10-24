using Business.IServices;
using Data.IRepository;
using Shared.Model.Base;
using Shared.Model.Response;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class HelpAndTipsService : IHelpAndTipsService
    {
        private readonly IHelpAndTipsRepository _helpAndTipsRepository;

        public HelpAndTipsService(IHelpAndTipsRepository helpAndTipsRepository)
        {
            _helpAndTipsRepository = helpAndTipsRepository;
        }

        public async Task<ApiResponse<List<HelpAndTips>>> GetHelpAndTips()
        {
            var helpAndTips = await _helpAndTipsRepository.GetHelpAndTips();

            if (!helpAndTips.Any())
            {
                return new ApiResponse<List<HelpAndTips>>(null, ResourceString.Fail, "GetHelpAndTips");
            }

            return new ApiResponse<List<HelpAndTips>>(helpAndTips, ResourceString.Success, "GetHelpAndTips");
        }
    }
}
