using System;
using System.Configuration;
using DiscordDemonBot.Source.Commands;
using DSharpPlus;
using OpenAI_API;

namespace DiscordDemonBot.Source;
internal static class Program
{
    private static void Main(string[] args)
    {
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
            .SetOpenAiApi(new OpenAIAPI(new APIAuthentication(openAiKey), Engine.Davinci));
        // .SetDatabase(new SqLiteDatabase().SetPath(Path.Join(Definitions.ResourceDir, databasePath)))
            
        if (ConfigurationManager.AppSettings["MinWordLimit"] != null)
            bibleBot.SetMinLetterLimit(int.Parse(ConfigurationManager.AppSettings["MinLetterLimit"]!));

        // register event handlers
        bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedReplyBot;
        bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedCheckable;
        bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedVotableContent;
        bibleBot.DiscordClient.MessageCreated += EventHandlers.MessageAddedMentionBot;
        bibleBot.DiscordClient.MessageReactionAdded += EventHandlers.ReactionAdded;

        // register commands
        bibleBot.SlashCommands.RegisterCommands<SlashCommands>();

        bibleBot.Run();
    }
}