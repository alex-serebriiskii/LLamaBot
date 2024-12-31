using System.ComponentModel;
using Discord.WebSocket;
using SchizoLlamaBot.DataModels.DB;
using SchizoLlamaBot.DataModels.LLM.Llama;
using SchizoLlamaBot.Repositories;

namespace SchizoLlamaBot.Services.Database
{
    public class LlamaDbService: IDatabaseService
    {
        private readonly DatabaseRepository _db;
        public LlamaDbService(DatabaseRepository db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<LlamaChatMessage>, Guid)> GetGuildUserChatHistory(SocketGuild socketGuild, SocketUser socketUser) 
        {
            var guildUser = await _db.ReadGuildUserAsync(socketGuild.Id, socketUser.Id);
            if(guildUser == null)
            {
                guildUser = await BuildGuildUser(socketGuild, socketUser);
            }
            var guildUserChat = await _db.ReadGuildUserChatWithMessagesAsync(guildUser.Id);
            if(guildUserChat.Chat.Messages.Any())
            {
                return (guildUserChat.Chat.Messages.Where(m=>m.ChatId != null).OrderBy(m=>m.CreatedAt).Select((m) => { return m.ToLlamaChatMessage();}),guildUserChat.Chat.Id);
            }
            return (new List<LlamaChatMessage>(), guildUserChat.Chat.Id);
        }
        public async Task UpdateGuildUserChatHistory(Guid chatId, LlamaChatMessage message)    
        {
           await _db.CreateChatMessageAsync( new ChatMessage(){
                ChatId = chatId,
                Role = message.role,
                Content = message.content,
                Images = message.images,
                CreatedAt = DateTime.UtcNow
           });
        }

        private async Task<GuildUser> BuildGuildUser(SocketGuild socketGuild, SocketUser socketUser)
        {
            var addGuild = false;
            var addUser = false;

            var guild = await _db.ReadGuildAsync(socketGuild.Id);
            if(guild == null)
            {
                guild = new Guild()
                {
                    Id = socketGuild.Id,
                    GuildName = socketGuild.Name,
                    CreatedAt = DateTime.UtcNow
                };
                addGuild = true;
            }
            var user = await _db.ReadUserAsync(socketUser.Id);
            if(user == null)
            {
                user = new User()
                {
                    Id = socketUser.Id,
                    Username = socketUser.Username,
                    CreatedAt = DateTime.UtcNow,
                };
                addUser = true;
            }
            var guildUser = new GuildUser()
            {
                UserId = user.Id,
                GuildId = guild.Id,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now
            };
            var chat = new Chat()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            };
            var guildUserChat = new GuildUserChat()
            {
                GuildUserId = guildUser.Id,
                ChatId = chat.Id,
                CreatedAt = DateTime.UtcNow,
                Chat = chat,
            };
            guildUser.GuildUserChat = guildUserChat;
            guildUser.Guild = guild;
            guildUser.User = user;
            await _db.CreateGuildUserWithChildrenAsync(guildUser:guildUser, addGuild: addGuild, addUser: addUser);
            return guildUser;

        }
        

    }
}