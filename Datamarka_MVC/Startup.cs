using Datamarka_Bll.Services.Sendgrid;
using Datamarka_DAL;
using Datamarka_DomainModel.Models.Identity;
using Datamarka_MVC.ConfigurationSections;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Datamarka_MVC
{
	public static class Startup
	{

		internal static void AddServices(WebApplicationBuilder builder)
		{
			AddSerilog(builder);
			RegisterDAL(builder);
			RegisterIdentity(builder);
			RegisterEmailSenderService(builder);
			CookiesConfig(builder);
		}

		private static void RegisterIdentity(WebApplicationBuilder builder)
		{
			builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddEntityFrameworkStores<DbContext>();

			//builder.Services.AddAuthentication().AddGoogle(googleOptions =>
			//{
			//	googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
			//	googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
			//});
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

		public static void RegisterEmailSenderService(WebApplicationBuilder builder)
		{
			builder.Services.AddSendGrid(options =>
		   {
			   options.ApiKey = builder.Configuration
			   .GetSection("EmailSettings").GetValue<string>("ApiKey");
		   });
			builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailSettings"));
			builder.Services.AddTransient<EmailSender>();
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
		//public static void RegisterUserService(WebApplicationBuilder builder)
		//      {
		//	var provider = builder.Services.BuildServiceProvider();
		//	var logger = provider.GetRequiredService<ILogger>();

		//	if (builder.Environment.IsDevelopment())
		//	{
		//		logger.Information("Register EmployeeService if IsDevelopment ...");
		//		builder.Services.AddScoped<IUserService, UserService>();

		//	}
		//	else
		//	{
		//		logger.Information("Register StubEmployeeService if not IsDevelopment ...");
		//		builder.Services.AddScoped<IEmployeeService, StubEmployeeService>();
		//	}
		//}

	}
}
