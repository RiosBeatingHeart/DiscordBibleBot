using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBibleBot.Source
{
    public static class EventHandlers
    {
        /// <summary>
        /// Function that's called when someone on discord writes a message.
        /// </summary>
        /// <param name="client">The discord client.</param>
        /// <param name="e">The message event.</param>
        public static async Task HandleMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            if (e.Author.Id != client.CurrentApplication.Id && // don't reply to yourself
                (Bot.Instance.Database.ChannelsJoined(e.Channel.Id) || 
                 (Bot.Instance.Database.UsersFollows(e.Author.Id))))
            {
                await CheckMessage(e.Message);
            }
            
            // add reactions to images, videos and curseforge links in a specific channel
            if (e.Channel.Id == 963739089549533245 && 
                e.Message.Content.Contains("https://www.curseforge.com/minecraft/") ||
                e.Message.Content.Contains("https://open.spotify.com/") ||
                e.Message.Attachments.Count > 0)
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client,964881658182529064)); // cross_up
                await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client,964881919126945853)); // cross_down
            }
        }
        
        /// <summary>
        /// Function that's called when someone on discord adds a reaction to a message.
        /// </summary>
        /// <param name="client">The discord client.</param>
        /// <param name="e">The reaction event.</param>
        public static async Task HandleReactionAdded(DiscordClient client, MessageReactionAddEventArgs e)
        {
            if (e.Emoji.Name == Bot.Icon)
            {
                var message = await e.Channel.GetMessageAsync(e.Message.Id);
                if (message.Author.Id == client.CurrentApplication.Id)
                {
                    if (!Bot.Instance.Database.UsersFollows(e.User.Id))
                    {
                        Bot.Instance.Database.UsersFollow(e.User.Id);
                        await new DiscordMessageBuilder()
                            .WithContent($"<@{e.User.Id}> You embraced God!")
                            .SendAsync(e.Channel);
                    }
                    else
                    {
                        string fileName = new Random().NextDouble() > 0.5 ? "cum_preacher.mp4" : "cum_jesus.mp4";
                        await using var file = File.OpenRead(Path.Join(Definitions.ResourceDir, @"video", fileName));

                        await new DiscordMessageBuilder()
                            .WithContent($"<@{e.User.Id}> I heard your prayer and I haven't forgotten about you!")
                            .WithFile(file)
                            .SendAsync(e.Channel);
                    }
                }
                else
                {
                    if(!await CheckMessage(message))
                        await message.RespondAsync("At least one of those words is in the Bible!");
                }
            }
        }
        
        private static async Task<bool> CheckMessage(DiscordMessage message)
        {
            string content = message.Content;

            switch (message.Author.Id)
            {
                case 957457743126614067: // 196 survival server bot
                    content = content.Substring(content.IndexOf('>'));
                    break;
            }
            
            var words = new HashSet<string>();

            if (message.Content.Count(char.IsLetter) >= Bot.Instance.MinLetterLimit)
            {
                words.UnionWith(Utils.GetWordsInString(content));

                if (!Bot.Instance.Database.WordsContain(words))
                {
                    Bot.Instance.Database.UsersUpCount(message.Author.Id);
                    await message.RespondAsync("None Of These Words Are In The Bible!");
                }
                else
                    return false;
            }
            return true;
        }
    }
}