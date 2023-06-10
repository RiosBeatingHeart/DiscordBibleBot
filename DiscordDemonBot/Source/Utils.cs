using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.VisualBasic;

namespace DiscordDemonBot.Source;
public static class Utils
{
    
    /// <summary>
    /// Separates words in a string by whitespaces and optionally removes special characters from those words.
    /// </summary>
    /// <param name="str">The string to clean.</param>
    /// <param name="removeSpecialChars">Whether special chars (all non letter chars) should get removed.</param>
    /// <returns>A cleaned string.</returns>
    public static string[] GetWordsInString(string str, bool removeSpecialChars = true)
    {
        
        string? workStr = str.Select(c => char.IsWhiteSpace(c) ? ' ' : c).ToString();
        if (workStr is null) return Array.Empty<string>();
        
        string[] words = str.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (removeSpecialChars) // remove special chars from all words
            words = words.Select(word => word.Where(char.IsLetter).ToString()).ToArray()!;

        return words;
    }
    
    /// <summary>
    /// whether any of the words in the message are in the bible.
    /// </summary>
    /// <param name="message">the message to check.</param>
    /// <returns>true: if any are in the bible, else false</returns>
    public static bool CheckMessage(DiscordMessage message)
    {
        HashSet<string> words = new();
        words.UnionWith(GetWordsInString(message.Content));
        return Bot.Instance.BibleWords.Overlaps(words);
    }
    
    /// <summary>
    /// whether any of the words in the text are in the bible.
    /// </summary>
    /// <param name="text">the text to check.</param>
    /// <returns>true: if any are in the bible, else false</returns>
    public static bool CheckMessage(string text)
    {
        HashSet<string> words = new();
        words.UnionWith(GetWordsInString(text));
        return Bot.Instance.BibleWords.Overlaps(words);
    }
    
    /// <summary>
    /// whether any of the words are in the bible.
    /// </summary>
    /// <param name="words">words to check.</param>
    /// <returns>true: if any are in the bible, else false</returns>
    public static bool CheckMessage(params string[] words) 
        => Bot.Instance.BibleWords.Overlaps(words);
    
    /// <summary>
    /// Tries pinging an IP to check if the program is connected to the internet.
    /// </summary>
    /// <param name="hostIp">IP/URL of the server to ping. default: discord</param>
    /// <returns>true. if online. else false</returns>
    public static bool IsOnline(string  hostIp = "discord.com")
    {
        Ping myPing = new Ping();
        byte[] buffer = new byte[32];
        int timeout = 1000;
        PingOptions pingOptions = new PingOptions();
        PingReply reply = myPing.Send(hostIp, timeout, buffer, pingOptions);
        return reply.Status == IPStatus.Success;
    }

    public static string GenerateKeySmash(int length, bool lettersOnly=false)
    {
        if (length <= 0) throw new ArithmeticException("Length has to be bigger than 0!");
        
        List<char> ret = new List<char>(length);
        char[] rand = Array.Empty<char>();
        int randPos = length;
        while (ret.Count < length)
        {
            if (randPos >= length)
            {
                rand = Convert.ToBase64String(RandomNumberGenerator.GetBytes(length)).ToCharArray();
                randPos = 0;
            }
            
            if (!lettersOnly || char.IsLetter(rand[randPos]))
                ret.Add(rand[randPos]);
            randPos++;
        }

        return new string(ret.ToArray()).ToLower();
    }
}

