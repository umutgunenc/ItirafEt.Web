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

using ItirafEt.Api.Data;
using ItirafEt.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ItirafEt API",
        Version = "v1"
    });
});

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddDbContext<Context>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

#if DEBUG
ApplyDbMigrations(app.Services);
#endif

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ItirafEt API v1");
    });
}

app.UseHttpsRedirection();

app.Run();

static void ApplyDbMigrations(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<Context>();

    if (context.Database.GetPendingMigrations().Any())
        context.Database.Migrate();
}
