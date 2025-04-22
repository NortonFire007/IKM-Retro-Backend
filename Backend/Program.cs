using System.Text;
using IKM_Retro.Data;
using IKM_Retro.Extensions;
using IKM_Retro.Models;
using IKM_Retro.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using Mapster;
using IKM_Retro.Services;
using IKM_Retro.DTOs.Auth;
using IKM_Retro.DTOs.Mappings;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var services = builder.Services;
var config = builder.Configuration;

services.AddDbContext<RetroDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<JwtOptions>(config.GetSection("JwtOptions"));


services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

services.AddMapster();
MapsterConfig.RegisterMappings();

services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RetroDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var secret = config["JwtOptions:SecretKey"] ?? Environment.GetEnvironmentVariable("Jwt__SecretKey");
    var issuer = config["JwtOptions:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer");
    var audience = config["JwtOptions:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience");
    if (secret is null || issuer is null || audience is null)
    {
        throw new ApplicationException("Jwt is not set in the configuration");
    }

    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies[config["JwtOptions:CookieName"]];
            return Task.CompletedTask;
        }
    };
});



services.AddScoped<RefreshTokenRepository>();
services.AddScoped<RetrospectiveRepository>();
services.AddScoped<RetrospectiveGroupRepository>();
services.AddScoped<RetrospectiveGroupItemRepository>();
services.AddScoped<InviteRepository>();

services.AddScoped<RetrospectiveService>();
services.AddScoped<AccountService>();
services.AddScoped<InviteService>();

// services.AddScoped<IBoardRoleRepository, BoardRoleRepository>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseExceptionHandler();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();