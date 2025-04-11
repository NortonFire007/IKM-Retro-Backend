using System.Text;
using IKM_Retro.Data;
using IKM_Retro.Extensions;
using IKM_Retro.Models;
using IKM_Retro.Repositories;
using IKM_Retro.Repositories.Interfaces;
using IKM_Retro.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddDbContext<RetroDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

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
    var secret = builder.Configuration["Jwt:Secret"] ?? Environment.GetEnvironmentVariable("Jwt__Secret");
    var issuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer");
    var audience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience");
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
});

services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

services.AddScoped<IAccountService, IAccountService>();

// services.AddScoped<IBoardRoleRepository, BoardRoleRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();