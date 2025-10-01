using System.Security.Authentication;
using System.Security.Cryptography;
using ItirafEt.Api.ConstStrings;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data
{
    public class dbContext : DbContext
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public dbContext(DbContextOptions<dbContext> options, IPasswordHasher<User> passwordHasher) : base(options)
        {
            _passwordHasher = passwordHasher;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<RoleType> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<CommentHistory> CommentHistories { get; set; }
        public DbSet<GenderType> Genders { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostHistory> PostHistories { get; set; }
        public DbSet<PostReaction> PostReaction { get; set; }
        public DbSet<PostReport> PostReports { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<MessageReport> MessageReports { get; set; }
        public DbSet<UserReadPost> UserReadPosts { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<ActivateAccountToken> ActivateAccountTokens { get; set; }
        public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.BlockerUser)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(ub => ub.BlockerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserBlock>()
                .HasOne(ub => ub.BlockedUser)
                .WithMany(u => u.BlockedByUsers)
                .HasForeignKey(ub => ub.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Initiator)
                .WithMany(u => u.ConversationsInitiated)
                .HasForeignKey(c => c.InitiatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Responder)
                .WithMany(u => u.ConversationsReceived)
                .HasForeignKey(c => c.ResponderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .ToTable(t => t.HasCheckConstraint("CK_Conversation_DifferentUsers", "InitiatorId != ResponderId"));

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .Property(m => m.SentDate)
                .HasPrecision(7);

            modelBuilder.Entity<PostHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PostHistories)
                .HasForeignKey(ph => ph.OperationByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentHistory>()
                .HasOne(ch => ch.User)
                .WithMany(u => u.CommentHistories)
                .HasForeignKey(ch => ch.OperationByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostReaction>()
                .HasOne(pr => pr.ReactingUser)
                .WithMany(u => u.PostReactions)
                .HasForeignKey(pr => pr.ReactingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(cr => cr.ReactingUser)
                .WithMany(u => u.CommentReactions)
                .HasForeignKey(cr => cr.ReactingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.ReactingUser)
                .WithMany(u => u.MessageReactions)
                .HasForeignKey(mr => mr.ReactingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostReport>()
                .HasOne(pr => pr.ReportingUser)
                .WithMany(u => u.PostReports)
                .HasForeignKey(pr => pr.ReportingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentReport>()
                .HasOne(cr => cr.ReportingUser)
                .WithMany(u => u.CommentReports)
                .HasForeignKey(cr => cr.ReportingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessageReport>()
                .HasOne(mr => mr.ReportingUser)
                .WithMany(u => u.MessageReports)
                .HasForeignKey(mr => mr.ReportingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryOrder)
                .IsUnique();

            modelBuilder.Entity<ReactionType>().HasData(
                Enum.GetValues(typeof(ReactionTypeEnum))
                    .Cast<ReactionTypeEnum>()
                    .Select(e => new ReactionType
                    {
                        Id = (int)e,
                        Name = e.ToString()
                    })
            );

            modelBuilder.Entity<GenderType>().HasData(
               GenderType.List()
                   .Select(gt => new { gt.Id, gt.Name })
           );

            modelBuilder.Entity<RoleType>().HasData(
               RoleType.List()
                   .Select(rt => new { rt.Name })
           );



            modelBuilder.Entity<UserReadPost>(entity =>
            {
                entity.HasKey(e => new
                {
                    e.UserId,
                    e.PostId
                });

                entity.HasOne(e => e.User)
                      .WithMany(u => u.ReadPosts)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Post)
                          .WithMany(p => p.Readers)
                          .HasForeignKey(e => e.PostId)
                          .OnDelete(DeleteBehavior.Restrict);
            });

            var adminUser = new User
            {
                Id = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                UserName = "admin",
                Email = "umutgunenc@gmail.com",
                GenderId = (int)GenderEnum.Male,
                PasswordHash = "AQAAAAIAAYagAAAAED2ATLdR3ZV/GbRKgMX8CnBTh6eN737Th/HT2GGvgGOLlzEQ50HcF8GT5XPRB2scoA==",
                BirthDate = new DateTime(1989, 5, 29),
                CreatedDate = new DateTime(2025, 4, 10),
                IsDeleted = false,
                IsBanned = false,
                IsProfilePrivate = false,
                IsTermOfUse = true
            };

            //adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "123456");

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleName, ur.AssignedDate });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.Roles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleName)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ur => ur.AssignedByUser)
                      .WithMany() // veya .WithMany(u => u.AssignedRoles) dersen User içinde navigation olur
                      .HasForeignKey(ur => ur.AssignedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            var systemUser = new User
            {
                Id = SystemUser.systemUserId,
                UserName = "system",
                Email = "system@yourapp.local",
                GenderId = (int)GenderEnum.Unknown,
                PasswordHash = "SYSTEMUSER_NO_PASSWORD",
                BirthDate = new DateTime(2000, 1, 1),
                CreatedDate = new DateTime(2000, 1, 1),
                IsDeleted = false,
                IsBanned = false,
                IsProfilePrivate = true,
                IsTermOfUse = true
            };



            var roleForeAdmin = new UserRoles()
            {
                UserId = adminUser.Id,
                RoleName = RoleType.SuperAdmin.Name,
                AssignedByUserId = adminUser.Id,
                AssignedDate = adminUser.CreatedDate,
                ExpireDate = null,
                RevokedDate = null
            };

            var roleForSystemUser = new UserRoles()
            {
                UserId = systemUser.Id,
                RoleName = RoleType.SuperAdmin.Name,
                AssignedByUserId = systemUser.Id,
                AssignedDate = systemUser.CreatedDate,
                ExpireDate = null,
                RevokedDate = null
            };



            modelBuilder.Entity<User>().HasData(systemUser);
            modelBuilder.Entity<User>().HasData(adminUser);
            modelBuilder.Entity<UserRoles>().HasData(roleForSystemUser);
            modelBuilder.Entity<UserRoles>().HasData(roleForeAdmin);
        }
    }
}
