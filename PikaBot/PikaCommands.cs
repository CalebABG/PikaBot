#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace PikaBot
{
    public class PikaCommands : IRegisterCommandsClass
    {
        [Command("fact")]
        [Description("Gets a random fact or the fact of the day!")]
        public async Task Fact(CommandContext context, string verb = "random")
        {
            await context.TriggerTypingAsync();

            try
            {
                string url = $"https://uselessfacts.jsph.pl/{(verb == "today" ? verb : "random")}.json?language=en";
                var randomFact = await GetFact(url, context.Dependencies.GetDependency<HttpClient>());
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor(0xB29830))
                    .WithFooter("Facts from: 'https://uselessfacts.jsph.pl'")
                    .AddField("Fact 🤔", randomFact, false));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<string?> GetFact(string url, HttpClient client)
        {
            string? fact = null;
            
            var todayResponse = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead);
            if (!todayResponse.IsSuccessStatusCode) 
                return fact;

            var responseData = await todayResponse.Content.ReadAsStringAsync();
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData);

            fact = responseDictionary["text"];

            return fact;
        }

        [Command("greet")]
        public async Task Greet(CommandContext context, string name, string verb = "channel")
        {
            await context.TriggerTypingAsync();

            var member = context.Channel.Guild.Members.FirstOrDefault(m => CheckName(m, name));

            if (member != null)
            {
                verb = verb.ToLower();

                string errorMsg = "Sorry :( - couldn't do that for ya. Woof!";

                string greetingBase = "Greetings ";
                string greeting = $"{greetingBase}, {member.DisplayName}!";

                try
                {
                    switch (verb)
                    {
                        case "dm":
                            await member.SendMessageAsync(greeting);
                            await context.Channel.SendMessageAsync("Sent greeting. Woof!");
                            break;

                        case "channel":
                            await context.RespondAsync(greeting);
                            break;
                    }
                }
                catch (Exception e)
                {
                    await context.RespondAsync(errorMsg);
                    Console.WriteLine(e);
                }
            }
        }

        private bool CheckName(DiscordMember member, string name)
        {
            name = name.ToLower();

            var dispName = member.DisplayName.ToLower();
            var usrName = member.Username.ToLower();

            if (name.Equals(dispName) ||
                name.Equals(usrName))
            {
                return true;
            }

            if (IsMention(name, out var id))
            {
                if (member.Id == id)
                    return true;
            }

            return false;
        }

        private bool IsMention(string str, out ulong id)
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

            // Failure case
            id = 0;
            return false;
        }
    }
}