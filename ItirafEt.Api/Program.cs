//using ItirafEt.Api.Data;
//using ItirafEt.Api.Data.Entities;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

//builder.Services.AddDbContext<Context>(options =>
//{
//    string? connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
//    options.UseSqlServer(connectionString);
//});


//var app = builder.Build();
//#if DEBUG
//ApplyDbMigrations(app.Services);
//#endif

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.Run();

//static void ApplyDbMigrations(IServiceProvider serviceProvider)
//{
//    var scope = serviceProvider.CreateScope();
//    var context = scope.ServiceProvider.GetRequiredService<Context>();

//    if (context.Database.GetPendingMigrations().Any())
//        context.Database.Migrate();
//}

using System.Text;
using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using ItirafEt.Api.EndPoints;
using ItirafEt.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "ItirafEt API",
//        Version = "v1"
//    });
//});

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<Context>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
    options.UseSqlServer(connectionString);
});

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
        ValidateIssuerSigningKey = true

    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
    {
        var allowedOriginsStr = builder.Configuration.GetValue<string>("AllowedOrigins");
        var allowedOrigins = allowedOriginsStr?.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        p.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
        //p.AllowAnyOrigin()
        //    .AllowAnyHeader()
        //    .AllowAnyMethod();
    });

});

builder.Services.AddTransient<AuthService>();

var app = builder.Build();

#if DEBUG
ApplyDbMigrations(app.Services);
#endif

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.MapAuthEndpoints();

app.Run();






static void ApplyDbMigrations(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<Context>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
