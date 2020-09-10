#nullable enable
using System.Text.RegularExpressions;
using DSharpPlus.Entities;

namespace PikaBot.Utilities
{
    public static class GeneralUtility
    {
        public static bool IsMention(string str, out ulong id)
        {
            var pattern = @"^<@(?<msgSyntax>[!#&]?)(?<memberId>\d+)>$";

            var r = new Regex(pattern);
            var m = r.Match(str);

            if (m.Success)
            {
                string mid = m.Groups["memberId"].Value;
                id = ulong.Parse(mid);
                return true;
            }

            id = 0;
            return false;
        }

        public static bool MemberNameMatches(DiscordMember? member, string name)
        {
            if (member == null) return false;

            var displayName = member.DisplayName.ToLower();
            var usrName = member.Username.ToLower();

            if (name.Equals(displayName) || name.Equals(usrName)) return true;

            if (!IsMention(name, out var id)) return false;

            return member.Id == id;
        }
    }
}