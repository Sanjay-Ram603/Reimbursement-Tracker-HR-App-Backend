using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

using ReimbursementSystem.Repositories.Implementations;
using ReimbursementTrackerApp.Contexts;
using ReimbursementTrackerApp.Models.Identity;
using ReimbursementTrackerApp.Repositories.Implementations;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;
using ReimbursementTrackerApp.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ReimbursementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IReimbursementRequestRepository, ReimbursementRequestRepository>();
builder.Services.AddScoped<IExpenseCategoryRepository, ExpenseCategoryRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IReimbursementRequestRepository, ReimbursementRequestRepository>();


builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IReimbursementService, ReimbursementService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<INotificationService, NotificationService>();



var jwtSection = builder.Configuration.GetSection("JwtSettings");

var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");
var secretKey = jwtSection.GetValue<string>("SecretKey");

if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("JWT SecretKey is missing in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo
{
Title = "BusTicketBooking API",
Version = "v1",
Description = "Web API for Bus Ticket Booking (Auth + Roles + Generic Repository)"
});

// JWT security definition for Swagger "Authorize" button
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
Name = "Authorization",
Type = SecuritySchemeType.ApiKey,
Scheme = "Bearer",
BearerFormat = "JWT",
In = ParameterLocation.Header,
Description = "Enter 'Bearer' followed by your token. Example: Bearer abc123"
});

c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

{


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ReimbursementTrackerApp.Middleware.GlobalExceptionMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();



    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ReimbursementDbContext>();

        context.Database.EnsureCreated();

        if (!context.Roles.Any())
        {
            var adminRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var employeeRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = "Employee",
                CreatedAt = DateTime.UtcNow
            };

            context.Roles.AddRange(adminRole, employeeRole);
            context.SaveChanges();
        }

        if (!context.Users.Any(u => u.Email == "admin@company.com"))
        {
            var adminRole = context.Roles.First(r => r.RoleName == "Admin");

            var adminUser = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                RoleId = adminRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }




    app.Run();

}
