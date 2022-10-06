using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarinaBot.BotModules;
public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
  private const string _ytLink = @"https://www.youtube.com/watch?v=bl5q4AIGGZQ&ab_channel=OpeningThemeSongs";
  private readonly IConfigurationRoot _configBuilder;
  private readonly GuildInformation _guildInfo;

  private const ulong MarinaId = 760623372089819187;


  public InteractionModule(IConfigurationRoot configBuilder, GuildInformation guildInfo)
  {
    _configBuilder = configBuilder;
    _guildInfo = guildInfo;
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
    var gazda = await _guildInfo.GetGuildOwner(ulong.Parse(_configBuilder["vladaGuild"]));
    await RespondAsync($"Ovde je gazda: <@{gazda?.Id}>. Voli punjeno pileće belo.");
  }

  [SlashCommand("mmute", "Divni mute")]
  public async Task HandleMMMute()
  {
    var marinaInVoice = Context.Guild.VoiceChannels
      .SelectMany(a => a.ConnectedUsers)
      .FirstOrDefault(u => u.Id == MarinaId);

    var guild = Context.Guild as IGuild;
    //await user.ModifyAsync(x => x.Mute = false);
    if (marinaInVoice == null)
    {
      await RespondAsync("Marina nije tu.");
      return;
    }

    if (!marinaInVoice!.IsMuted)
    {
      await RespondAsync($"Marina je mutovana. Manje psuj <@{MarinaId}> :) :)");
    } 
    var user = await guild.GetUserAsync(MarinaId);
    await user.ModifyAsync(x => x.Mute = true);
    await RespondAsync($"<@{MarinaId}> manje psuj!");
  }
}
