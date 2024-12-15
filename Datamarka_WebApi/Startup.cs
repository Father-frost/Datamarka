using Datamarka_DAL;
using Datamarka_WebApi.ConfigurationSections;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Datamarka_WebApi
{
    public static class Startup
    {

        internal static void AddServices(WebApplicationBuilder builder)
        {
            AddSerilog(builder);
            RegisterDAL(builder);
            RegisterIdentity(builder);
            //RegisterEmailSenderService(builder);
            CookiesConfig(builder);
        }

        private static void RegisterIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        }

        public static void RegisterDAL(WebApplicationBuilder builder)
        {
            var services = builder.Services;

            var connectionString = builder.Configuration.GetConnectionString("Default");
            services.AddTransient<DbContextOptions<DatamarkaDbContext>>(provider =>
            {
                var builder = new DbContextOptionsBuilder<DatamarkaDbContext>();
                builder.UseNpgsql(connectionString);
                return builder.Options;
            });

            services.AddScoped<DbContext, DatamarkaDbContext>();

			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

			services.AddScoped<IUnitOfWork>(prov =>
            {
                var context = prov.GetRequiredService<DbContext>();
                return new UnitOfWork(context);
            });
        }

        internal static void AddSerilog(WebApplicationBuilder builder)
        {
            var serilogConfig = builder.Configuration.GetSection(nameof(SerilogConfig)).Get<SerilogConfig>();
            var logFilePath = Path.Combine(serilogConfig?.LoggingDir ?? "./", "log.txt");

            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Month,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

            if (builder.Environment.IsDevelopment())
            {
                loggerConfig = loggerConfig.MinimumLevel.Debug();
            }
            else
            {
                loggerConfig = loggerConfig.MinimumLevel.Warning();
            }

            var logger = loggerConfig.CreateLogger();

            builder.Services.AddSingleton<ILogger>(logger);
        }

        public static void CookiesConfig(WebApplicationBuilder builder)
        {
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings  
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                //options.LoginPath = "/Login";     //set the login path.  
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
        }

    }
}
