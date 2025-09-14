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

            var jobUnbanUser = JobBuilder.Create<UnbanUserServiceJob>()
                .Build();

            var trigger = TriggerBuilder.Create()
                .StartNow() // publish edildiğinde hemen çalışır
                .WithCronSchedule("25 0 0 * * ?", x => x.InTimeZone(TimeZoneInfo.Utc)) // her gün 00:00:25 (server local time)
                .Build();


            //var jobUnbanUser = JobBuilder.Create<UnbanUserServiceJob>().Build();

            //DateTime scheduledDate = DateTime.UtcNow.AddDays(1);
            //DateTime scheduledTime = new DateTime(scheduledDate.Year, scheduledDate.Month, scheduledDate.Day, 00, 00, 25);

            //var trigger = TriggerBuilder.Create()
            //    .StartAt(scheduledTime)
            //    //.StartNow()
            //    .WithSimpleSchedule(x => x
            //        //.WithIntervalInMinutes(1)
            //        .WithIntervalInHours(24)
            //        .RepeatForever())
            //    .Build();

            //var trigger = TriggerBuilder.Create()
            //    .StartNow() // hemen başlat
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1) // her 1 dakikada bir
            //        .RepeatForever())          // sonsuz tekrar
            //    .Build();

            await scheduler.ScheduleJob(jobUnbanUser, trigger);

        }
    }
}
