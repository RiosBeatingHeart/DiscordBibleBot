using System.ComponentModel;
using DSharpPlus.CommandsNext;
using Attributes = DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OpenAI_API;

namespace DiscordBibleBot.Source
{
    [Attributes.Description("Prayers to the almighty God!")]
    public class Commands : BaseCommandModule
    {
        [Command("join"), Attributes.Description("Embrace God's love in a channel!")]
        public async Task JoinCommand(CommandContext ctx)
        {
            await JoinCommand(ctx, ctx.Channel);
        }
        
        [Command("join"), Attributes.Description("Embrace God's love in a channel!")]
        public async Task JoinCommand(CommandContext ctx, DiscordChannel channel)
        {
            if (Bot.Instance.Database.ChannelsJoin(channel.Id))
                await ctx.RespondAsync("God blesses this channel!");
            else
                await ctx.RespondAsync("This channel does already have God's blessing!");
        }
        
        [Command("leave"), Attributes.Description("Ban God from a channel!")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            await LeaveCommand(ctx, ctx.Channel);
        }
        
        [Command("leave"), Attributes.Description("Ban God from a channel!")]
        public async Task LeaveCommand(CommandContext ctx, DiscordChannel channel)
        {
            if (Bot.Instance.Database.ChannelsLeave(channel.Id))
                await ctx.RespondAsync("I CURSE THIS GODLESS PLACE!!!");
        }

        [Command("follow"), Attributes.Description("Become a devout follower of God!")]
        public async Task FollowCommand(CommandContext ctx)
        {
            if (!Bot.Instance.Database.UsersFollows(ctx.User.Id))
            {
                Bot.Instance.Database.UsersFollow(ctx.User.Id);
                await ctx.RespondAsync("You embraced God!");
            }
            else
            {
                await ctx.RespondAsync("I have not forgotten about you, my child!");
            }
        }
        
        [Command("unfollow"), Attributes.Description("Turn your back on your Lord and Saviour!")]
        public async Task UnfollowCommand(CommandContext ctx)
        {
            if (Bot.Instance.Database.UsersFollows(ctx.User.Id))
            {
                Bot.Instance.Database.UsersUnfollow(ctx.User.Id);
                await ctx.RespondAsync("YOU TURNED YOUR BACK ON YOUR LORD! FOR THIS YOU SHALL BURN IN HELL!!");
            }
        }

        [Command("cum"), Attributes.Description("Come to God!")]
        public async Task CumCommand(CommandContext ctx)
        {
            string fileName = new Random().NextDouble() > 0.5 ? "cum_preacher.mp4" : "cum_jesus.mp4";
            await using var file = File.OpenRead(Path.Join(Definitions.ResourceDir, @"video", fileName));
            
            await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(file));
        }
        
        [Command("ai"), Attributes.Description("Talk to God!")]
        public async Task AiCommand(CommandContext ctx, [RemainingText] string prompt)
        {
            var result = await Bot.Instance.OpenAiApi.Completions.CreateCompletionAsync(
                new CompletionRequest(
                    prompt,
                    max_tokens: 200, 
                    0.1));
            await Console.Out.WriteLineAsync($"Prayer triggered with request '{prompt}'!");
            await Console.Out.WriteLineAsync($"Response: '{result}'!");
            await ctx.RespondAsync($"```{result}```");
        }
    }
}