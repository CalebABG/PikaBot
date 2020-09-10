using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PikaBot.Utilities;

namespace PikaBot.Commands.General
{
    public class GeneralCommand : BaseCommandModule
    {
        [Command("greet")]
        [Description("Ask the bot to greet a member of the Channel!")]
        public async Task Greet(CommandContext context, string name, string whereToSend = "channel")
        {
            await context.TriggerTypingAsync();

            var currentMember = context.Member;

            var valuePair = context.Channel.Guild.Members
                .FirstOrDefault(m =>
                    GeneralUtility.MemberNameMatches(m.Value, name.ToLower()));

            var member = valuePair.Value;

            if (member != null)
            {
                whereToSend = whereToSend.ToLower();

                string errorMsg = "Sorry :( - couldn't do that for ya. Woof!";

                string greetingBase = "Greetings ";
                string greeting = $"{greetingBase}, {member.DisplayName}!";

                try
                {
                    switch (whereToSend)
                    {
                        // Todo: check if the member is an admin or mod, only then can they dm
                        case "dm":
                        {
                            if (!currentMember.IsAdmin() || !currentMember.IsMod())
                            {
                                await context.Channel.SendMessageAsync("Sorry, only Moderators and Admins can send DMs. Woof!");
                                return;
                            }
                            
                            await member.SendMessageAsync(greeting);
                            await context.Channel.SendMessageAsync("Sent greeting. Woof!");
                            break;
                        }
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
            else
            {
                await context.Channel.SendMessageAsync(
                    $"Sorry, I couldn't find anyone in the channel with the name given. Woof!");
            }
        }
    }
}