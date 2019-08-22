using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.Postgre
{
    public static class Extensions
    {
        public static IServiceCollection AddPostgre<T>(this IServiceCollection services)
             where T : DbContext
        {
            PostgreOptions options = null;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<PostgreOptions>(configuration.GetSection("postgre"));
                options = configuration.GetOptions<PostgreOptions>("postgre");
            }

            if (!options.Enabled)
            {
                return services;
            }

            return services.AddEntityFrameworkNpgsql()
                    .AddDbContext<T>(op => op.UseNpgsql(options.ConnectionString)); ;
        }
    }
}
