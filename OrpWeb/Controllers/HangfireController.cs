using Business.IServices;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace OrpWeb.Controllers
{
    public class HangFireController : Controller
    {
        public IActionResult Index()
        {
            return View();
        } 
        public class ScheduleReminder
        {
            private readonly IHangFireService _hangFireService;
            public ScheduleReminder(IHangFireService hangFireService)
            {
                _hangFireService = hangFireService;
            }
            public async Task Run(IJobCancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                await RunAtTimeOf();
            }
            
            public async Task RunAtTimeOf()
            {
                await _hangFireService.ScheduleTaskReminder();
            }                  
        }

        public class NotAnyTaskCompleteReminder
        {
            private readonly IHangFireService _hangFireService;
            public NotAnyTaskCompleteReminder(IHangFireService hangFireService)
            {
                _hangFireService = hangFireService;
            }
            public async Task Run(IJobCancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                await RunAtTimeOf();
            }

            public async Task RunAtTimeOf()
            {
                await _hangFireService.NotAnyTaskCompleteReminder();
            }
        }
        public class UpdateUserStreak
        {
            private readonly IHangFireService _hangFireService;
            public UpdateUserStreak(IHangFireService hangFireService)
            {
                _hangFireService = hangFireService;
            }
            public async Task Run(IJobCancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                await RunAtTimeOf();
            }

            public async Task RunAtTimeOf()
            {
                await _hangFireService.UpdateUserStreak();
            }
        }
    }
}



