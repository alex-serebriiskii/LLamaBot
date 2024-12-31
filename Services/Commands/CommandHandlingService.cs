using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
namespace SchizoLlamaBot.Services.Commands
{
    public class CommandHandlingService
    {
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlingService(
            CommandService commandService,
            DiscordSocketClient client,
            IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _client = client;
            _serviceProvider = serviceProvider;
            _client.MessageReceived += HandleMessageAsync;
        }

        public async Task InitializeAsync()
        {
            //_logger.LogInformation("Initialzing Command Handler");
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(),_serviceProvider);
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            // Don't process the command if it was a system message
            var message = socketMessage as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            var isCommand = message.HasCharPrefix('!', ref argPos);

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            var r = await _commandService.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: _serviceProvider);

            if(r.IsSuccess)
            {Console.WriteLine("nice");}
            else
            {Console.WriteLine("Not nice");}
        }
    }
}