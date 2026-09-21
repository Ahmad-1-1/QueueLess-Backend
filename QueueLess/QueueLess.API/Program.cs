using Microsoft.EntityFrameworkCore;
using QueueLess.Application.Interfaces;
using QueueLess.Application.Services;
using QueueLess.Infrastructure.Persistence;
using QueueLess.Infrastructure.Persistence.Repositories;
using QueueLess.Infrastructure.Security;
using QueueLess.API.Middleware;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using QueueLess.Infrastructure.Services;
using QueueLess.Infrastructure.Notifications;

namespace QueueLess.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<QueueLessDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injection registration
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOtpRepository, OtpRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.Configure<PictureOptions>(
                builder.Configuration.GetSection(PictureOptions.SectionName));
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddScoped<INotificationStrategy, InAppNotificationStrategy>();
            builder.Services.AddScoped<INotificationStrategy, PushNotificationStrategy>();
            builder.Services.AddScoped<IPictureResolver, PictureResolver>();
            builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();
            builder.Services.AddControllers();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "QueueLess API",
                        Version = "v1"
                    });

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "Input your JWT token in the text box below. " +
                            "Swagger will automatically append 'Bearer '.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
            });

            // JWT configuration
            var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException(
                    "Jwt:SecretKey is not configured.");

            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtIssuer,
                            ValidAudience = jwtAudience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(jwtSecretKey)),

                            ClockSkew = TimeSpan.Zero
                        };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            // Apply pending EF Core migrations and seed initial data.
            using (var scope = app.Services.CreateScope())
            {
                var context =
                    scope.ServiceProvider
                        .GetRequiredService<QueueLessDbContext>();

                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Warning] Database.Migrate skipped: {ex.Message}");
                }

                try
                {
                    DbSeeder.SeedAsync(context)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Warning] DbSeeder skipped: {ex.Message}");
                }
            }

            // Global exception handling
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // HTTPS redirection is currently disabled.
            // app.UseHttpsRedirection();

            app.UseAuthentication();

            // Reject requests with blacklisted tokens.
            app.UseMiddleware<TokenBlacklistMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}