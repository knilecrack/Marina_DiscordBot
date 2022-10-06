using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace MarinaBot.BotModules;
public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{

  [SlashCommand("pali", "Dodaje pali na početak poruke")]
  public Task HandlePaliAsync(string message)
  {
    return ReplyAsync($"Pali ***{message}*** brate narodnjaci su zakon");
  }
}
