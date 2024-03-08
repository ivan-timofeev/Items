using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Services;
using Items.Commands;
using Items.Commands.Factories;
using Items.Controllers;
using Items.Data;
using Items.Helpers;
using Items.Models.DataTransferObjects;
using Items.Models.Exceptions;
using Items.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
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

        services.AddQueries();

        services.AddTransient<ICreateOrderCommandHandlerFactory, CreateOrderCommandHandlerFactory>();

        services.AddSingleton<ICacheService, CacheService>();
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddTransient<ICommandsFactory, CommandsFactory>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
    }
 
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

        app.UseResponseCaching();

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

                if (error is BusinessException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }

                await context.Response.WriteAsJsonAsync(
                    new ErrorDto
                    {
                        ErrorMessage = error.Message,
                        Data = error.Data
                    });
            });
        });

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            databaseInitializer.InitializeDatabase();
        }

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
