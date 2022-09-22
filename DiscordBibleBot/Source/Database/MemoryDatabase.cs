using System;
using System.Collections.Generic;

namespace DiscordBibleBot.Source.Database;

public class MemoryDatabase: IDatabase
{
    public IDatabase.IWords Words { get; }
    public IDatabase.IChannels Channels { get; }
    public IDatabase.IUsers Users { get; }

    public MemoryDatabase()
    {
        Words = new WordsImplementation();
        Channels = new ChannelsImplementation();
        Users = new UsersImplementation();
    }

    public IDatabase Connect() => this;

    public class WordsImplementation : IDatabase.IWords
    {
        private readonly HashSet<string> _words;
        public int Count => _words.Count;
        
        public WordsImplementation(){ _words = new HashSet<string>(); }

        public bool Add(in string word) => _words.Add(word);

        public int Add(in IEnumerable<string> words)
        {
            int tmp = Count;
            _words.UnionWith(words);
            return Count - tmp;
        }

        public bool Remove(in string word) => _words.Remove(word);
        
        public int Remove(in IEnumerable<string> words)
        {
            int tmp = Count;
            _words.ExceptWith(words);
            return Count - tmp;
        }

        public bool Contains(in string word) => _words.Contains(word);

        public bool Contains(in IEnumerable<string> words)
        {
            foreach (var word in words)
                if (_words.Contains(word)) return true;
            return false;
        }
    }

    public class ChannelsImplementation : IDatabase.IChannels
    {
        private HashSet<ulong> _channels;

        public ChannelsImplementation()
        {
            _channels = new HashSet<ulong>();
        }

        public bool Join(in ulong id) => _channels.Add(id);

        public bool Leave(in ulong id) => _channels.Remove(id);

        public bool IsJoined(in ulong id) => _channels.Contains(id);
    }
    
    public class UsersImplementation : IDatabase.IUsers
    {
        private Dictionary<ulong, Tuple<bool, long>> _users;
        
        public UsersImplementation() { _users = new Dictionary<ulong, Tuple<bool, long>>(); }
        
        public void Follow(in ulong id)
        {
            _users[id] = _users.TryGetValue(id, out var entry)
                ? new Tuple<bool, long>(true, entry.Item2)
                : new Tuple<bool, long>(true, 0);
        }

        public void Unfollow(in ulong id)
        {
            _users[id] = _users.TryGetValue(id, out var entry)
                ? new Tuple<bool, long>(false, entry.Item2)
                : new Tuple<bool, long>(false, 0);
        }

        public bool IsFollowing(in ulong id) => _users.TryGetValue(id, out var entry) && entry.Item1;

        public long IncreaseMessageCount(in ulong id)
        {
            if (_users.TryGetValue(id, out var entry))
            {
                _users[id] = new Tuple<bool, long>(entry.Item1, entry.Item2 + 1);
                return entry.Item2 + 1;
            }
            _users[id] = new Tuple<bool, long>(false, 1);
            return 1;
        }

        public long GetMessageCount(in ulong id) => _users.TryGetValue(id, out var entry) ? entry.Item2 : 0;
    }
}

