#nullable enable
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace PikaBot.Commands.General
{
    public class FactCommand : BaseCommandModule
    {
        private HttpClient? _httpClient;

        [Command("fact")]
        [Description("Gets a random fact or the fact of the day!")]
        public async Task Fact(CommandContext context, string verb = "random")
        {
            await context.TriggerTypingAsync();

            try
            {
                string url = $"https://uselessfacts.jsph.pl/{(verb == "today" ? verb : "random")}.json?language=en";
                var randomFact = await GetFact(url, context.Services.GetService<IHttpClientFactory>());
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

        private async Task<string?> GetFact(string url, IHttpClientFactory httpClientFactory)
        {
            string? fact = null;

            if (_httpClient == null)
            {
                _httpClient = httpClientFactory.CreateClient();
                _httpClient.Timeout = TimeSpan.FromMinutes(1);
            }

            var todayResponse = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
            if (!todayResponse.IsSuccessStatusCode)
                return fact;

            var responseData = await todayResponse.Content.ReadAsStringAsync();
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData);

            fact = responseDictionary["text"] as string;

            return fact;
        }
    }
}