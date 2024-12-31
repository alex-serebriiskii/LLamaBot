using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.Logging;

public class LoggingService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _clientLogger;
    private readonly ILogger _commandLogger;
    public LoggingService(
        DiscordSocketClient client,
        CommandService commandService,
        ILoggerFactory loggerFactory)
    {
        _client = client;
        _commandService = commandService;
        _loggerFactory = loggerFactory;

        _clientLogger = _loggerFactory.CreateLogger("client");
        _commandLogger = _loggerFactory.CreateLogger("commands");

        _client.Log += LogClientAsync;
        _commandService.Log += LogCommandAsync;

    }

    private Task LogClientAsync(LogMessage message)
    {
        _clientLogger.Log(LogLevelFromSeverity(message.Severity), 0, message, message.Exception,(_1, _2) => message.ToString(prependTimestamp: false));
        return Task.CompletedTask;
    }
    private Task LogCommandAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            var _ = cmdException.Context.Channel.SendMessageAsync($"Error: {cmdException.Message}");
            /*
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
            */
        }

        _commandLogger.Log(LogLevelFromSeverity(message.Severity), 0, message, message.Exception,(_1, _2) => message.ToString(prependTimestamp: false));
        return Task.CompletedTask;
    }
    private static LogLevel LogLevelFromSeverity(LogSeverity severity)
            => (LogLevel)(Math.Abs((int)severity - 5));
}