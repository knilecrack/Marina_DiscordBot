using Discord.Commands;
using Discord.WebSocket;

namespace MarinaBot;
public class InfoModule : ModuleBase<SocketCommandContext>
{
  // ~say hello world -> hello world
  [Command("/say")]
  [Summary("Echoes a message.")]
  public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
  {
    string buildMesasge = $"Pali {echo}";
    return ReplyAsync(buildMesasge);
  }

  [Command("/pali")]
  [Summary("Dodaje pali na pocetak poruke")]
  public Task PaliAsync([Remainder] [Summary("Dodaje pali na pocetak")] string msg)
  {
    return ReplyAsync($"Pali {msg}");
  } 

  // ReplyAsync is a method on ModuleBase 
}


// Create a module with the 'sample' prefix
[Group("/matematika")]
public class SampleModule : ModuleBase<SocketCommandContext>
{
  // ~sample square 20 -> 400
  [Command("square")]
  [Summary("Squares a number.")]
  public Task SquareAsync(
    [Summary("The number to square.")]
    int num)
  {
    // We can also access the channel from the Command Context.
    return Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}.\n Nauci matematiku majmune.");
  }

  // ~sample userinfo --> foxbot#0282
  // ~sample userinfo @Khionu --> Khionu#8708
  // ~sample userinfo Khionu#8708 --> Khionu#8708
  // ~sample userinfo Khionu --> Khionu#8708
  // ~sample userinfo 96642168176807936 --> Khionu#8708
  // ~sample whois 96642168176807936 --> Khionu#8708
  [Command("userinfo")]
  [Summary
    ("Returns info about the current user, or the user parameter, if one passed.")]
  [Alias("user", "whois")]
  public Task UserInfoAsync(
    [Summary("The (optional) user to get info from")]
    SocketUser? user = null)
  {
    var userInfo = user ?? Context.Client.CurrentUser;
    return ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
  }
}

//public class TwoAndAHalfMenSong : ModuleBase<SocketCommandContext>
//{
//  private const string _ytLink = @"https://www.youtube.com/watch?v=bl5q4AIGGZQ&ab_channel=OpeningThemeSongs";

//  public Task PlayMe()
//  {
//  }
//}
public class ReciModule : ModuleBase<SocketCommandContext>
{
  [Command("reci")]
  [Summary("Vraca eho")]
  public Task ReciAsyhnc(
    [Summary("The number to square.")]
    string msg)
  {
    return ReplyAsync($"Pali {msg}");
  }
}

