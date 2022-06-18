using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using DSharpPlus;

namespace DiscordBibleBot.source
{
    public interface IDatabase
    {
        /// <summary>
        /// Builds a connection to the database.
        /// </summary>
        /// <returns>The object from which the method was called.</returns>
        /// <exception cref="DatabaseException">If no connection to the database can be created.</exception>
        public IDatabase Connect();

        /// <summary>
        /// Adds a word to the database. Does nothing if the word already exists.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the word was already in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool WordsAdd(string word);
        
        /// <summary>
        /// Adds multiple words to the database.
        /// Only words that aren't in the database yet will be added.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>The amount of words that were added.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int WordsAdd(IEnumerable<string> words);
        
        /// <summary>
        /// Deletes a word from the database.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the word was in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool WordsDelete(string word);
        
        /// <summary>
        /// Deletes words from the database.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>The number of words that has been deleted from the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int WordsDelete(IEnumerable<string> words);
        
        /// <summary>
        /// Whether the database contains a word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the database contains the word.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool WordsContain(string word);

        /// <summary>
        /// Whether the database contains any words from a list.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>Whether the database contains any of the words.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool WordsContain(IEnumerable<string> words);

        /// <summary>
        /// The number of unique words in the database.
        /// </summary>
        /// <returns>The number of unique words in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int WordsGetCount();
        
        /// <summary>
        /// Adds the bot to a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>True if the bot wasn't in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool ChannelsJoin(ulong id);
        
        /// <summary>
        /// Removes the bot from a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>True if the bot was in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool ChannelsLeave(ulong id);
        
        /// <summary>
        /// Whether the bot is in a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>Whether the bot is in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool ChannelsJoined(ulong id);
        
        /// <summary>
        /// Makes the bot follow a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public void UsersFollow(ulong id);
        
        /// <summary>
        /// Makes the bot unfollow a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public void UsersUnfollow(ulong id);
        
        /// <summary>
        /// Whether the bot follows a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>Whether the bot follows a user.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool UsersFollows(ulong id);
        
        /// <summary>
        /// Increases the count of a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>The new count.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public long UsersUpCount(ulong id);
        
        /// <summary>
        /// The count of a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>The count of the user.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public long UsersGetCount(ulong id);
        
        /// <summary>
        /// An exception that can be thrown by databases.
        /// </summary>
        [Serializable]
        public class DatabaseException : Exception
        {
            public DatabaseException() : base() { }
            public DatabaseException(string message) : base(message) { }
            
            protected DatabaseException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}