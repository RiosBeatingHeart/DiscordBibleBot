using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DiscordDemonBot.Source;
public static class Utils
{
    /// <summary>
    /// Splits a text string into its words.
    /// </summary>
    /// <param name="str">The string that should be split.</param>
    /// <param name="preCleaned">Whether the string has already been cleaned.</param>
    /// <returns>The words.</returns>
    public static IEnumerable<string> GetWordsInString(string str, bool preCleaned = false)
    {
        str = preCleaned ? str : CleanString(str);
        return str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
        
    /// <summary>
    /// Replaces all chars except letters with spaces and merges adjacent spaces into one.
    /// </summary>
    /// <param name="str">The string to clean.</param>
    /// <returns>A cleaned string.</returns>
    public static string CleanString(string str)
    {
        List<char> cleaned = new List<char>
        {
            Capacity = str.Length
        };
        foreach (char c in str.ToLower().ToCharArray())
        {
            if (char.IsLetter(c))
                cleaned.Add(c);
            else if (cleaned.Count == 0 || cleaned.Last() != ' ')
                cleaned.Add(' ');
        }
        return new string(cleaned.ToArray());
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
}