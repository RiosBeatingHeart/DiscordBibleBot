using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using OpenAI_API;
using DSharpPlus.SlashCommands;

namespace DiscordDemonBot.Source;

/// <summary>
/// Singleton class that wraps bot related data.
/// </summary>
public class Bot
{
    private static Bot? _instance;
    public static Bot Instance
    {
        get => _instance ??= new Bot();
        private set => _instance = value;
    }
    
    public HashSet<string> BibleWords { get; private set; }
    
    private DiscordClient? _discordClient;
    public DiscordClient DiscordClient
    {
        get => _discordClient ?? throw new NullReferenceException("Discord client not set!");
        set
        {
            _discordClient = value;
            SlashCommands = DiscordClient.UseSlashCommands();
        }
    }
    
    private SlashCommandsExtension? _slashCommands;

    public SlashCommandsExtension SlashCommands
    {
        get => _slashCommands ?? throw new NullReferenceException("Commands (CommandsNextExtension) not set!");
        private set => _slashCommands = value;
    }

    private string? _biblePath;
    public string BiblePath 
    { 
        get => _biblePath ?? throw new NullReferenceException("BiblePath not set!");
        set => _biblePath = value;
    }

    private OpenAIAPI? _openAiApi;
    public OpenAIAPI OpenAiApi {
        get => _openAiApi ?? throw new NullReferenceException("OpenAI API not set!");
        set => _openAiApi = value;
    }

    private int _minLetterLimit = 4;
    public int MinLetterLimit
    {
        get => _minLetterLimit;
        set => _minLetterLimit = value >= 0 ? value : 0;
    }

    private Bot()
    {
        BibleWords = new HashSet<string>();
    }

    /// <summary>
    /// Runs the Bot.
    /// </summary>
    public void Run()
    {
        LoadBible();
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// The function that actually runs the Bot.
    /// </summary>
    private async Task RunAsync()
    {
        await Console.Out.WriteLineAsync("Starting Bot!");
        await DiscordClient.ConnectAsync();
        await Task.Delay(-1);
    }

    /// <summary>
    /// Gets all the words form a Bible and puts them into the database.
    /// </summary>
    /// <returns>The singleton Bot instance.</returns>
    public Bot LoadBible()
    {
        Console.Out.WriteLine("Loading words from bible! This might take a little.");
        Console.Out.WriteLine($"Bible URL: {BiblePath}.");

        using (var stream = new HttpClient().GetStreamAsync(BiblePath).GetAwaiter().GetResult())
        {
            using var reader = new StreamReader(stream);

            for (var i = 0; i < 2; i++) //skip first x lines
                reader.ReadLine();

            var line = reader.ReadLine();
            while (line != null)
            {
                var start = line.IndexOf("    ", StringComparison.Ordinal) +
                            1; // get rid of the line number and book name
                if (start > 0)
                    line = line.Substring(start);

                BibleWords.UnionWith(Utils.GetWordsInString(line));
                line = reader.ReadLine();
            }
        }

        Console.Out.WriteLine($"Successfully loaded {BibleWords.Count} words!");
        return this;
    }

    /// <summary>
    /// Sets the client of the bot.
    /// </summary>
    /// <param name="client"></param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetClient(DiscordClient client)
    {
        DiscordClient = client;
        return this;
    }

    /// <summary>
    /// Sets the client to a new client constructed from the config.
    /// </summary>
    /// <param name="config">The config to construct the client from.</param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetClient(DiscordConfiguration? config)
    {
        DiscordClient = new DiscordClient(config);
        return this;
    }

    /// <summary>
    /// The minimal amount of letters in a message needed to activate the bot.
    /// </summary>
    /// <param name="minLetterLimit"></param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetMinLetterLimit(int minLetterLimit)
    {
        MinLetterLimit = minLetterLimit;
        return this;
    }

    /// <summary>
    /// The URL to a bible int text format. Only the bibles from openbible.com really work.
    /// </summary>
    /// <param name="biblePath"></param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetBiblePath(string biblePath)
    {
        BiblePath = biblePath;
        return this;
    }

    /// <summary>
    /// The API used to communicate with the AI.
    /// </summary>
    /// <param name="api">An OpenAI API instance.</param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetOpenAiApi(OpenAIAPI api)
    {
        OpenAiApi = api;
        return this;
    }
}