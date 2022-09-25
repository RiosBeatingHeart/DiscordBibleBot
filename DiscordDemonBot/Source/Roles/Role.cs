using System.IO;
using DSharpPlus.Entities;

namespace DiscordDemonBot.Source.Roles;

public struct Role
{
    /// <summary>Name of the role.</summary>
    public string Name { get; set; }

    /// <summary>Permissions for the role.</summary>
    public DSharpPlus.Permissions? Permissions { get; set; }

    /// <summary>Color for the role.</summary>
    public DiscordColor? Color { get; set; }

    /// <summary>Whether the role is to be hoisted.</summary>
    public bool? Hoist { get; set; }

    /// <summary>Whether the role is to be mentionable.</summary>
    public bool? Mentionable { get; set; }

    /// <summary>Reason for audit logs.</summary>
    public string? Reason { get; set; }

    /// <summary>The icon to add to this role.</summary>
    public Stream? Icon { get; set; }

    /// <summary>he emoji to add to this role. Must be unicode.</summary>
    public DiscordEmoji? Emoji { get; set; }
}