using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DiscordBibleBot.Source.Database;
public interface IDatabase
{

    public IWords Words { get; }
    public IChannels Channels { get; }
    public IUsers Users{ get; }
    
    /// <summary>
    /// Builds a connection to the database.
    /// </summary>
    /// <returns>The object from which the method was called.</returns>
    /// <exception cref="DatabaseException">If no connection to the database can be created.</exception>
    public IDatabase Connect();

    public interface IWords
    {
        /// <summary>
        /// The number of unique words in the database.
        /// </summary>
        /// <returns>The number of unique words in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int Count { get; }
        
        /// <summary>
        /// Adds a word to the database. Does nothing if the word already exists.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the word was already in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Add(in string word);
        
        /// <summary>
        /// Adds words to the database. Does nothing for words that already exists.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>The number of words newly added to the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int Add(in IEnumerable<string> words);
        
        /// <summary>
        /// Deletes a word from the database.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the word was in the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Remove(in string word);
        
        /// <summary>
        /// Deletes words from the database.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>The number of words that have been deleted from the database.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public int Remove(in IEnumerable<string> words);
        
        /// <summary>
        /// Whether the database contains a word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns>Whether the database contains the word.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Contains(in string word);
        
        /// <summary>
        /// Whether the database contains any words from a list.
        /// </summary>
        /// <param name="words"></param>
        /// <returns>Whether the database contains any of the words.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Contains(in IEnumerable<string> words);
    }

    public interface IChannels
    {
        /// <summary>
        /// Adds the bot to a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>True if the bot wasn't in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Join(in ulong id);
        
        /// <summary>
        /// Removes the bot from a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>True if the bot was in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool Leave(in ulong id);
        
        /// <summary>
        /// Whether the bot is in a channel.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>Whether the bot is in the channel.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool IsJoined(in ulong id);
    }

    public interface IUsers
    {
        /// <summary>
        /// Makes the bot follow a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public void Follow(in ulong id);
        
        /// <summary>
        /// Makes the bot unfollow a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public void Unfollow(in ulong id);
        
        /// <summary>
        /// Whether the bot follows a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>Whether the bot follows a user.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public bool IsFollowing(in ulong id);
        
        /// <summary>
        /// Increases the count of a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>The new count.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public long IncreaseMessageCount(in ulong id);

        /// <summary>
        /// The count of a user.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>The count of the user.</returns>
        /// <exception cref="DatabaseException">If the database isn't connected.</exception>
        public long GetMessageCount(in ulong id);
    }

    /// <summary>
    /// An exception that can be thrown by databases.
    /// </summary>
    [Serializable]
    public class DatabaseException : Exception
    {
        public DatabaseException() : base() { }
        public DatabaseException(in string message) : base(message) { }
            
        protected DatabaseException(SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}