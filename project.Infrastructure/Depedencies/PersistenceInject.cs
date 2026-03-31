using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using project.Application.Interfaces;
using project.Domain.Interfaces;
using project.Domain.Models;
using project.Infrastructure.Database;
using project.Infrastructure.Interfaces;
using project.Infrastructure.Repositories;
using project.Infrastructure.Security;
using project.Infrastructure.Services.Email;
using project.Infrastructure.Services.ExcelImport;
using project.Infrastructure.Services.Gemini;
using project.Infrastructure.Services.GithubService;
using project.Infrastructure.Services.Photo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Depedencies
{
    public static class PersistenceInject
    {

        public static IServiceCollection AddConnectionDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });

            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWorkTaskRepository, WorkTaskRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>(); 
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILabelRepository, LabelRepository>();


            // Service
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            return services;
        }

    }
    public static class EmailInject
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailConfiguration>(config.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
    public static class PhotoServiceInject
    {
        public static IServiceCollection AddPhotoService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IPhotoService, PhotoService>();
            return services;
        }
    }
    public static class ExcelImport
    {
        public static IServiceCollection AddExcelImport(this IServiceCollection services)
        {
            services.AddScoped<IExcelImportService, ExcelImportService>();
            return services;
        }
    }
    
    public static class AuthenticationInject
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("Jwt");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
                {
                    opt.MapInboundClaims = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = "role",
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
                    };
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
                
            return services;
        }
    }
    public static class ExternalServiceInject
    {
        public static IServiceCollection AddGithubService(this IServiceCollection services)
        {
            services.AddHttpClient<IGithubOAuthService, GithubOauthService>();
            services.AddHttpClient<IGithubService, GithubService>();
            return services;
        }
        public static IServiceCollection AddGeminiService(this IServiceCollection services)
        {
            services.AddHttpClient<IGeminiService, GeminiService>();
            return services;
        }
    }
}
