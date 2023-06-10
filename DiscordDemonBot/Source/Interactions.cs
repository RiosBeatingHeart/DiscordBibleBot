using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscordDemonBot.Source.Roles;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using static DiscordDemonBot.Source.Utils;
using static DiscordDemonBot.Source.Roles.Utils;

#pragma warning disable CS4014 // call async without await

namespace DiscordDemonBot.Source;
public static class EventHandlers
{
    /// <summary>
    /// Adds vote reaction emojis to specific content types.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedVotableContent(DiscordClient client, MessageCreateEventArgs e)
    {
        await Task.Run(async () =>
        {
            string[] textContent = new[]
            {
                "https://www.curseforge.com/minecraft/",
                "https://open.spotify.com/",
                "https://www.youtube.com/",
                "https://www.youtu.be/",
                "https://www.youtu.be/",
            };

            bool hasContent = e.Message.Attachments.Count > 0;
            if(!hasContent)
            {
                // add reactions to images, videos and curseforge links in a specific channel
                foreach (var text in textContent)
                {
                    if (!e.Message.Content.Contains(text)) continue;
                    hasContent = true;
                    break;
                }
            }
            if (!hasContent) return;

            ulong upEmoji = 1023358264617992212;
            ulong downEmoji = 1023358256351019008;

            switch (e.Guild.Id)
            {
                case 953438883859796008: // saturn mc discord server
                    upEmoji = 1028823890815893614;
                    downEmoji = 1028823892216774686;
                    break;
            }
            
            await Console.Out.WriteLineAsync("Adding vote emojis to message!");
            await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client, upEmoji)); // up_penty
            await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client, downEmoji)); // down_penty
        });

    }
    
    /// <summary>
    /// Does a words check if there are more than 5 words in the message.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedCheckable(DiscordClient client, MessageCreateEventArgs e)
    {
        var member = await e.Guild.GetMemberAsync(e.Author.Id);
        var memberRoles = GetDiscordRoles(member.Roles);
        
        if (e.Author.Id != client.CurrentApplication.Id && ( // don't reply to yourself
                memberRoles.ContainsKey(MyRoles.Cultist) || 
                memberRoles.ContainsKey(MyRoles.Demon))) // only reply to cultists
        {
            string[] words = GetWordsInString(e.Message.Content);
            Console.Out.WriteAsync($"words: {string.Join(',', words)}");
            if (words.Length > 5 && !CheckMessage(words))
                await e.Message.RespondAsync("none of these words is in the bible. good! >:3");
        }
    }
    
    /// <summary>
    /// Reacts to replies to the bot's messages.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedReplyBot(DiscordClient client, MessageCreateEventArgs e)
    {
        if (e.Author.Id != client.CurrentApplication.Id && // don't reply to yourself
            e.Message.ReferencedMessage != null &&
            e.Message.ReferencedMessage.Author.Id == client.CurrentApplication.Id) // reply to bot
        {
            await e.Message.RespondAsync("sowwyy, i can't read :<");
        }
    }
    
    /// <summary>
    /// Reacts to replies to the bot's messages.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedMentionBot(DiscordClient client, MessageCreateEventArgs e)
    {
        if (e.Author.Id != client.CurrentApplication.Id && // don't reply to yourself
            (e.Message.ReferencedMessage == null ||
            e.Message.ReferencedMessage.Author.Id != client.CurrentApplication.Id)) // not a reply to bot
        {
            foreach (var user in e.Message.MentionedUsers)
            {
                if (user.Id != client.CurrentApplication.Id) continue;
                
                await e.Message.RespondAsync("ow, you are talking about me? :3");
                return;
            }
        }
    }

    /// <summary>
    /// Function that's called when someone on discord adds a reaction to a message.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The reaction event.</param>
    public static async Task ReactionAdded(DiscordClient client, MessageReactionAddEventArgs e)
    {
        await Task.Run(async () =>
        {
            if (!e.Emoji.Name.Contains("pent") || // penta emoji
                e.Emoji.Id is 1023358256351019008 or 1023358264617992212) // but not the vote emojis
            {
                return;
            }

            e.Channel.TriggerTypingAsync();
            var message = await e.Channel.GetMessageAsync(e.Message.Id);
            if (message.Author.Id == client.CurrentApplication.Id) // reacted to the bot
            {
                
                await new DiscordMessageBuilder()
                    .WithContent($"{Formatter.Mention(e.User, true)} heyy cutie~! ;3")
                    .SendAsync(e.Channel);
                return;
            }

            if (!CheckMessage(message))
                await message.RespondAsync("none of these words is in the bible. good! >:3");
        });
    }
}