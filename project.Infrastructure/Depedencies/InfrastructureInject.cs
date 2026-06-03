using Microsoft.AspNetCore.DataProtection;
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
                .AddRepositories()
                .AddAuthentication(config)
                .AddMemoryCache()
                .AddExternalService(config)
                .AddProtection(config);
            return services;
        }
    }
    public static class DataProtectionInject
    {
        public static IServiceCollection AddProtection(this  IServiceCollection services, IConfiguration config)
        {
            var keysPath = config["DataProtection:KeysPath"] ?? "DataProtectionKeys";
            services.AddDataProtection()
             .SetApplicationName("graduation-be")
             .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

            return services;
        }
    }
}
