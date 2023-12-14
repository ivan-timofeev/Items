using Items.BackgroundServices;
using Items.Data;
using Items.Services;
using Microsoft.EntityFrameworkCore;
using Prometheus;

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
        services.AddSwaggerGen();

        var connectionString = _configuration["ItemsSqlConnectionString"];
        services.AddDbContextFactory<ItemsDbContext>(
            options => options.UseSqlServer(connectionString));

        services.AddTransient<IItemsRepository, ItemsRepository>();
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddSingleton<IOrdersMicroserviceApiClient, OrdersMicroserviceApiClient>();
        services.AddTransient<IReserveItemsRequestProcessor, ReserveItemsRequestProcessor>();
        services.AddHostedService<ReserveItemsRequestProcessingBackgroundService>();
    }
 
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMetricServer();
        app.UseHttpMetrics();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseEndpoints(e => { e.MapControllers(); });
    }
}
