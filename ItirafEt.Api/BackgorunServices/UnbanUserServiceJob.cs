using ItirafEt.Api.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace ItirafEt.Api.BackgorunServices
{
    public class UnbanUserServiceJob :IJob
    {
        private readonly dbContext _dbContext;

        public UnbanUserServiceJob(dbContext context)
        {
            _dbContext = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var users = await _dbContext.Users
                            .Where(u => u.IsBanned && u.BannedDateUntil != null && u.BannedDateUntil <= DateTime.Now)
                            .ToListAsync();

            if (users.Count == 0)
                return;

            foreach (var user in users)
            {
                user.IsBanned = false;
                user.BannedDate = null;
                user.BannedDateUntil = null;
                _dbContext.Users.Update(user);
            }
            await _dbContext.SaveChangesAsync();
        }


    }
}
