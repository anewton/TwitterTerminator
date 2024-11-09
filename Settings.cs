using Microsoft.Extensions.Configuration;

namespace TwitterTerminator;

public class Settings : ISettings
{
    private readonly IConfiguration _configuration;

    private Settings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static ISettings Create(IConfiguration configuration)
    {
        Settings settings = new(configuration);
        return settings;
    }

    public string SerilogMinimumLevel => _configuration["SerilogOptions:MinimumLevel"];
    public string SerilogConsoleLevel => _configuration["SerilogOptions:ConsoleLevel"];

    public TwitterApiSettings TwitterApiSettings
    {
        get
        {
            TwitterApiSettings twitterApiSettings = new();
            IConfigurationSection config = _configuration.GetSection("AppSettings:TwitterApiSettings");
            config?.Bind(twitterApiSettings);
            return twitterApiSettings;
        }
    }

}
