using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BooruSharp.Search;
using BooruSharp.Search.Wiki;
using DiscordDemonBot.Source.Roles;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using OpenAI_API;
using static DiscordDemonBot.Source.Roles.Utils;

#pragma warning disable CS4014

namespace DiscordDemonBot.Source.Commands;

public class BasicCommands : ApplicationCommandModule
{
    [SlashCommand("summon", "Summon a demon!")]
    public async Task SummonCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await AddMissingRolesToGuild(ctx.Guild);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("hii cutie~! :3 you summoned me!?"));
    }

    [SlashCommand("sacrifice", "Sacrifice someone to satan!")]
    public async Task SacrificeUserCommand(
        InteractionContext ctx, 
        [Option("user", "User to sacrifice!")] DiscordUser sacrificedUser)
    {
        // need to do this or the event gets wasted
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        
        var sacrificedMember = await ctx.Guild.GetMemberAsync(sacrificedUser.Id);
        await Console.Out.WriteLineAsync($"User <{ctx.Member.DisplayName}> tries to sacrifice <{sacrificedMember.DisplayName}>!");
        
        await AddMissingRolesToGuild(ctx.Guild);
        var guildRoles = GetDiscordRoles(ctx.Guild.Roles);
        var memberRoles = GetDiscordRoles(ctx.Member.Roles);
        
        // tried to sacrifice the bot
        if (sacrificedMember.Id == Bot.Instance.DiscordClient.CurrentApplication.Id)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("oh c'mon, don't sacrifice me! >:("));
            return;
        }

        if (sacrificedMember.IsBot)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{Formatter.Mention(sacrificedUser, true)} is a mere machine, satan has no use for it ;3"));
            return;
        }
        
        // sacrificed someone else
        if (ctx.Member != sacrificedMember)
        {
            if (!memberRoles.ContainsKey(MyRoles.Cultist) && !memberRoles.ContainsKey(MyRoles.Demon))
            {
                await Task.WhenAll(
                    ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Sacrificed],
                        $"Sacrificed {sacrificedMember.DisplayName} to satan!"),
                    ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Cultist],
                        $"Sacrificed {sacrificedMember.DisplayName} to satan!"));
            }
            
            await Task.WhenAll(
                sacrificedMember.RevokeRoleAsync(guildRoles[MyRoles.Demon], $"Was sacrificed by {ctx.Member.DisplayName}!"),
                sacrificedMember.RevokeRoleAsync(guildRoles[MyRoles.Cultist], $"Was sacrificed by {ctx.Member.DisplayName}!"),
                sacrificedMember.GrantRoleAsync(guildRoles[MyRoles.Sacrificed], $"Was sacrificed by {ctx.Member.DisplayName}!"));
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"woaah, you sacrificed <@{sacrificedMember.Id}>! satan is going to like this~! :3"));
            return;
        }
        
        // sacrificed themselves
        await Task.WhenAll(
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], $"Sacrificed themselves to satan!"),
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Sacrificed], $"Sacrificed themselves to satan!"),
            ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Demon], $"Sacrificed themselves to satan!"));
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("you sacrificed yourself!? i wonder what that did.."));
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
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("you can't join the cult twice, silly~! ;3"));
            return;
        }
        // member not yet in cult
        await ctx.Member.GrantRoleAsync(guildRoles[MyRoles.Cultist], "Joined the cult!");
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("another servant of satan joined the cult! >:3"));
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
                .WithContent("you aren't in the cult, silly.. would you like to join tho~? ;3"));
            return;
        }

        await Task.WhenAll(
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Cultist], "Left the cult!"),
            ctx.Member.RevokeRoleAsync(guildRoles[MyRoles.Demon], "Left the cult!"));
        
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("such a shame. i really liked you~! ;3\n" +
                         "..\n" +
                         "*murders you*"));
    }
}