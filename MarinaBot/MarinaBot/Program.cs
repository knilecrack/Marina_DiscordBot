using System.Reactive.PlatformServices;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MarinaBot;
using MarinaBot.BotModules;
using MarinaBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


DiscordSocketClient _client;
CommandService _commands;
IServiceProvider _services;

var confBuilder = new ConfigurationBuilder()
  .AddEnvironmentVariables(prefix:"DISCORD_")
  .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
  .Build();

using IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices((context, services) =>
  {
    services.AddSingleton(new TestSingletonService(confBuilder));
    services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig()
    {
      LogLevel = LogSeverity.Verbose
    }));
    services.AddSingleton(confBuilder);

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
    services.AddSingleton<InteractionHandler>();
    services.AddSingleton(x => new CommandService());
    services.AddSingleton<PrefixHandler>();

    services.AddSingleton(x => new CommandService(new CommandServiceConfig()
    {
      LogLevel = LogSeverity.Debug,
      DefaultRunMode = Discord.Commands.RunMode.Async,
      CaseSensitiveCommands = false
    }));

  }).Build();


await RunAsync(host);

async Task RunAsync(IHost host)
{
  using IServiceScope serviceScope = host.Services.CreateScope();
  IServiceProvider provider = serviceScope.ServiceProvider;

  _client = provider.GetRequiredService<DiscordSocketClient>();
  var sCommands = provider.GetRequiredService<InteractionService>();
  await provider.GetRequiredService<InteractionHandler>().InitializeAsync();
  var config = provider.GetRequiredService<IConfigurationRoot>();

  _client.Ready += async () =>
  {
    Console.WriteLine("Bot is starting");
    await sCommands.RegisterCommandsToGuildAsync(ulong.Parse(config["guildId"]));
  };
  sCommands.Log += Log;

  await _client.LoginAsync(Discord.TokenType.Bot, config["marinaBotToken"]);
  _client.Log += Log;


  await _client.StartAsync();
  await Task.Delay(Timeout.Infinite);
}



static Task Log(LogMessage message)
{
  switch (message.Severity)
  {
    case LogSeverity.Critical:
    case LogSeverity.Error:
      Console.ForegroundColor = ConsoleColor.Red;
      break;
    case LogSeverity.Warning:
      Console.ForegroundColor = ConsoleColor.Yellow;
      break;
    case LogSeverity.Info:
      Console.ForegroundColor = ConsoleColor.White;
      break;
    case LogSeverity.Verbose:
    case LogSeverity.Debug:
      Console.ForegroundColor = ConsoleColor.DarkGray;
      break;
  }
  Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
  Console.ResetColor();

  // If you get an error saying 'CompletedTask' doesn't exist,
  // your project is targeting .NET 4.5.2 or lower. You'll need
  // to adjust your project's target framework to 4.6 or higher
  // (instructions for this are easily Googled).
  // If you *need* to run on .NET 4.5 for compat/other reasons,
  // the alternative is to 'return Task.Delay(0);' instead.
  return Task.CompletedTask;
}







