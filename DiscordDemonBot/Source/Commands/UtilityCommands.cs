using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static DiscordDemonBot.Source.Utils;

namespace DiscordDemonBot.Source.Commands;

public class UtilityCommands: ApplicationCommandModule
{
    [SlashCommand("keysmash", "acabujaikwebgfhsenhgpesjmgolseg")]
    public async Task KeySmashCommand(
        InteractionContext ctx,
        [Option("lettersOnly", "Whether it should only contain latin letters!")] bool lettersOnly = false,
        [Minimum(1)]
        [Option("length", "How long the keysmash should be!")] long length = 20)
    {
        await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder
        {
            Content = Formatter.InlineCode(GenerateKeySmash((int)length, lettersOnly)),
            IsEphemeral = true, // client side reply
        });
    }
}