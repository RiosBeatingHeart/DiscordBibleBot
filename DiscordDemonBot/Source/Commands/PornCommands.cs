using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BooruSharp.Search;
using DiscordDemonBot.Source.Roles;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static DiscordDemonBot.Source.Roles.Utils;
#pragma warning disable CS4014

namespace DiscordDemonBot.Source.Commands;

public class PornCommands : ApplicationCommandModule
{
    public enum BooruWebsite
    {
        [ChoiceName("r34")]
        R34,
        [ChoiceName("e621")]
        E621,
    }

    private static readonly string[] SuccessMessages = new[]
    {
        "oww, i like your taste! have fun, cutie~! :3",
        "such a slut.. u would make a good demon~! >:3",
        "okii, here u go~! :3",
        "ohmy >w< me and who?",
        "/k",
        "/k"
    };

    private static string GetSuccessMessage()
    {
        if (SuccessMessages.Length <= 0)
            throw new DataException("No success messages set!");
        var rnd = new Random();
        int index = rnd.Next(SuccessMessages.Length);
        if (!SuccessMessages[index].Contains("/k"))
            return SuccessMessages[index];
        return Utils.GenerateKeySmash(20, true);
    }
    
    [SlashCommand("porn", "Request a gift from your satanic lord!")]
    public async Task PornCommand(
        InteractionContext ctx,
        [Option("website", "Website to get posts from!")] BooruWebsite website,
        [Option("tags", "Porn post tags! Separate by ','")] string tags)
    {
        // need to do this or the event gets wasted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        if (!memberRoles.ContainsKey(MyRoles.Cultist) && !memberRoles.ContainsKey(MyRoles.Demon))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("that's for cute cult members only~! wanna join? :3"));
            return;
        }
        
        // don't post porn to non nsfw channels
        if (!ctx.Channel.IsNSFW)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("oh, you know i can't send porn in here! ;3"));
            return;
        }

        // split tags up
        string[] queryTags = tags.Split(',', StringSplitOptions.TrimEntries).Distinct().ToArray();

        await Console.Out.WriteLineAsync($"Try sending porn with tags <{string.Join(", ", queryTags)}> to channel <{ctx.Channel.Name}> for user <{ctx.Member.DisplayName}>.");
        
        Task<BooruSharp.Search.Post.SearchResult> getPostTask;

        switch (website)
        {
            case BooruWebsite.R34:
                getPostTask = new BooruSharp.Booru.Rule34().GetRandomPostAsync(queryTags);
                break;

            case BooruWebsite.E621:
                getPostTask = new BooruSharp.Booru.E621().GetRandomPostAsync(queryTags);
                break;
            
            default:
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("waaa something went wrongg ;w;"));
                return;
        }

        const int maxTries = 5;
        try
        {
            for (int i = 0; i < maxTries; i++)
            {
                var result = await getPostTask;
                if (result.FileUrl == null) continue;
                
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent(GetSuccessMessage())
                    .AddEmbed(new DiscordEmbedBuilder
                    {
                        Title = website.GetName(),
                        Url = result.FileUrl.AbsoluteUri,
                        ImageUrl = result.FileUrl.AbsoluteUri,
                        Description = $"tags: {Formatter.Strip(string.Join(", ", queryTags))}",
                    }.Build()));
                return;
            }
        }
        catch (InvalidTags)
        {
            Console.Out.WriteLineAsync($"Couldn't get porn for tags <{string.Join(", ", queryTags)}>.");
        }
        catch (Exception e)
        {
            Console.Out.WriteLineAsync($"Unexpected exception in /porn command. Exception: {e}");
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("sowwyy, i couldn't find what you are looking for.. :<"));
    }
}