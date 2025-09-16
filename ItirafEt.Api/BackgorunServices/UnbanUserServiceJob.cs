using ItirafEt.Api.BackgorunServices.RabbitMQ;
using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.Data;
using ItirafEt.Api.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ItirafEt.Api.BackgorunServices
{
    //public class UnbanUserServiceJob : IJob
    //{
    //    private readonly dbContext _dbContext;
    //    private readonly EmailSenderProducer _emailSenderProducer;

    //    public UnbanUserServiceJob(dbContext context, EmailSenderProducer emailSenderProducer)
    //    {
    //        _dbContext = context;
    //        _emailSenderProducer = emailSenderProducer;
    //    }

    //    public async Task Execute(IJobExecutionContext context)
    //    {
    //        var users = await _dbContext.Users
    //                        .Where(u => u.IsBanned && u.BannedDateUntil != null && u.BannedDateUntil <= DateTime.UtcNow)
    //                        .ToListAsync();

    //        if (!users.Any())
    //            return;

    //        foreach (var user in users)
    //        {
    //            user.IsBanned = false;
    //            user.BannedDate = null;
    //            user.BannedDateUntil = null;
    //            _dbContext.Users.Update(user);
    //            await _emailSenderProducer.PublishAsync(EmailTypes.Ban, EmailCreateFactory.CreateEmail(EmailTypes.Ban, user));
    //        }
    //        await _dbContext.SaveChangesAsync();
    //    }
    //}


    public class UnbanUserServiceJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public UnbanUserServiceJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<dbContext>();
            var emailProducer = scope.ServiceProvider.GetRequiredService<EmailSenderProducer>();

            var users = await dbContext.Users
                            .Where(u => u.IsBanned && u.BannedDateUntil != null && u.BannedDateUntil <= DateTime.UtcNow)
                            .ToListAsync();

            if (!users.Any())
                return;

            foreach (var user in users)
            {
                user.IsBanned = false;
                user.BannedDate = null;
                user.BannedDateUntil = null;
                dbContext.Users.Update(user);
                await emailProducer.PublishAsync(EmailTypes.Ban, EmailCreateFactory.CreateEmail(EmailTypes.Ban, user));
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
