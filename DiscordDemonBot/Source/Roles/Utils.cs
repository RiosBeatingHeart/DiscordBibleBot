using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DiscordDemonBot.Source.Roles;

public static class Utils
{
    public static async Task AddMissingRolesToGuild(DiscordGuild guild)
    {
        // gets the roles the guild is missing
        var guildRoles = GetDiscordRoles(guild.Roles);
        
        Role[] missing = MyRoles.Enumerable
            .Where(role => !guildRoles.ContainsKey(role))
            .ToArray();
        
        foreach (var role in missing)
            await Console.Out.WriteLineAsync($"Role <{role.Name}> is missing form guild <{role.Name}>. Try Adding!");

        // adds the missing roles to the guild
        await Task.WhenAll(missing.Select(role => AddRoleToGuild(guild, role)));
    }
    
    private static async Task AddRoleToGuild(DiscordGuild guild, Role role)
        => await guild.CreateRoleAsync(
            role.Name,
            role.Permissions,
            role.Color,
            role.Hoist,
            role.Mentionable,
            role.Reason,
            role.Icon,
            role.Emoji);

    /// <summary>
    /// Gets discord roles corresponding to my custom roles.
    /// </summary>
    /// <param name="roles">Dict of discord roles./></param>
    /// <returns>
    /// A dict with the id:s of the custom roles as keys and the DiscordRoles as values.
    /// If no role was found for an id, the value will be null.
    /// </returns>
    public static IDictionary<Role, DiscordRole> GetDiscordRoles(IReadOnlyDictionary<ulong, DiscordRole> roles)
        => GetDiscordRoles(roles.Values);
    
    /// <summary>
    /// Gets discord roles corresponding to my custom roles.
    /// </summary>
    /// <param name="roles">List of discord roles./></param>
    /// <returns>
    /// A dict with the id:s of the custom roles as keys and the DiscordRoles as values.
    /// </returns>
    public static IDictionary<Role, DiscordRole> GetDiscordRoles(IEnumerable<DiscordRole> roles)
    {
        Dictionary<Role, DiscordRole> ret = new();

        foreach (var guildRole in roles)
        {
            if (!MyRoles.TryGetRole(guildRole.Name, out var myRole)) continue;
            ret[(Role) myRole!] = guildRole;
        }
        
        return ret;
    }
}