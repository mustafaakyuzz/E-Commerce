using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniECommerce.Gateway.YARP.Context;
using MiniECommerce.Gateway.YARP.Dtos;
using MiniECommerce.Gateway.YARP.Models;
using MiniECommerce.Gateway.YARP.Repositories;
using MiniECommerce.Gateway.YARP.Services;
using System.Text;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));
});

// YARP Reverse Proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// JWT Authentication
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value ?? "")),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

// DI Registrations
builder.Services.AddScoped<JwtProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Unit Of Work Registeration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

// Auto Migration
using (var scope = app.Services.CreateScope())
{
    var srv = scope.ServiceProvider;
    var context = srv.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
