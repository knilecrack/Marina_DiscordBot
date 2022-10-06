using Discord;
using Discord.WebSocket;

public class GuildInformation
{
  private readonly DiscordSocketClient _client;

  public GuildInformation(DiscordSocketClient client)
  {
    _client = client;
  }

  public async Task<IUser?> GetGuildOwner(ulong id)
  {
    var getGuild = _client.Guilds.FirstOrDefault(a => a.Id == id);
    var user = await _client.Rest.GetGuildAsync(id, RequestOptions.Default);
    var rez = await _client.GetUserAsync(user.OwnerId, RequestOptions.Default);
    return rez;
  }
}






