using Shared.Model.Base;
using Shared.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.IServices
{
    public interface IHelpAndTipsService
    {
        Task<ApiResponse<List<HelpAndTips>>> GetHelpAndTips();
    }
}
