using IdentityProvider;
using Items.BackgroundServices;
using Items.Data;
using Items.Models.DataTransferObjects;
using Items.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Prometheus;
using static System.Net.Mime.MediaTypeNames;

namespace Items;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddConfiguredSwaggerGen();
        services.AddJwtAuth(_configuration);

        var connectionString = _configuration["ItemsSqlConnectionString"]
            ?? throw new InvalidOperationException("ItemsSqlConnectionString must be specified.");
        services.AddDbContextFactory<ItemsDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddTransient<IItemsRepository, ItemsRepository>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddSingleton<IOrdersMicroserviceApiClient, OrdersMicroserviceApiClient>();
        services.AddTransient<IReserveItemsRequestProcessor, ReserveItemsRequestProcessor>();
        services.AddHostedService<ReserveItemsRequestProcessingBackgroundService>();
    }
 
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = Text.Plain;

                var error = context
                    .Features
                    .GetRequiredFeature<IExceptionHandlerPathFeature>()
                    .Error;

                await context.Response.WriteAsJsonAsync(
                    new ErrorDto
                    {
                        ErrorMessage = error.Message,
                        Details = error.ToString()
                    });
            });
        });

        app.UseMetricServer();
        app.UseHttpMetrics();

        // Available at: http://localhost:<port>/swagger/v1/swagger.json
        app.UseSwagger();
        // Available at: http://localhost:<port>/swagger
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(e => { e.MapControllers(); });
    }
}
