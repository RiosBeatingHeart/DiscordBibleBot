using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace DiscordBibleBot.Source.Database;

public class SqLiteDatabase : IDatabase
{
    public string? Path { get; set; }
    private SQLiteConnection? Connection { get; set; }
    public IDatabase.IWords Words { get; private set; }
    public IDatabase.IChannels Channels { get; private set; }
    public IDatabase.IUsers Users { get; private set; }


    public SqLiteDatabase()
    {
        Words = new WordsImplementation(this);
        Channels = new ChannelsImplementation(this);
        Users = new UsersImplementation(this);
    }

    ~SqLiteDatabase()
    {
        Connection?.Close();
    }
    public IDatabase Connect()
    {
        if (Path == null)
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

        query = "CREATE TABLE IF NOT EXISTS Users (" +
                "User INTEGER PRIMARY KEY NOT NULL, " +
                "Follows INTEGER NOT NULL DEFAULT(0), " +
                "Count INTEGER NOT NULL DEFAULT(0))";
        ExecuteWrite(query);

        return this;
    }

    /// <summary>
    /// Sets the path to a the database file.
    /// </summary>
    /// <param name="path">The Absolute path to a the database file.</param>
    /// <returns>The object from which the method was called.</returns>
    public SqLiteDatabase SetPath(in string path)
    {
        Path = path;
        return this;
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
        if (Connection == null)
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
        if (Connection == null)
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
        if (Connection == null)
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
        if (Connection == null)
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
        if (Connection == null)
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
        if (Connection == null)
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

    public class WordsImplementation : IDatabase.IWords
    {
        private readonly SqLiteDatabase _parent;

        public int Count
        {
            get
            {
                string query =
                    "SELECT * FROM Words";
                var result = _parent.ExecuteRead(query);
                return result?.Rows.Count ?? 0;
            }
        }

        public WordsImplementation(SqLiteDatabase parent) { _parent = parent; }
            
        public bool Add(in string word)
        {
            string query =
                "INSERT OR IGNORE INTO Words(Word) VALUES(@word)";
            return _parent.ExecuteWrite(query, "@word", word) > 0;
        }

        public int Add(in IEnumerable<string> words)
        {
            var counter = 0;
            foreach (string word in words)
                counter += Add(word) ? 1 : 0;
            return counter;
        }

        public bool Remove(in string word)
        {
            string query =
                "DELETE FROM Words WHERE Word = @word";
            return _parent.ExecuteWrite(query, "@word", word) > 0;
        }

        public int Remove(in IEnumerable<string> words)
        {
            var counter = 0;
            foreach (string word in words)
                counter += Remove(word) ? 1 : 0;
            return counter;
        }

        public bool Contains(in string word)
        {
            string query =
                "SELECT * FROM Words WHERE Word = @word";
            var result = _parent.ExecuteRead(query, "@word", word);
            return result != null && result.Rows.Count > 0;
        }

        public bool Contains(in IEnumerable<string> words)
        {
            foreach (var word in words)
                if (Contains(word))
                    return true;
            return false;
        }
    }

    public class ChannelsImplementation : IDatabase.IChannels
    {
        private readonly SqLiteDatabase _parent;
        public ChannelsImplementation(SqLiteDatabase parent) { _parent = parent; }
        
        public bool Join(in ulong id)
        {
            if (IsJoined(id))
                return false;

            string query =
                "INSERT OR REPLACE INTO Channels(Channel, Joined) VALUES(@id, 1)";
            _parent.ExecuteWrite(query, "@id", id);
            return true;
        }

        public bool Leave(in ulong id)
        {
            if (!IsJoined(id))
                return false;

            string query =
                "INSERT OR REPLACE INTO Channels(Channel, Joined) VALUES(@id, 0)";
            _parent.ExecuteWrite(query, "@id", id);
            return true;
        }

        public bool IsJoined(in ulong id)
        {
            string query =
                "SELECT * FROM Channels WHERE Channel = @id";
            var result = _parent.ExecuteRead(query, "@id", id);

            return result != null && result.Rows.Count > 0 && (long) result.Rows[0]["Joined"] != 0;
        }

        private bool Contains(in ulong id)
        {
            string query =
                "SELECT * FROM Channels WHERE Channel = @id";
            var result = _parent.ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0;
        }
    }

    public class UsersImplementation : IDatabase.IUsers
    {
        private readonly SqLiteDatabase _parent;
        public UsersImplementation(SqLiteDatabase parent) { _parent = parent; }
        
        public void Follow(in ulong id)
        {
            var count = GetMessageCount(id);
            string query =
                "INSERT OR REPLACE INTO Users(User, Follows, Count) VALUES(@id, 1, @count)";
            _parent.ExecuteWrite(query, new Dictionary<string, object>() {{"@id", id}, {"@count", count}});
        }

        public void Unfollow(in ulong id)
        {
            var count = GetMessageCount(id);
            string query =
                "INSERT OR REPLACE INTO Users(User, Follows, Count) VALUES(@id, 0, @count)";
            _parent.ExecuteWrite(query, new Dictionary<string, object>() {{"@id", id}, {"@count", count}});
        }

        public bool IsFollowing(in ulong id)
        {
            string query =
                "SELECT * FROM Users WHERE User = @id";
            var result = _parent.ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0 && (long) result.Rows[0]["Follows"] == 1;
        }

        public long IncreaseMessageCount(in ulong id)
        {
            string query;
            if (!Contains(id))
            {
                query =
                    "INSERT INTO Users(User, Follows) VALUES(@id, 1)";
                _parent.ExecuteWrite(query, "@id", id);
                return 1;
            }

            long count = GetMessageCount(id);
            query =
                "UPDATE Users SET Count = @count WHERE User = @id";
            _parent.ExecuteWrite(query, new Dictionary<string, object>() {{"@id", id}, {"@count", count + 1}});
            return count + 1;
        }

        public long GetMessageCount(in ulong id)
        {
            string query =
                "SELECT * FROM Users WHERE User = @id";
            var result = _parent.ExecuteRead(query, "@id", id);
            if (!(result != null && result.Rows.Count > 0))
                return 0;
            return (long) result.Rows[0]["Count"];
        }

        private bool Contains(in ulong id)
        {
            string query =
                "SELECT * FROM Users WHERE User = @id";
            var result = _parent.ExecuteRead(query, "@id", id);
            return result != null && result.Rows.Count > 0;
        }
    }
}