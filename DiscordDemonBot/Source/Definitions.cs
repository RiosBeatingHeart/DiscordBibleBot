using System.IO;
using System.Reflection;

namespace DiscordDemonBot.Source;

public static class Definitions
{
    public static readonly string ResourceDir = Path.Join(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
        @"Resources");
}