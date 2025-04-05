using Chowbro.Core.Entities;
using Chowbro.Infrastructure;
using Chowbro.Infrastructure.Middlewares;
using Chowbro.Infrastructure.Persistence.Seeders;
using Chowbro.Infrastructure.Settings;
using Chowbro.Modules.Accounts;
using Chowbro.Modules.Accounts.Data;
using Chowbro.Modules.Accounts.Data.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using Chowbro.Infrastructure.Persistence;
using Chowbro.Modules.Customers;
using Chowbro.Modules.Riders;
using Chowbro.Modules.Vendors;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICES CONFIGURATION ========== //

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
});

// Rate Limiting Configuration
builder.Services.AddRateLimiter(options =>
{
    // Global limiter as fallback
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        string partitionKey = context.User.Identity?.Name
                           ?? context.Request.Headers["X-Client-Id"].ToString()
                           ?? context.Connection.RemoteIpAddress?.ToString()
                           ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    // Strict auth endpoint policy
    options.AddPolicy("strict-auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            StatusCode = 429,
            Message = "Too many requests. Please try again later."
        }, cancellationToken: token);
    };
});

// Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();

// Email Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Application Modules
builder.Services.AddInfrastructure(builder.Configuration)
               .AddCustomSwagger()
               .AddAccountsModule()
               .AddCustomerModule()
               .AddVendorModule()
               .AddRiderModule();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = jwtSettings["Issuer"],
                       ValidAudience = jwtSettings["Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(key)
                   };
               });

builder.Services.AddAuthorization();

// API Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// ========== APP BUILD & MIDDLEWARE ========== //

var app = builder.Build();

// Database Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seed roles first
        await SeedRolesData.InitializeAsync(services);

        // Then seed super admin
        var superAdminSeeder = services.GetRequiredService<SuperAdminSeeder>();
        await superAdminSeeder.SeedAsync();

        var dbContext = services.GetRequiredService<AppDbContext>();
        await NigeriaLocationSeeder.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// Middleware Pipeline
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

// Swagger Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();