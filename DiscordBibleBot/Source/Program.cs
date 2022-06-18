using System;
using System.Configuration;
using System.IO;
using DiscordBibleBot.Source.Database;
using DSharpPlus;
using OpenAI_API;

namespace DiscordBibleBot.Source
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string? databasePath = ConfigurationManager.AppSettings["DatabasePath"];
            string? oauth2Token = ConfigurationManager.AppSettings["BotToken"];
            string? biblePath = ConfigurationManager.AppSettings["BiblePath"];
            string? openAiKey = ConfigurationManager.AppSettings["OpenAiApiKey"];
            
            Bot bibleBot = Bot.Instance
                .SetBiblePath(biblePath)
                .SetDatabase(new SQLiteDatabase().SetPath(Path.Join(Definitions.ResourceDir, databasePath)))
                .SetClient(new DiscordConfiguration
                {
                    Token = oauth2Token,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged
                })
                .SetOpenAiApi(new OpenAIAPI(new APIAuthentication(openAiKey), Engine.Davinci));
            
            if (ConfigurationManager.AppSettings["MinWordLimit"] != null)
                bibleBot.SetMinLetterLimit(int.Parse(ConfigurationManager.AppSettings["MinLetterLimit"]));

            // register event handlers
            bibleBot.Client.MessageCreated += EventHandlers.HandleMessageCreated;
            bibleBot.Client.MessageReactionAdded += EventHandlers.HandleReactionAdded;

            // register commands
            bibleBot.Commands.RegisterCommands<Commands>();

            bibleBot.Run();
        }
    }
}