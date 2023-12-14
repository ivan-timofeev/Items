using Microsoft.EntityFrameworkCore;
using Items.BackgroundServices;
using Items.Data;
using Items.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["ItemsSqlConnectionString"];
builder.Services.AddDbContextFactory<ItemsDbContext>(
    options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IItemsRepository, ItemsRepository>();
builder.Services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddSingleton<IOrdersMicroserviceApiClient, OrdersMicroserviceApiClient>();
builder.Services.AddTransient<IReserveItemsRequestProcessor, ReserveItemsRequestProcessor>();
builder.Services.AddHostedService<ReserveItemsRequestProcessingBackgroundService>();

var app = builder.Build();

// Custom Metrics to count requests for each endpoint and the method
var counter = Metrics.CreateCounter(
    name: "orders_api_path_counter",
    help: "Counts requests to the People API endpoints",
    new CounterConfiguration
    {
        LabelNames = new[] { "method", "endpoint" }
    });
app.Use((context, next) =>
{
    counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
    return next();
});

// Use the Prometheus middleware
app.UseMetricServer();
app.UseHttpMetrics();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();