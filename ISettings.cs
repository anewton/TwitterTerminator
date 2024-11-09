namespace TwitterTerminator;

public interface ISettings
{
    string SerilogMinimumLevel { get; }
    string SerilogConsoleLevel { get; }
    TwitterApiSettings TwitterApiSettings { get; }
}