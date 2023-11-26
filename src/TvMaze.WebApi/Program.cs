using Serilog;

namespace TvMaze.WebApi;

public class Program
{
    public static void Main(string[] args)
   {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        Log.Information("Starting up");

        try
        {
            var builder = CreateWebApplicationBuilder(args);

            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, app, app.Environment);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }

    private static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((hostContext, serilog) => serilog.ReadFrom.Configuration(hostContext.Configuration))
                    .ConfigureAppConfiguration((hostContext, config) => AddConfiguration(hostContext, config, args));

        return builder;
    }

    private static void AddConfiguration(HostBuilderContext _, IConfigurationBuilder config, string[] args)
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        config.AddEnvironmentVariables();

        if (args != null)
        {
            config.AddCommandLine(args);
        }
    }
}
