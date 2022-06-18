using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using DiscordBibleBot.source;
using DSharpPlus;

namespace DiscordBibleBot.Source.Database
{
    public class SQLiteDatabase: IDatabase
    {
        public string? Path { get; set; }
        private SQLiteConnection? Connection { get; set; }

        ~SQLiteDatabase() { Connection?.Close(); }
        
        public IDatabase Connect()
        {
            if(Path == null)
                throw new IDatabase.DatabaseException("Database path not set!");
            
            if (!File.Exists(Path))
            {
                var dir = System.IO.Path.GetDirectoryName(Path);
                if (dir != null)
                {
                    Directory.CreateDirectory(dir);
                    SQLiteConnection.CreateFile(Path);
                }
            }
            
            Connection = new SQLiteConnection($"Data Source={System.IO.Path.GetFileName(Path)};Version=3;");
            Connection.Open();
            
            InitTables();
            return this;
        }
        
        /// <summary>
        /// Initializes the necessary tables in the database if they don't exist already.
        /// </summary>
        /// <returns>The object from which the method was called.</returns>
        private IDatabase InitTables()
        {
            string query = 
                "CREATE TABLE IF NOT EXISTS Words (Word VARCHAR(255) PRIMARY KEY UNIQUE NOT NULL)";
            ExecuteWrite(query);
            
            query = 
                "CREATE TABLE IF NOT EXISTS Channels (" +
                "Channel INTEGER PRIMARY KEY UNIQUE NOT NULL, " +
                "Joined INTEGER NOT NULL DEFAULT(1))";
            ExecuteWrite(query);
            
            query = "CREATE TABLE IF NOT EXISTS Users ("+
                  "User INTEGER PRIMARY KEY NOT NULL, "+
                  "Follows INTEGER NOT NULL DEFAULT(0), "+
                  "Count INTEGER NOT NULL DEFAULT(0))";
            ExecuteWrite(query);
            
            return this;
        }
        
        /// <summary>
        /// Sets the path to a the database file.
        /// </summary>
        /// <param name="path">The Absolute path to a the database file.</param>
        /// <returns>The object from which the method was called.</returns>
        public SQLiteDatabase SetPath(string path)
        {
            Path = path;
            return this;
        }

        public bool WordsAdd(string word)
        {
            string query =
                "INSERT OR IGNORE INTO Words(Word) VALUES(@word)";
            return ExecuteWrite(query, "@word", word) > 0;
        }
        
        public int WordsAdd(IEnumerable<string> words)
        {
            var counter = 0;
            foreach (string word in words)
                counter += WordsAdd(word)? 1: 0;
            return counter;
        }

        public bool WordsDelete(string word)
        {
            string query = 
                "DELETE FROM Words WHERE Word = @word";
            return ExecuteWrite(query, "@word", word) > 0;
        }

        public int WordsDelete(IEnumerable<string> words)
        {
            var counter = 0;
            foreach (string word in words)
                counter += WordsDelete(word)? 1: 0;
            return counter;
        }

        public bool WordsContain(string word)
        {
            string query = 
                "SELECT * FROM Words WHERE Word = @word";
            var result = ExecuteRead(query, "@word", word);
            return result != null && result.Rows.Count > 0;
        }
        
        public bool WordsContain(IEnumerable<string> words)
        {
            foreach (var word in words)
                if (WordsContain(word))
                    return true;
            return false;
        }
        
        public int WordsGetCount()
        {
            string query = 
                "SELECT * FROM Words";
            var result = ExecuteRead(query);
            return result?.Rows.Count ?? 0;
        }

        public bool ChannelsJoin(ulong id)
        {
            if (ChannelsJoined(id))
                return false;
            
            string query =
                "INSERT OR REPLACE INTO Channels(Channel, Joined) VALUES(@id, 1)";
            ExecuteWrite(query, "@id", id);
            return true;
        }
        
        public bool ChannelsLeave(ulong id)
        {
            if (!ChannelsJoined(id))
                return false;
            
            string query =
                "INSERT OR REPLACE INTO Channels(Channel, Joined) VALUES(@id, 0)";
            ExecuteWrite(query, "@id", id);
            return true;
        }

        public bool ChannelsJoined(ulong id)
        {
            string query = 
                "SELECT * FROM Channels WHERE Channel = @id";
            var result = ExecuteRead(query, "@id", id);

            return result != null && result.Rows.Count > 0 && (long)result.Rows[0]["Joined"] != 0;
        }
        
