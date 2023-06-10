using System;
using System.Configuration;
using System.Threading;
using DiscordDemonBot.Source.Commands;
using DSharpPlus;
using Microsoft.Extensions.Logging;
using OpenAI_API;
using static DiscordDemonBot.Source.Utils;

namespace DiscordDemonBot.Source;
internal static class Program
{
    private static void Main(string[] args)
    {
        bool running = true;
        while (running)
        {

            if (!IsOnline())
            {
                Console.WriteLine("No internet connection detected. Next attempt in 60 seconds.");
                Thread.Sleep(60000);
                continue;
            }

            string oauth2Token = ConfigurationManager.AppSettings["BotToken"] ?? throw new NullReferenceException();
            string biblePath = ConfigurationManager.AppSettings["BiblePath"] ?? throw new NullReferenceException();
            string openAiKey = ConfigurationManager.AppSettings["OpenAiApiKey"] ?? throw new NullReferenceException();



            Bot bibleBot = Bot.Instance
                .SetBiblePath(biblePath)
                .SetClient(new DiscordConfiguration
                {
                    Token = oauth2Token,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged,
                    MinimumLogLevel = LogLevel.Information,
                });
            // .SetOpenAiApi(new OpenAIAPI(new APIAuthentication(openAiKey), Engine.Davinci));
            // .SetDatabase(new SqLiteDatabase().SetPath(Path.Join(Definitions.ResourceDir, databasePath)));

            if (ConfigurationManager.AppSettings["MinWordLimit"] != null)
                bibleBot.SetMinLetterLimit(int.Parse(ConfigurationManager.AppSettings["MinLetterLimit"]!));

            // register event handlers
            bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedReplyBot;
            bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedCheckable; // got annoying
            bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedVotableContent;
            bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedMentionBot;
            bibleBot.DiscordClient.MessageReactionAdded += EventHandlers.ReactionAdded;

            // register commands
            bibleBot.SlashCommands.RegisterCommands<BasicCommands>();
            bibleBot.SlashCommands.RegisterCommands<PornCommands>();
            bibleBot.SlashCommands.RegisterCommands<UtilityCommands>();

            bibleBot.Run();
        }
    }
}