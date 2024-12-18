
using Datamarka_BLL;
using Datamarka_WebApi.ConfigurationSections;
using Datamarka_WebApi.RequestFilters;

namespace Datamarka_WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers()                
				// do not change field names Case!
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);


			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddTransient<ExceptionLoggingHandler, ExceptionLoggingHandler>();
			builder.Services.AddExceptionHandler<ExceptionLoggingHandler>();

			Startup.AddServices(builder);

			Datamarka_BLL_ModuleHead.RegisterModule(builder.Services);

			var app = builder.Build();

			DBInitializer.InitializeDB(app.Services);

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			// seems to be a bug, not calling the /Error endpoint
			// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-8.0#iexceptionhandler
			app.UseExceptionHandler("/Error");

			app.MapControllers();

			if (app.Environment.IsDevelopment())
			{
				app.UseCors(CorsConfigurer.RelaxedCorsPolicyName);
			}
			else
			{
				// todo add another policy when ready for production
				app.UseCors(CorsConfigurer.RelaxedCorsPolicyName);
			}

			app.Run();
		}
	}
}
