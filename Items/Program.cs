using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Loki;

namespace Items;

public static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("Staring the Host");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host Terminated Unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((ctx,cfg)=>
            {
                cfg
                    .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
                    .WriteTo.Console(new RenderedCompactJsonFormatter());

                var lokiApiUrl = ctx.Configuration["LokiApi"];
                if (lokiApiUrl is null)
                    return;

                cfg.WriteTo.LokiHttp(lokiApiUrl);
                Log.Information("Connected to Loki");
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
