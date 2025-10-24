using Hangfire;
using static OrpWeb.Controllers.HangFireController;

namespace OrpWeb.Hangfire
{
    public static class HangfireUtility
    {
        public static void ScheduleJobs()
        {
            RecurringJob.AddOrUpdate<ScheduleReminder>(nameof(ScheduleReminder), x => x.Run(JobCancellationToken.Null), "*/4 * * * *");

            // every day 8 pm 
            RecurringJob.AddOrUpdate<NotAnyTaskCompleteReminder>(nameof(NotAnyTaskCompleteReminder), x => x.Run(JobCancellationToken.Null), "0 20 * * *");

            // every day 01:00 am 
            RecurringJob.AddOrUpdate<UpdateUserStreak>(nameof(UpdateUserStreak), x => x.Run(JobCancellationToken.Null), "0 1 * * *");
        }
    }
}
