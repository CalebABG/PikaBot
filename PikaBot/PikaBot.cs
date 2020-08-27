using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;

namespace PikaBot
{
    public class PikaBot
    {
        private const string PikaBotDiscordCommandPrefix = "!";
        
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private IConfigurationRoot _config;
        private DiscordClient _discord;
        private CommandsNextModule _commands;
        private InteractivityModule _interactivity;
        private HttpClient _httpClient;

        public async Task InitBot(string[] args)
        {
            try
            {
                _config = new ConfigurationBuilder()
                    .AddUserSecrets(typeof(Program).Assembly, optional: false, reloadOnChange: true)
                    .Build();

                _discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config["discord:token"],
                    AutoReconnect = true,
                    TokenType = TokenType.Bot,
                    LogLevel = LogLevel.Debug,
                    UseInternalLogHandler = true
                });

                _interactivity = _discord.UseInteractivity(new InteractivityConfiguration
                {
                    PaginationBehaviour = TimeoutBehaviour.Delete,
                    PaginationTimeout = TimeSpan.FromSeconds(30),
                    Timeout = TimeSpan.FromSeconds(30)
                });
                
                _httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30),
                };

                var deps = BuildDeps();
                _commands = _discord.UseCommandsNext(new CommandsNextConfiguration
                {
                    StringPrefix = PikaBotDiscordCommandPrefix,
                    Dependencies = deps
                });

                RegisterCommandClasses();

                await RunAsync(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void RegisterCommandClasses()
        {
            var types = GetTypes();
            foreach (var t in types)
                _commands.RegisterCommands(t);
        }

        private Type[] GetTypes()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IRegisterCommandsClass).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            return types as Type[] ?? types.ToArray();
        }

        private async Task RunAsync(string[] args)
        {
            Console.WriteLine("Connecting...");
            await _discord.ConnectAsync();
            Console.WriteLine("Connected!");

            while (!_cts.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromMinutes(1));
        }

        private DependencyCollection BuildDeps()
        {
            using var deps = new DependencyCollectionBuilder();

            deps.AddInstance(_interactivity) // Add interactivity
                .AddInstance(_cts) // Add the cancellation token
                .AddInstance(_config) // Add our config
                .AddInstance(_httpClient) // Add http client
                .AddInstance(_discord); // Add the discord client

            return deps.Build();
        }
    }
}