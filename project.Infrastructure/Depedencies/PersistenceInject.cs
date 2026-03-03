using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using project.Infrastructure.Database;
using project.Infrastructure.Interfaces;
using project.Infrastructure.Services.Email;
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

            services.AddDbContext<IdentityDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
            });
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
}
