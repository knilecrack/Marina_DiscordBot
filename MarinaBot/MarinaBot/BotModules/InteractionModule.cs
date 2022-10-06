using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarinaBot.BotModules;
public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
  private const string _ytLink = @"https://www.youtube.com/watch?v=bl5q4AIGGZQ&ab_channel=OpeningThemeSongs";
  private readonly IConfigurationRoot _configBuilder;
  private readonly IServiceProvider _services;


  public InteractionModule(IConfigurationRoot configBuilder, IServiceProvider services)
  {
    _configBuilder = configBuilder;
    _services = services;
  }

  [SlashCommand("pali", "Dodaje pali na početak poruke")]
  public Task HandlePaliAsync(string message)
  {
    var currentUsers = Context.Guild.VoiceChannels;
    return RespondAsync($"Pali ***{message}***");
  }

  [SlashCommand("ae", "Dodaje pali na početak poruke")]
  public Task HandleAePaliAsync(string message)
  {
    return RespondAsync($"Ae, pali ***{message}***");
  }

  [SlashCommand("2ipo", "Dva i po coveka")]
  public Task Handle2ipo() => RespondAsync($"Stisni play: {_ytLink}");

  [SlashCommand("gazda", "Ko je ovde gazda?")]
  public async Task HandleGazda()
  {
    using var scope = _services.CreateScope();
    var provider = scope.ServiceProvider;
    var guildInfo = provider.GetRequiredService<GuildInformation>();
    var gazda = await guildInfo.GetGuildOwner(ulong.Parse(_configBuilder["guildId"]));
    await RespondAsync($"Ovde je gazda: <@{gazda?.Id}>. Voli punjeno pileće belo.");
  }

}
