using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Npgsql.NameTranslation;

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

        public static void ApplySnakeCase(this ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            var types = modelBuilder.Model.GetEntityTypes().ToList();

            // Refer to tables in snake_case internally
            types.ForEach(e => e.Relational().TableName = mapper.TranslateMemberName(e.Relational().TableName));

            // Refer to columns in snake_case internally
            types.SelectMany(e => e.GetProperties())
                .ToList()
                .ForEach(p => p.Relational().ColumnName = mapper.TranslateMemberName(p.Relational().ColumnName));
        }
    }
}
