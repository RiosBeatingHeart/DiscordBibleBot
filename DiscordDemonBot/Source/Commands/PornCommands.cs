using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static DiscordDemonBot.Source.Roles.Utils;
using R34Sharp;

namespace DiscordDemonBot.Source.Commands;


public class PornCommands : ApplicationCommandModule
{
    private static readonly R34ApiClient R34Client = new ();
    
    private static readonly R34TagModel[] ExcludedTags = {
        new("video"),
        new("animated"),
    };
    
    private static readonly string[] DemonComments = new[]
    {
        "oww, i like your taste! have fun, cutie~! :3",
        "such a slut.. u would make a good demon~! >:3",
        "okii, here u go~! :3",
        "ohmy >w< me and who?",
        "/k",
        "/k"
    };
    
    
    private static string GetRandomDemonComment()
    {
        if (DemonComments.Length <= 0)
            throw new DataException("No success messages set!");
        var rnd = new Random();
        int index = rnd.Next(DemonComments.Length);
        if (!DemonComments[index].Contains("/k"))
            return DemonComments[index];
        return Utils.GenerateKeySmash(20, true);
    }
    

    [SlashCommand("porn", "Request a gift from your satanic lord!")]
    public async Task PornCommand(
        InteractionContext ctx,
        [Option("Tags", "R34 post tags! (seperated by ',')")] string tags)
    {

        // need to do this or the event gets wasted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        // TODO add role restriction
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        // if (!memberRoles.ContainsKey(MyRoles.Cultist) && !memberRoles.ContainsKey(MyRoles.Demon))
        // {
        //     await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("that's for cute cult members only~! wanna join? :3"));
        //     return;
        // }
        
        // don't post porn to non nsfw channels
        if (!ctx.Channel.IsNSFW)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("oh, you know i can't send porn in here! ;3"));
            return;
        }
        
        
        string[] queryTags = tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
        

        // logging
        await Console.Out.WriteLineAsync($"Try sending porn with tags <{string.Join(", ", tags)}> to channel <{ctx.Channel.Name}> for user <{ctx.Member.DisplayName}>.");

        Task<R34Posts> getPostTask = R34Client.Posts.GetPostsAsync(new R34PostsSearchBuilder 
        {
            Limit = 1000,
            Tags = queryTags.Select(tag => new R34TagModel(tag)),
            BlockedTags = new R34Sharp.Optional<IEnumerable<R34TagModel>>(ExcludedTags),
        });
        
        var posts = await getPostTask;
        
        if (posts.Count == 0) // no posts found matching the tags
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("sowwyy, i couldn't find what you are looking for.. :<"));
            return;
        }

        Random rnd = new Random();
        R34Post rndPost = posts.Data[rnd.Next(0, (int)posts.Count)];
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent(GetRandomDemonComment())
            .AddEmbed(new DiscordEmbedBuilder
            {
                Title = "r34",
                Url = rndPost.FileUrl,
                ImageUrl = rndPost.FileUrl,
                Description = $"tags: {Formatter.Strip(string.Join(", ", queryTags))}",
            }.Build()));
    }
}