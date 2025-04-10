using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ItirafEt.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReactionType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    IsTermOfUse = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LastMessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessagePreview = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsDeletedByUser1 = table.Column<bool>(type: "bit", nullable: false),
                    IsDeletedByUser2 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.CheckConstraint("CK_Conversation_DifferentUsers", "InitiatorId != ResponderId");
                    table.ForeignKey(
                        name: "FK_Conversations_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_ResponderId",
                        column: x => x.ResponderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_BlockedUserId",
                        column: x => x.BlockedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlocks_Users_BlockerUserId",
                        column: x => x.BlockerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsVisibleToInitiatorUser = table.Column<bool>(type: "bit", nullable: false),
                    IsVisibleToResponderUser = table.Column<bool>(type: "bit", nullable: false),
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostHistories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostHistories_Users_OperationByUserId",
                        column: x => x.OperationByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostReaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReactingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    ReactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: true),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReaction_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReaction_ReactionType_ReactionTypeId",
                        column: x => x.ReactionTypeId,
                        principalTable: "ReactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReaction_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PostReaction_Users_ReactingUserId",
                        column: x => x.ReactingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAttachment_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReactingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ReactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: true),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReactions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactions_ReactionType_ReactionTypeId",
                        column: x => x.ReactionTypeId,
                        principalTable: "ReactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactions_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReactions_Users_ReactingUserId",
                        column: x => x.ReactingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCommentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentHistories_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentHistories_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentHistories_Users_OperationByUserId",
                        column: x => x.OperationByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentReactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReactingUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    ReactionTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: true),
                    ReportExplanation = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_ReactionType_ReactionTypeId",
                        column: x => x.ReactionTypeId,
                        principalTable: "ReactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_ReportType_ReportTypeId",
                        column: x => x.ReportTypeId,
                        principalTable: "ReportType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommentReactions_Users_ReactingUserId",
                        column: x => x.ReactingUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Unknown" },
                    { 2, "Male" },
                    { 3, "Female" },
                    { 4, "Other" }
                });

            migrationBuilder.InsertData(
                table: "ReactionType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Like" },
                    { 2, "Dislike" },
                    { 3, "Report" }
                });

            migrationBuilder.InsertData(
                table: "ReportType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Spam" },
                    { 2, "Harassment" },
                    { 3, "AbusiveLanguage" },
                    { 4, "OffTopic" },
                    { 5, "InappropriateContent" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" },
                    { 3, "Moderator" },
                    { 4, "SuperUser" },
                    { 5, "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedDate", "Email", "GenderId", "IsActive", "IsPremium", "IsTermOfUse", "PasswordHash", "ProfilePictureUrl", "RoleId", "UserName" },
                values: new object[] { new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"), new DateTime(1989, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "umutgunenc@gmail.com", 2, true, true, true, "AQAAAAIAAYagAAAAEMMH/7QHaOQE4G24K3VWQp1WjQaa1UvQQu6ZlNIj6JTHj2cDEW1u0uvkX8Fq5ZgSwQ==", null, 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistories_CommentId",
                table: "CommentHistories",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistories_OperationByUserId",
                table: "CommentHistories",
                column: "OperationByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistories_ParentCommentId",
                table: "CommentHistories",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId",
                table: "CommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactingUserId",
                table: "CommentReactions",
                column: "ReactingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactionTypeId",
                table: "CommentReactions",
                column: "ReactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReportTypeId",
                table: "CommentReactions",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_InitiatorId",
                table: "Conversations",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ResponderId",
                table: "Conversations",
                column: "ResponderId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageAttachment_MessageId",
                table: "MessageAttachment",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_MessageId",
                table: "MessageReactions",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReactingUserId",
                table: "MessageReactions",
                column: "ReactingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReactionTypeId",
                table: "MessageReactions",
                column: "ReactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_ReportTypeId",
                table: "MessageReactions",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PostHistories_OperationByUserId",
                table: "PostHistories",
                column: "OperationByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostHistories_PostId",
                table: "PostHistories",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_PostId",
                table: "PostReaction",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReactingUserId",
                table: "PostReaction",
                column: "ReactingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReactionTypeId",
                table: "PostReaction",
                column: "ReactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReaction_ReportTypeId",
                table: "PostReaction",
                column: "ReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedUserId",
                table: "UserBlocks",
                column: "BlockedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockerUserId",
                table: "UserBlocks",
                column: "BlockerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderId",
                table: "Users",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentHistories");

            migrationBuilder.DropTable(
                name: "CommentReactions");

            migrationBuilder.DropTable(
                name: "MessageAttachment");

            migrationBuilder.DropTable(
                name: "MessageReactions");

            migrationBuilder.DropTable(
                name: "PostHistories");

            migrationBuilder.DropTable(
                name: "PostReaction");

            migrationBuilder.DropTable(
                name: "UserBlocks");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ReactionType");

            migrationBuilder.DropTable(
                name: "ReportType");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
