using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using static DiscordBibleBot.Source.Utils;

#pragma warning disable CS4014 // call async without await

namespace DiscordBibleBot.Source;
public static class EventHandlers
{
    /// <summary>
    /// Adds vote reaction emojis to specific content types.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedVotableContent(DiscordClient client, MessageCreateEventArgs e)
    {
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
    /// Does a words check if there are more than 5 words in the message.
    /// </summary>
    /// <param name="client">The discord client.</param>
    /// <param name="e">The message event.</param>
    public static async Task MessageAddedCheckable(DiscordClient client, MessageCreateEventArgs e)
    {
        if (e.Author.Id != client.CurrentApplication.Id) // don't reply to yourself
        {
            var words = (string[])GetWordsInString(e.Message.Content);
            if (words.Length > 5 && !CheckMessage(words))
                await e.Message.RespondAsync("None of these words is in the bible. Good! >:3");
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
            await e.Message.RespondAsync("Sowwyy, i can't read :<");
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
                
                await e.Message.RespondAsync("Ow, you are talking about me? :3");
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
        if (!e.Emoji.Name.Contains("penta")) return;

        e.Channel.TriggerTypingAsync();
        var message = await e.Channel.GetMessageAsync(e.Message.Id);
        if (message.Author.Id == client.CurrentApplication.Id) // reacted to the bot
        {
            await new DiscordMessageBuilder()
                .WithContent($"<@{e.User.Id}> Heyy cutie~! ;3")
                .SendAsync(e.Channel);
            return;
        }

        if (!CheckMessage(message))
            await message.RespondAsync("None of these words is in the bible. Good! >:3");
    }
}