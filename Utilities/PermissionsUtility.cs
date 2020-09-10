using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;

namespace PikaBot.Utilities
{
    public static class PermissionsUtility
    {
        public static bool IsMod(this DiscordMember member) =>
            member.Roles.Any(role => role.Permissions.HasPermission(Permissions.KickMembers));

        public static bool IsAdmin(this DiscordMember member) =>
            member.Roles.Any(role => role.Permissions.HasPermission(Permissions.Administrator));

        public static bool HasAnyRoles(this DiscordMember member)
            => member.Roles.Any();

        public static bool HasPermission(this DiscordRole role, Permissions permission) =>
            role.Permissions.HasPermission(permission);

        public static bool HasPermission(this DiscordMember member, Permissions permission)
        {
            return !member.HasAnyRoles()
                ? member.Guild.EveryoneRole.HasPermission(permission)
                : member.Roles.Any(role => role.HasPermission(permission));
        }
    }
}