using System.Text;
using ItirafEt.Api.BackgorunServices;
using ItirafEt.Api.BackgorunServices.RabbitMQ;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.HelperServices;
using ItirafEt.Api.EndPoints;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Api.Services;
using ItirafEt.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<dbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration.GetValue<string>("Jwt:Secret");
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = symmetricKey,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

        };
    });


builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddSignalR();


//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy
//    .AllowAnyOrigin()
//    .AllowAnyHeader()
//    .AllowAnyMethod();
//    });
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://itirafetweb.runasp.net") // Ýstemci adresi
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


builder.Services.AddAuthorization();


builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<RabbitMqConnection>();

// RabbitMQ Producer
//builder.Services.AddSingleton<EmailSenderProducer>();
//builder.Services.AddSingleton<MessageSenderReaderProducer>();



builder.Services.AddSingleton<EmailSenderProducer>(sp =>
{
    var producer = new EmailSenderProducer(sp.GetRequiredService<IConfiguration>(),sp.GetRequiredService<RabbitMqConnection>());
    producer.InitAsync().GetAwaiter().GetResult(); // InitAsync direkt startup'ta çaðýrýlýyor
    return producer;
});

builder.Services.AddSingleton<MessageSenderReaderProducer>(sp =>
{
    var producer = new MessageSenderReaderProducer(sp.GetRequiredService<IConfiguration>(), sp.GetRequiredService<RabbitMqConnection>());
    producer.InitAsync().GetAwaiter().GetResult();
    return producer;
});

// RabbitMQ Consumer
builder.Services.AddHostedService<EmailSenderConsumer>();
builder.Services.AddHostedService<MessageSenderConsumer>();
builder.Services.AddHostedService<MessageReaderConsumer>();

//SignedUrl Service
builder.Services.AddScoped<SignedUrl>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BanUserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ReactionService>();
builder.Services.AddScoped<PostViewService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<UserSettingService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<UserRoleService>();
builder.Services.AddScoped<ReportService>();


builder.Services.AddTransient<ReactionHubService>();
builder.Services.AddTransient<CategoryHubService>();
builder.Services.AddTransient<CommentHubService>();
builder.Services.AddTransient<PostViewHubService>();
builder.Services.AddTransient<MessageHubService>();
builder.Services.AddTransient<BanUserHubService>();




var app = builder.Build();

app.UseStaticFiles();

#if DEBUG
ApplyDbMigrations(app.Services);
#endif

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");
//app.UseCors();


app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204; // No Content
        await context.Response.CompleteAsync();
    }
    else
        await next();
});
app.UseAuthentication();
app.UseAuthorization();



app.MapHub<CategoryHub>(HubConstants.CategoryHub);
app.MapHub<ReactionHub>(HubConstants.ReactionHub);
app.MapHub<CommentHub>(HubConstants.CommentHub);
app.MapHub<PostViewHub>(HubConstants.PostViewHub);
app.MapHub<MessageHub>(HubConstants.MessageHub);
app.MapHub<BanUserHub>(HubConstants.BanUserHub);

app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapBanUserEndPoints();
app.MapPostEndPoints();
app.MapCommentEndpoints();
app.MapReactionEndpoints();
app.MapPostViewEndpoints();
app.MapMessageEndpoints();
app.MapUserSettingsEndpoints();
app.MapUserProfileEndpoints();
app.MapUserRoleEndPoints();
app.MapReportEndpoints();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await BackgorundService.ScheduleJobs(services);
}

    app.Run();


static void ApplyDbMigrations(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<dbContext>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
