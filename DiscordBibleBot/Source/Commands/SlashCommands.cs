using System;
using System.Threading.Tasks;
using DiscordBibleBot.Source.Roles;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static DiscordBibleBot.Source.Roles.Utils;

namespace DiscordBibleBot.Source.Commands;

public class SlashCommands : ApplicationCommandModule
{
    [SlashCommand("summon", "Summon a demon!")]
    public async Task SummonCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync("Hii cutie~! :3 You summoned me!?");
    }

    [SlashCommand("sacrifice", "Sacrifice someone to satan!")]
    public async Task SacrificeCommand(
        InteractionContext ctx, 
        [Option("user", "User to sacrifice!")] DiscordUser sacrificedUser)
    {
        // need to do this or the event gets waisted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        var sacrificedMember = await ctx.Guild.GetMemberAsync(sacrificedUser.Id);
        await Console.Out.WriteLineAsync($"User <{ctx.Member.DisplayName}> try sacrifice <{sacrificedMember.DisplayName}>!");
        
        await AddMissingRolesToGuild(ctx.Guild);
        var guildRoles = GetDiscordRoles(ctx.Guild.Roles);
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        
        // tried to sacrifice the bot
        if (sacrificedMember.Id == Bot.Instance.DiscordClient.CurrentApplication.Id)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Oh c'mon, don't sacrifice me! >:("));
            return;
        }
        
        // sacrificed someone else
        if (ctx.Member != sacrificedMember)
        {
            if (memberRoles.ContainsKey(MyRoles.Cultist) || memberRoles.ContainsKey(MyRoles.Demon))
                await ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Cultist], $"Sacrificed {sacrificedMember.DisplayName} to satan!");
            
            await Task.WhenAll(
                ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Demon], $"Was sacrificed by {ctx.Member.DisplayName}!"),
                ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], $"Was sacrificed by {ctx.Member.DisplayName}!"),
                ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Sacrificed], $"Was sacrificed by {ctx.Member.DisplayName}!"));
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Thank you for your sacrifice, cute human~!"));
            return;
        }
        
        // sacrificed themselves
        await Task.WhenAll(
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], $"Sacrificed themselves to satan!"),
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], $"Sacrificed themselves to satan!"),
            ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Demon], $"Sacrificed themselves to satan!"));
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Wow, you sacrificed yourself! I wonder what that did.."));
    }

    [SlashCommand("join", "Join a satanist cult!")]
    public async Task JoinCommand(InteractionContext ctx)
    {
        // need to do this or the event gets wasted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await AddMissingRolesToGuild(ctx.Guild);
        var guildRoles = GetDiscordRoles(ctx.Guild.Roles);
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        
        // member is already in the cult
        if (memberRoles.ContainsKey(MyRoles.Cultist) || memberRoles.ContainsKey(MyRoles.Demon))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You can't join the cult twice, silly~! ;3"));
            return;
        }
        // member not yet in cult
        await Task.WhenAll(
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Sacrificed], "Joined the cult!"),
            ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Cultist], "Joined the cult!"));
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Another servant of satan joined the cult! >:3"));
    }

    [SlashCommand("leave", "Leave the cult!")]
    public async Task LeaveCommand(InteractionContext ctx)
    {
        // need to do this or the event gets wasted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await AddMissingRolesToGuild(ctx.Guild);
        var guildRoles = GetDiscordRoles(ctx.Guild.Roles);
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        
        if (!memberRoles.ContainsKey(MyRoles.Cultist) && !memberRoles.ContainsKey(MyRoles.Demon))
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("You aren't in the cult, silly.. Would you like to join tho~? ;3"));
            return;
        }

        await Task.WhenAll(
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], "Left the cult!"),
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Demon], "Left the cult!"));
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Such a shame. I really liked you~! ;3\n" +
                         "..\n" +
                         "*murders you*"));
    }
}