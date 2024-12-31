using Discord.WebSocket;

namespace SchizoLlamaBot.Services.LLMServices
{
    public interface ILLMService
    {
        Task<List<string>> Generate(string question);
        Task<List<string>> Chat(string input, SocketGuild guild, SocketUser user);
    }
}