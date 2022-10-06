using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MarinaBot;
using MarinaBot.BotModules;
using MarinaBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


DiscordSocketClient _client;
//CommandService _commands;
//IServiceProvider _services;

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
      LogLevel = LogSeverity.Debug
    }));
    services.AddSingleton(confBuilder);

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
    services.AddSingleton<InteractionHandler>();
    services.AddSingleton(x => new CommandService());
    services.AddSingleton<PrefixHandler>();
    services.AddSingleton<LoggingService>();
    services.AddTransient<GuildInformation>();
    services.AddSingleton<GuildIdFromConfig>();

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

  //var _ = provider.GetRequiredService<LoggingService>();
  //var guildInfo = provider.GetRequiredService<GuildInformation>();
  var guildIdFromConfig = provider.GetRequiredService<GuildIdFromConfig>();
  _client = provider.GetRequiredService<DiscordSocketClient>();
  var sCommands = provider.GetRequiredService<InteractionService>();
  await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

  var config = provider.GetRequiredService<IConfigurationRoot>();


  Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

  _client.Ready += async () =>
  {
    await sCommands.RegisterCommandsToGuildAsync(guildIdFromConfig.GuildId());
  };

  await _client.LoginAsync(Discord.TokenType.Bot, GetBotToken(config));

  await _client.StartAsync();
  await Task.Delay(Timeout.Infinite);
}

static string GetBotToken(IConfigurationRoot conf)
{
  var valueFromConfig = conf["marinaBotToken"];
  if (!string.IsNullOrEmpty(valueFromConfig))
  {
    return valueFromConfig;
  }
  var getEnv = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
  if (!string.IsNullOrEmpty(getEnv))
  {
    return getEnv;
  }
  return string.Empty;
}



