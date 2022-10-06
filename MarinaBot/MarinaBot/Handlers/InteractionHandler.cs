using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace MarinaBot.Handlers;

public class InteractionHandler
{
  private readonly InteractionService _commands;
  private readonly DiscordSocketClient _client;
  private readonly IServiceProvider _services;

  public InteractionHandler(IServiceProvider services,
    DiscordSocketClient client, InteractionService commands)
  {
    _services = services;
    _client = client;
    _commands = commands;
  }

  public async Task InitializeAsync()
  {
    await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    _client.InteractionCreated += HandleTransaction;
  }

  public async Task HandleTransaction(SocketInteraction arg)
  {
    try
    {
      var ctx = new SocketInteractionContext(_client, arg);
      await _commands.ExecuteCommandAsync(ctx, _services);
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      throw;
    }
  }

}