        /// <summary>
        /// Whether a channel is in the database.
        /// </summary>
        /// <param name="id">The channel's id.</param>
        /// <returns>Whether the channel is in the database.</returns>
        private bool ChannelsContain(ulong id)
        {
            string query = 
                "SELECT * FROM Channels WHERE Channel = @id";
            var result = ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0;
        }

        public void UsersFollow(ulong id)
        {
            var count = UsersGetCount(id);
            string query =
                "INSERT OR REPLACE INTO Users(User, Follows, Count) VALUES(@id, 1, @count)";
            ExecuteWrite(query, new Dictionary<string, object>(){{"@id", id}, {"@count", count}});
        }
        
        public void UsersUnfollow(ulong id)
        {
            var count = UsersGetCount(id);
            string query =
                "INSERT OR REPLACE INTO Users(User, Follows, Count) VALUES(@id, 0, @count)";
            ExecuteWrite(query, new Dictionary<string, object>(){{"@id", id}, {"@count", count}});
        }

        public bool UsersFollows(ulong id)
        {
            string query = 
                "SELECT * FROM Users WHERE User = @id";
            var result = ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0 && (long)result.Rows[0]["Follows"] == 1;
        }
    
        public long UsersUpCount(ulong id)
        {
            string query;
            if(!UsersContain(id))
            {
                query =
                    "INSERT INTO Users(User, Follows) VALUES(@id, 1)";
                ExecuteWrite(query, "@id", id);
                return 1;
            }

            long count = UsersGetCount(id);
            query = 
                "UPDATE Users SET Count = @count WHERE User = @id";
            ExecuteWrite(query, new Dictionary<string, object>(){{"@id", id}, {"@count", count+1}});
            return count+1;
        }
        
        public long UsersGetCount(ulong id)
        {
            string query = 
                "SELECT * FROM Users WHERE User = @id";
            var result = ExecuteRead(query, "@id", id);
            if (!(result != null && result.Rows.Count > 0))
                return 0;
            return (long)result.Rows[0]["Count"];
        }
        
        /// <summary>
        /// Whether a user is in the database.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <returns>Whether the user is in the database.</returns>
        private bool UsersContain(ulong id)
        {
            string query = 
                "SELECT * FROM Users WHERE User = @id";
            var result = ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0;
        }
        
        /// <summary>
        /// Executes a parameterized write query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <param name="kwargs">The parameters.</param>
        /// <returns>The number of changed rows.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private int ExecuteWrite(string query, Dictionary<string, object> kwargs)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");
            
            using var cmd = new SQLiteCommand(query, Connection);
            foreach (var (key, value) in kwargs)
                cmd.Parameters.AddWithValue(key, value);

            return cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Executes a parameterized write query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <param name="keyword">The keyword of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The number of changed rows.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private int ExecuteWrite(string query, string keyword, object value)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");

            using var cmd = new SQLiteCommand(query, Connection);
            cmd.Parameters.AddWithValue(keyword, value);

            return cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Executes a write query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <returns>The number of changed lines.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private int ExecuteWrite(string query)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");

            using var cmd = new SQLiteCommand(query, Connection);

            return cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Executes a parameterized read query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <param name="kwargs">The parameters.</param>
        /// <returns>The result table.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private DataTable? ExecuteRead(string query, Dictionary<string, object> kwargs)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");
            
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using var cmd = new SQLiteCommand(query, Connection);
            foreach (var (keyword, value) in kwargs)
                cmd.Parameters.AddWithValue(keyword, value);

            var da = new SQLiteDataAdapter(cmd);

            var dt = new DataTable();
            
            da.Fill(dt);
            da.Dispose();
            return dt;
        }
        
        /// <summary>
        /// Executes a parameterized read query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <param name="keyword">The keyword of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The result table.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private DataTable? ExecuteRead(string query, string keyword, object value)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");
            
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using var cmd = new SQLiteCommand(query, Connection);
            cmd.Parameters.AddWithValue(keyword, value);

            var da = new SQLiteDataAdapter(cmd);

            var table = new DataTable();
            
            da.Fill(table);
            da.Dispose();
            return table;
        }
        
        /// <summary>
        /// Executes a read query with the database.
        /// </summary>
        /// <param name="query">The sql query.</param>
        /// <returns>The result table.</returns>
        /// <exception cref="IDatabase.DatabaseException">If the database isn't connected.</exception>
        private DataTable? ExecuteRead(string query)
        {
            if(Connection == null)
                throw new IDatabase.DatabaseException("Database not connected!");
            
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using var cmd = new SQLiteCommand(query, Connection);

            var da = new SQLiteDataAdapter(cmd);

            var table = new DataTable();
            
            da.Fill(table);
            da.Dispose();
            return table;
        }
    }
}