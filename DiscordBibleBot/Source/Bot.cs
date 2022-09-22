using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using OpenAI_API;
using DiscordBibleBot.Source.Database;

namespace DiscordBibleBot.Source;

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

    private DiscordClient? _client;
    public DiscordClient Client
    {
        get => _client ?? throw new Exception("Discord client not set!");
        set
        {
            _client = value;
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] {Icon}
            });
        }
    }

    private IDatabase? _database;
    public IDatabase Database
    {
        get => _database ?? throw new Exception("Database not set!");
        set => _database = value;
    }

    private CommandsNextExtension? _commands;
    public CommandsNextExtension Commands
    {
        get => _commands ?? throw new Exception("Commands (CommandsNextExtension) not set!");
        private set => _commands = value;
    }

    private string? _biblePath;
    public string BiblePath 
    { 
        get => _biblePath ?? throw new Exception("BiblePath not set!");
        set => _biblePath = value;
    }

    private OpenAIAPI? _openAiApi;
    public OpenAIAPI OpenAiApi {
        get => _openAiApi ?? throw new Exception("OpenAI API not set!");
        set => _openAiApi = value;
    }


    private int _minLetterLimit = 4;
    public int MinLetterLimit
    {
        get => _minLetterLimit;
        set => _minLetterLimit = value >= 0 ? value : 0;
    }

    public const string Icon = "🙏";
    
    private Bot() { }

    /// <summary>
    /// Runs the Bot.
    /// </summary>
    public void Run()
    {
        Database.Connect();
        if (Database.Words.Count < 10000)
            LoadBible();
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// The function that actually runs the Bot.
    /// </summary>
    private async Task RunAsync()
    {
        await Console.Out.WriteLineAsync("Starting Bot!");
        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

    /// <summary>
    /// Gets all the words form a Bible and puts them into the database.
    /// </summary>
    /// <returns>The singleton Bot instance.</returns>
    public Bot LoadBible()
    {
        Console.Out.WriteLine("Loading words! This might take a little.");

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

                Database?.Words.Add(Utils.GetWordsInString(line));
                line = reader.ReadLine();
            }
        }

        Console.Out.WriteLine($"Successfully loaded {Database?.Words.Count} words!");
        return this;
    }

    /// <summary>
    /// Sets the client of the bot.
    /// </summary>
    /// <param name="client"></param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetClient(DiscordClient client)
    {
        Client = client;
        return this;
    }

    /// <summary>
    /// Sets the client to a new client constructed from the config.
    /// </summary>
    /// <param name="config">The config to construct the client from.</param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetClient(DiscordConfiguration? config)
    {
        Client = new DiscordClient(config);
        return this;
    }

    /// <summary>
    /// Sets the database instance the bot will use.
    /// </summary>
    /// <param name="database"></param>
    /// <returns>The singleton Bot instance.</returns>
    public Bot SetDatabase(IDatabase database)
    {
        Database = database;
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