using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace MarinaBot.Handlers;

public class PrefixHandler
{
  private readonly DiscordSocketClient _client;
  private readonly CommandService _commandSvc;
  private readonly IConfigurationRoot _confRoot;
  private readonly IServiceProvider _services;

  public PrefixHandler(DiscordSocketClient client,
    CommandService commandSvc,
    IConfigurationRoot confRoot, IServiceProvider services)
  {
    _client = client;
    _commandSvc = commandSvc;
    _confRoot = confRoot;
    _services = services;
  }

  public async Task InitializeAsync()
  {
    _client.MessageReceived += HandleCommandAsync;
  }

  public Task AddModule<T>()
  {
    return _commandSvc.AddModuleAsync<T>(null);
  }

  public async Task HandleCommandAsync(SocketMessage msg)
  {
    var message = msg as SocketUserMessage;
    if (message is null)
      return;

    int argPos = 0;

    if (!(message.HasCharPrefix('/', ref argPos)) || !message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
        message.Author.IsBot) return;


    var context = new SocketCommandContext(_client, message);
    await _commandSvc.ExecuteAsync(context: context, argPos, services: _services);
  }
}
