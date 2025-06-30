using Quartz;

namespace ItirafEt.Api.BackgorunServices
{
    public static class BackgorundService 
    {
        public static async Task ScheduleJobs(IServiceProvider services)
        {
            var schedulerFactory = services.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler();


            await scheduler.Start();

            var jobUnbanUser = JobBuilder.Create<UnbanUserServiceJob>().Build();

            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 00, 00, 05);

            var unbanUser = TriggerBuilder.Create()
                .StartAt(scheduledTime.ToLocalTime())
                //.StartNow()
                .WithSimpleSchedule(x => x
                    //.WithIntervalInMinutes(1)
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(jobUnbanUser, unbanUser);

        }
    }
}
