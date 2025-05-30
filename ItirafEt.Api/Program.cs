using System.Text;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.EndPoints;
using ItirafEt.Api.Hubs;
using ItirafEt.Api.HubServices;
using ItirafEt.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<dbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(options =>
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

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOriginsStr = builder.Configuration.GetValue<string>("AllowedOrigins");
        var allowedOrigins = allowedOriginsStr?.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        //policy.AllowAnyOrigin()
        //    .AllowAnyHeader()
        //    .AllowAnyMethod();
    });

});

builder.Services.AddAuthorization();

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<CategoryService>();
builder.Services.AddTransient<BanUserService>();
builder.Services.AddTransient<PostService>();
builder.Services.AddTransient<CommentService>();
builder.Services.AddTransient<ReactionService>();
builder.Services.AddTransient<PostViewService>();
builder.Services.AddTransient<MessageService>();


builder.Services.AddTransient<ReactionHubService>();
builder.Services.AddTransient<CategoryHubService>();
builder.Services.AddTransient<CommentHubService>();
builder.Services.AddTransient<PostViewHubService>();
builder.Services.AddTransient<MessageHubService>();


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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<CategoryHub>("/categoryhub");
//});
app.MapHub<CategoryHub>("/categoryhub");
app.MapHub<ReactionHub>("/reactionhub");
app.MapHub<CommentHub>("/commenthub");
app.MapHub<PostViewHub>("/postviewhub");
app.MapHub<MessageHub>("/messagehub");

app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapBanUserEndPoints();
app.MapPostEndPoints();
app.MapCommentEndpoints();
app.MapReactionEndpoints();
app.MapPostViewEndpoints();
app.MapMessageEndpoints();

app.Run();

static void ApplyDbMigrations(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<dbContext>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
