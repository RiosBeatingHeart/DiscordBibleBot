using System;
using System.Configuration;
using System.IO;
using DiscordBibleBot.Source.Database;
using DSharpPlus;
using OpenAI_API;

namespace DiscordBibleBot.Source;
internal static class Program
{
    private static void Main(string[] args)
    {
        string databasePath = ConfigurationManager.AppSettings["DatabasePath"] ?? throw new NullReferenceException();
        string oauth2Token = ConfigurationManager.AppSettings["BotToken"] ?? throw new NullReferenceException();
        string biblePath = ConfigurationManager.AppSettings["BiblePath"] ?? throw new NullReferenceException();
        string openAiKey = ConfigurationManager.AppSettings["OpenAiApiKey"] ?? throw new NullReferenceException();
        
        
        
        Bot bibleBot = Bot.Instance
            .SetBiblePath(biblePath)
            .SetClient(new DiscordConfiguration
            {
                Token = oauth2Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            })
            .SetDatabase(new MemoryDatabase())
            .SetOpenAiApi(new OpenAIAPI(new APIAuthentication(openAiKey), Engine.Davinci));
        // .SetDatabase(new SqLiteDatabase().SetPath(Path.Join(Definitions.ResourceDir, databasePath)))
            
        if (ConfigurationManager.AppSettings["MinWordLimit"] != null)
            bibleBot.SetMinLetterLimit(int.Parse(ConfigurationManager.AppSettings["MinLetterLimit"]!));

        // register event handlers
        bibleBot.Client.MessageCreated += EventHandlers.HandleMessageCreated;
        bibleBot.Client.MessageReactionAdded += EventHandlers.HandleReactionAdded;

        // register commands
        bibleBot.Commands.RegisterCommands<Commands>();

        bibleBot.Run();
    }
}