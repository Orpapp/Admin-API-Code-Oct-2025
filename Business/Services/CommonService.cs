using Business.IServices;
using Microsoft.AspNetCore.Http;
using Shared.Common;

namespace Business.Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommonService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DateTime GetTimeSubtractOffSet(DateTime dt) // convert local time into UTC time
        {
            return dt.AddSeconds(GetOffsetSecond_FromKey() * -1);
        }
        public DateTime GetTimeAfterAddOffSet(DateTime dt) // convert UTC into local time
        {
            return dt.AddSeconds(GetOffsetSecond_FromKey());
        }

        public DateTime Convertdate(string date, string time)
        {

            DateTime convertedDate = DateTime.Parse(date);
            DateTime convertedTime = DateTime.Parse(time);
            DateTime dateTime = convertedDate.Date.Add(convertedTime.TimeOfDay);
            return GetTimeSubtractOffSet(dateTime);

        }
        private Int32 GetOffsetSecond_FromKey()
        {
            Int32 timeOffSet = 0;
            if (SiteKeys.UtcOffsetInSecond_API != 0)
            {
                timeOffSet = SiteKeys.UtcOffsetInSecond_API;
            }
            else
            {
                timeOffSet = _httpContextAccessor.HttpContext.Session.GetComplexData<Int32>("UtcOffsetInSecond");
            }
            return timeOffSet;
        }
    }
}
