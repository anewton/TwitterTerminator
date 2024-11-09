using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace TwitterTerminator;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args).UseConsoleLifetime();

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.json");
        });

        builder.ConfigureLogging((context, logging) =>
        {
            logging.AddConsole();
            logging.AddDebug();
            logging.AddSerilog();
        });

        builder.ConfigureServices((context, services) =>
        {
            ISettings settings = Settings.Create(context.Configuration);
            ConfigureSerilog(settings);
            services.AddSingleton(settings);
            services.AddScoped<AppRunner, AppRunner>();
        });

        using IHost host = builder.Build();
        await host.StartAsync();
        IHostApplicationLifetime lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;

         // Application logic
        AppRunner appRunner = serviceProvider.GetService<AppRunner>();
        await appRunner.RunAllAsync();

        Log.CloseAndFlush();
        lifetime.StopApplication();
        await host.WaitForShutdownAsync();
    }

    private static void ConfigureSerilog(ISettings settings)
    {
        string minimumLevel = settings.SerilogMinimumLevel is null ? LogEventLevel.Information.ToString() : settings.SerilogMinimumLevel;
        string consoleLevel = settings.SerilogConsoleLevel is null ? LogEventLevel.Information.ToString() : settings.SerilogConsoleLevel;

        LogEventLevel minimumSerilogLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), minimumLevel);
        LogEventLevel consoleConsoleLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), consoleLevel);

        string debuggingOutputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3} {Environment}] {Message:lj} {NewLine}{Exception}";

        LoggerConfiguration logger = new LoggerConfiguration()
            .WriteTo.Debug(
                restrictedToMinimumLevel: minimumSerilogLevel,
                outputTemplate: debuggingOutputTemplate)
            .WriteTo.Console(
                restrictedToMinimumLevel: consoleConsoleLevel,
                outputTemplate: debuggingOutputTemplate)
            .MinimumLevel.Is(minimumSerilogLevel)
            .Enrich.FromLogContext();

        Log.Logger = logger.CreateLogger();

        //Serilog.Debugging.SelfLog.Enable(msg =>
        //{
        //    System.Diagnostics.Debug.Print(msg);
        //    System.Diagnostics.Debugger.Break();
        //});
    }
}
