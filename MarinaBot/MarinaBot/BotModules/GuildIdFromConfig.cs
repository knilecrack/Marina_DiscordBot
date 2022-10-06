using Microsoft.Extensions.Configuration;

namespace MarinaBot.BotModules;

public class GuildIdFromConfig
{
  private readonly IConfigurationRoot _confRoot;
  public GuildIdFromConfig(IConfigurationRoot confRoot) 
  {
    _confRoot = confRoot;
  }
  public ulong GuildId()
  {
    if (ulong.TryParse(_confRoot["guildId"], out ulong parsed))
    {
      return parsed;
    }

    var getValueFromEnv = Environment.GetEnvironmentVariable("DISCORD_GUILD");
    if(ulong.TryParse(getValueFromEnv, out ulong parsed2))
    {
      return parsed2;
    }

    return 0;
  }
}
