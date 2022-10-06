using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog.Events;

public class LoggingService
{

  public LoggingService(DiscordSocketClient client, CommandService command)
  {
    client.Log += LogAsync;
    command.Log += LogAsync;
  }
  
  private Task LogAsync(LogMessage message)
  {
    var severity = message.Severity switch
    {
      LogSeverity.Critical => LogEventLevel.Fatal,
      LogSeverity.Error => LogEventLevel.Error,
      LogSeverity.Warning => LogEventLevel.Warning,
      LogSeverity.Info => LogEventLevel.Information,
      LogSeverity.Verbose => LogEventLevel.Verbose,
      LogSeverity.Debug => LogEventLevel.Debug,
      _ => LogEventLevel.Information
    };
    if (message.Exception is CommandException cmdException)
    {
      Serilog.Log.Error($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                        + $" failed to execute in {cmdException.Context.Channel}.");
      Console.WriteLine(cmdException);
    }
    else
    {
      Serilog.Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
    }
    return Task.CompletedTask;
  }
}






