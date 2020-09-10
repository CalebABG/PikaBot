using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace PikaBot.Commands.Moderation
{
    public class MessagesHandler : IDisposable
    {
        private bool _disposed;
        private DiscordClient _client;


        private static bool _initialized;
        private static MessagesHandler _instance;

        public static MessagesHandler Instance =>
            _instance ?? throw new NullReferenceException($"MessagesHandler Instance is not initialized, " +
                                                          $"call '{nameof(Init)}' to create instance");

        protected MessagesHandler(DiscordClient client)
        {
            _client = client;
        }

        public static void Init(DiscordClient client)
        {
            if (_initialized) return;

            try
            {
                _instance ??= new MessagesHandler(client);
                RegisterEventHandlers();

                _initialized = true;
            }
            catch (Exception e)
            {
                if (_instance != null)
                    UnRegisterEventHandlers();
                
                Console.WriteLine(e);
            }
        }

        private static void RegisterEventHandlers()
        {
            if (Instance?._client == null) return;

            Instance._client.MessageCreated += OnMessageCreated;
            Instance._client.MessageUpdated += OnMessageUpdated;
            Instance._client.MessageDeleted += OnMessageDeleted;
        }
        
        private static void UnRegisterEventHandlers()
        {
            if (Instance?._client == null) return;
            
            Instance._client.MessageCreated -= OnMessageCreated;
            Instance._client.MessageUpdated -= OnMessageUpdated;
            Instance._client.MessageDeleted -= OnMessageDeleted;
        }

        private async static Task OnMessageCreated(MessageCreateEventArgs e)
        {
            
        }

        private static async Task OnMessageUpdated(MessageUpdateEventArgs e)
        {
            if (e.Message.Author.IsCurrent ||
                e.Message.Author.IsBot)
                return;
        }

        private static async Task OnMessageDeleted(MessageDeleteEventArgs e)
        {
            if (e.Message.Author.IsCurrent || 
                e.Message.Content.StartsWith(PikaBot.Instance.CommandPrefix))
                return;
        }

        #region Dispose Pattern

        // Public implementation of Dispose pattern callable by consumers
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects)
                UnRegisterEventHandlers();
            }

            _disposed = true;
        }

        #endregion
    }
}