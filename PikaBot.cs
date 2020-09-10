using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PikaBot.Commands.Moderation;

namespace PikaBot
{
    public class PikaBot
    {
        public static PikaBot Instance { get; } = new PikaBot();

        private IConfigurationRoot _config;


        #region Properties

        public string CommandPrefix { get; set; } = "!";
        public CancellationTokenSource Cts { get; set; } = new CancellationTokenSource();
        public DiscordClient Client { get; set; }
        public CommandsNextConfiguration CommandsNextConfig { get; set; }

        public InteractivityConfiguration InteractivityConfig;

        #endregion
        

        public async Task RunBotAsync(string[] args)
        {
            try
            {
                _config = new ConfigurationBuilder()
                    .AddUserSecrets(typeof(Program).Assembly, optional: false, reloadOnChange: true)
                    .Build();

                Client = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config["discord:token"],
                    AutoReconnect = true,
                    TokenType = TokenType.Bot,
                    MinimumLogLevel = LogLevel.Debug | LogLevel.Information,
                });

                InteractivityConfig = new InteractivityConfiguration
                {
                    PaginationBehaviour = PaginationBehaviour.WrapAround,
                    Timeout = TimeSpan.FromSeconds(30)
                };

                Client.UseInteractivity(InteractivityConfig);

                var deps = BuildDeps();

                CommandsNextConfig = new CommandsNextConfiguration
                {
                    CaseSensitive = false,
                    StringPrefixes = new List<string>
                    {
                        CommandPrefix,
                    },
                    Services = deps
                };

                Client.UseCommandsNext(CommandsNextConfig);
                
                MessagesHandler.Init(Client);

                RegisterCommandClasses();

                await ConnectAndRunAsync(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void RegisterCommandClasses()
        {
            Client.GetCommandsNext().RegisterCommands(Assembly.GetExecutingAssembly());
        }

        private async Task ConnectAndRunAsync(string[] args)
        {
            Console.WriteLine("Connecting...");
            await Client.ConnectAsync();
            Console.WriteLine("Connected!");

            while (!Cts.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromMinutes(1));
        }

        private IServiceProvider BuildDeps()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(sp => Cts); // Add the cancellation token
            serviceCollection.AddHttpClient(); // Add http client

            var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true,
            });

            return serviceProvider;
        }
    }
}