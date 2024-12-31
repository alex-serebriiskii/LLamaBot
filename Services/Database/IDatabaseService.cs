using Discord;
using Discord.WebSocket;
using SchizoLlamaBot.DataModels.LLM.Llama;
namespace SchizoLlamaBot.Services.Database
{
    public interface IDatabaseService
    {
        Task<(IEnumerable<LlamaChatMessage>,Guid)> GetGuildUserChatHistory(SocketGuild socketGuild, SocketUser user);
        Task UpdateGuildUserChatHistory(Guid guildUserId, LlamaChatMessage message);
    }
}