using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Depedencies
{
    public static class InfrastructureInject
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
            {
                services
                    .AddConnectionDatabase(config)
                    .AddEmailService(config)
                    .AddRepositories()
                    .AddExcelImport()
                    .AddPhotoService(config);
                return services;
        }
    }
}
