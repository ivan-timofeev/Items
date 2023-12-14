using Microsoft.EntityFrameworkCore;
using Items.BackgroundServices;
using Items.Data;
using Items.Services;

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
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();