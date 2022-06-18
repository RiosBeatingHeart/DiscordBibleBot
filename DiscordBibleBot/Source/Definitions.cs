using System.IO;
using System.Reflection;

namespace DiscordBibleBot
{
    public static class Definitions
    {
        public static readonly string ResourceDir = Path.Join(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
            @"resources");
    }
}