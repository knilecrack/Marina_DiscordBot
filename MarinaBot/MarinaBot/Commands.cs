using Discord.Interactions;
using SummaryAttribute = Discord.Interactions.SummaryAttribute;

namespace MarinaBot;


// Create a module with the 'sample' prefix
public class SampleModule : InteractionModuleBase<SocketInteractionContext>
{
  // ~sample square 20 -> 400
  [SlashCommand("square", "Kvadrat broja, jer sam mnogo dobra sa matematikom.")]
  public async Task HandleSquareAsync(int message)
  {
    // We can also access the channel from the Command Context.
    await Context.Channel.SendMessageAsync($"{message}^2 = {Math.Pow(message, 2)}.\n Nauci matematiku majmune.");
    await RespondAsync("...");
  }
}
