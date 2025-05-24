using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using MySql.Data.MySqlClient;
using Test.Shop.Core.Repositories.Abstractions;
using Test.Shop.Infrastructure.DAL.Configuration;
using Test.Shop.Infrastructure.DAL.Repositories;

namespace Test.Shop.Infrastructure.DAL
{
    public static class Extensions
    {
        private const string MigrationsEnabledString = "MigrationsEnabled";
        private const string AllowLoadLocalInfile = "AllowLoadLocalInfile";

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("Database");
            services.Configure<DatabaseOptions>(section);
            var options = new DatabaseOptions();
            section.Bind(options);
            var connectionString = PrepareConnectionString(options.ConnectionString);
            services.AddDbContext<ShopDbContext>(x => x.UseMySql(connectionString, ServerVersion.AutoDetect(options.ConnectionString)));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IMappedRepository<>), typeof(MappedRepository<>));

            return services;
        }

        private static string PrepareConnectionString(string connectionString)
        {
            var connectionstringBuiulder = new MySqlConnectionStringBuilder(connectionString)
            {
                [AllowLoadLocalInfile] = true
            };

            return connectionstringBuiulder.ConnectionString;
        }

        public static WebApplication InitializeDatabase(this WebApplication applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.Services.CreateScope())
            {
                var featureManager = serviceScope.ServiceProvider.GetRequiredService<IFeatureManager>();
                var enabled = featureManager.IsEnabledAsync(MigrationsEnabledString).GetAwaiter().GetResult();
                if (enabled)
                {
                    serviceScope.MigrateDatabase();
                }
            }

            return applicationBuilder;
        }

        public static void MigrateDatabase(this IServiceScope serviceScope)
        {
            try
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ShopDbContext>();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
