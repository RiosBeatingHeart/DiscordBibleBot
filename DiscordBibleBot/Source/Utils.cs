using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBibleBot.Source;
public static class Utils
{
    /// <summary>
    /// Splits a text string into its words.
    /// </summary>
    /// <param name="str">The string that should be split.</param>
    /// <param name="cleaned">Whether the string has already been cleaned.</param>
    /// <returns>The words.</returns>
    public static IEnumerable<string> GetWordsInString(string str, bool cleaned = false)
    {
        str = cleaned ? str : CleanString(str);
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
}