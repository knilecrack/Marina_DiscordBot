using Discord;

namespace MarinaBot.CustomLogger;
public abstract class CustomLogger : ICustomLogger
{
  protected CustomLogger() =>
    // extra data to show individual logger instances
    Guid = System.Guid.NewGuid().ToString()[^4..];

  public string Guid { get; set; }

  public abstract Task Log(LogMessage message);
}

public interface ICustomLogger
{
  // Establish required method for all Loggers to implement
  public Task Log(LogMessage message);
}

public class ConsoleLogger : CustomLogger
{
  // Override Log method from ILogger, passing message to LogToConsoleAsync()
  public override Task Log(LogMessage message)
  {
    // Using Task.Run() in case there are any long running actions, to prevent blocking gateway
    Task.Run(() => LogToConsoleAsync(this, message));
    return Task.CompletedTask;
  }

  private async Task LogToConsoleAsync<T>(T logger, LogMessage message) where T : ICustomLogger
  {
    Console.WriteLine($"guid:{Guid} : " + message);
  }
}
