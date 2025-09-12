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

            DateTime scheduledDate = DateTime.UtcNow.AddDays(1);
            DateTime scheduledTime = new DateTime(scheduledDate.Year, scheduledDate.Month, scheduledDate.Day, 00, 00, 05);

            var unbanUserTrigger = TriggerBuilder.Create()
                .StartAt(scheduledTime)
                //.StartNow()
                .WithSimpleSchedule(x => x
                    //.WithIntervalInMinutes(1)
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            //var unbanUserTrigger = TriggerBuilder.Create()
            //    .StartNow() // hemen başlat
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(1) // her 1 dakikada bir
            //        .RepeatForever())          // sonsuz tekrar
            //    .Build();

            await scheduler.ScheduleJob(jobUnbanUser, unbanUserTrigger);

        }
    }
}
