using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MarinaBot;
using MarinaBot.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;


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
      LogLevel = LogSeverity.Debug
    }));
    services.AddSingleton(confBuilder);

    services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
    services.AddSingleton<InteractionHandler>();
    services.AddSingleton(x => new CommandService());
    services.AddSingleton<PrefixHandler>();
    services.AddSingleton<LoggingService>();
    services.AddTransient<GuildInformation>();

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

  var _ = provider.GetRequiredService<LoggingService>();
  var guildInfo = provider.GetRequiredService<GuildInformation>();
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
    //await sCommands.RegisterCommandsToGuildAsync(ulong.Parse(config["guildId"]));
    await sCommands.RegisterCommandsToGuildAsync(ulong.Parse(config["vladaGuild"]));
  };

  await _client.LoginAsync(Discord.TokenType.Bot, config["marinaBotToken"]);

  await _client.StartAsync();
  await Task.Delay(Timeout.Infinite);
}


