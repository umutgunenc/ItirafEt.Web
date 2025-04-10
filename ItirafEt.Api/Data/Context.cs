using System.Security.Authentication;
using System.Security.Cryptography;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItirafEt.Api.Data
{
    public class Context :DbContext
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public Context(DbContextOptions<Context> options, IPasswordHasher<User> passwordHasher) : base(options) 
        {
            _passwordHasher = passwordHasher;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RoleType> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReaction> CommentReactions { get; set; }
        public DbSet<CommentHistory> CommentHistories { get; set; }
        public DbSet<GenderType> Genders { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostHistory> PostHistories { get; set; }
        public DbSet<PostReaction> PostReaction { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User1)
                .WithMany(u => u.ConversationsInitiated)
                .HasForeignKey(c => c.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany(u => u.ConversationsReceived)
                .HasForeignKey(c => c.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .ToTable(t => t.HasCheckConstraint("CK_Conversation_DifferentUsers", "User1Id != User2Id"));

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GenderType>().HasData(
               GenderType.List()
                   .Select(gt => new { gt.Id, gt.Name })
           );

            modelBuilder.Entity<RoleType>().HasData(
               RoleType.List()
                   .Select(rt => new { rt.Id, rt.Name })
           );

            modelBuilder.Entity<ReportType>().HasData(
               ReportType.List()
                   .Select(rt => new { rt.Id, rt.Name })
           ); 
            
           modelBuilder.Entity<ReactionType>().HasData(
               ReactionType.List()
                   .Select(rt => new { rt.Id, rt.Name })
           );



            var adminUser = new User
            {
                Id = new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                UserName = "admin",
                Email = "umutgunenc@gmail.com",
                RoleId = RoleType.FromEnum(UserRole.SuperAdmin).Id,
                GenderId = GenderType.FromEnum(Gender.Male).Id,
                PasswordHash = "AQAAAAIAAYagAAAAEMMH/7QHaOQE4G24K3VWQp1WjQaa1UvQQu6ZlNIj6JTHj2cDEW1u0uvkX8Fq5ZgSwQ==",
                BirthDate = new DateTime(1989, 5, 29),
                CreatedDate = new DateTime(2025, 4, 10),
                IsActive = true,
                IsPremium = true,
                IsTermOfUse = true

            };

            //adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "123456");




            modelBuilder.Entity<User>().HasData(adminUser);
        }
    }
}
