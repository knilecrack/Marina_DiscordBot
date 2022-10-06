using Microsoft.Extensions.Configuration;

namespace MarinaBot;
public class BotConfiguration 
{
  public string? BotToken { get; set; }
  public ulong BandientKaput { get; set; }
}


public class TestSingletonService
{
  private readonly IConfiguration _conf;
  public TestSingletonService(IConfiguration conf) 
  {
    _conf = conf;
  }

  public void LogSomething()
  {
    Console.WriteLine(_conf["marinaBotToken"]);
  }
}

