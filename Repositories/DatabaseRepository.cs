using System.Data;
using System.Reflection;
using Dapper;
using Discord;
using Discord.WebSocket;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using SchizoLlamaBot.Database;
using SchizoLlamaBot.DataModels.DB;

namespace SchizoLlamaBot.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly IDbContext _db;
        public DatabaseRepository(IDbContext db)
        {
           _db = db; 
        }
        //These should be broken out into their own repos probably
        public async Task Command(string query, object? parameters)
        {
            using (var connection = _db.CreateConnection())
            {
                await connection.ExecuteAsync(query,parameters);
            }

        }
        public async Task<IEnumerable<T>> Query<T>(string query, object? parameters = null)
        {
            using (var connection = _db.CreateConnection())
            {
                DefaultTypeMap.MatchNamesWithUnderscores = true;
                var result = await connection.QueryAsync<T>(query,parameters);
                return result;
            }
        }
        #region Chat
        // CRUD methods for Chat
        public async Task CreateChatAsync(Chat chat)
        {
            //var sql = "Insert into chats (id, chat_history, model_file, created_at) values (@Id, @ChatHistory, @ModelFile, @CreatedAt)";
            var sql = BuildInsertQuery(typeof(Chat));
            await Command(sql, chat);
        }

        public async Task CreateChatWithChildrenAsync(Chat chat)
        {
            var sql = BuildInsertQuery(typeof(Chat));
            await Command(sql, chat);
            if(chat.Messages.Any())
            {
                await CreateChatMessageAsync(chat.Messages);
            }
        }

        public async Task<Chat?> ReadChatAsync(Guid id)
        {
            var sql = "select top 1 * from chats where id = @Id";
            var chat = await Query<Chat>(sql, new {Id = id});
            return (chat!= null) && chat.Any() ? chat.FirstOrDefault() : null;
        }

        public async Task UpdateChatAsync(Chat chat)
        {
            // TODO, currently no case for chat updates
            throw new NotImplementedException();
        }

        public async Task DeleteChatAsync(int id)
        {
            // TODO, currently no case for chat deletions
            throw new NotImplementedException();
        }
        #endregion

        #region ChatMessage
        // CRUD methods for ChatMessage
        //TODO clean this part up with generics
        public async Task CreateChatMessageAsync(ChatMessage chatMessage)
        {
            //var sql = "Insert into chatmessages (chat_id, role, content, images, created_at) values (@ChatId, @Role, @Content, @Images, @CreatedAt)";
            var sql = BuildInsertQuery(typeof(ChatMessage));
            await Command(sql, chatMessage);
        }
        public async Task CreateChatMessageAsync(IEnumerable<ChatMessage> messages)
        {

            var sql = BuildInsertQuery(typeof(ChatMessage));
            await Command(sql, messages);
        }

        public async Task<IEnumerable<ChatMessage>?> ReadChatMessageAsync(Guid chatId)
        {
            var sql = "select * from chatmessages where chat_id = @ChatId";
            var messages = await Query<ChatMessage>(sql, new {ChatId = chatId});
            return messages;
        }

        public async Task UpdateChatMessageAsync(ChatMessage chatMessage)
        {
            //TODO no use case yet
            throw new NotImplementedException();
        }

        public async Task DeleteChatMessageAsync(int id)
        {
            //TODO no use case yet
            throw new NotImplementedException();
        }
        #endregion

        #region Guild
        // CRUD methods for Guild
        public async Task CreateGuildAsync(Guild guild)
        {
            // Implement your create logic here
            var sql = BuildInsertQuery(typeof (Guild));
            await Command(sql, guild);
        }

        public async Task<Guild?> ReadGuildAsync(decimal id)
        {
            var sql = "select Top 1 * from guilds where id = @Id";
            var guilds = await Query<Guild>(sql, new {Id = id});
            if(guilds != null)
            {
                return guilds.FirstOrDefault();
            }
            return null;
        }

        public async Task UpdateGuildAsync(Guild guild)
        {
            // Implement your update logic here
        }

        public async Task DeleteGuildAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion

        #region  GuildChannelChat
        // CRUD methods for GuildChannelChat
        public async Task CreateGuildChannelChatAsync(GuildChannelChat guildChannelChat)
        {
            // Implement your create logic here
        }

        public async Task<GuildChannelChat> ReadGuildChannelChatAsync(int id)
        {
            // Implement your read logic here
            return new GuildChannelChat(); // Placeholder return
        }

        public async Task UpdateGuildChannelChatAsync(GuildChannelChat guildChannelChat)
        {
            // Implement your update logic here
        }

        public async Task DeleteGuildChannelChatAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion

        #region GuildChat
        // CRUD methods for GuildChat
        public async Task CreateGuildChatAsync(GuildChat guildChat)
        {
            // Implement your create logic here
        }

        public async Task<GuildChat> ReadGuildChatAsync(int id)
        {
            // Implement your read logic here
            return new GuildChat(); // Placeholder return
        }

        public async Task UpdateGuildChatAsync(GuildChat guildChat)
        {
            // Implement your update logic here
        }

        public async Task DeleteGuildChatAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion

        #region  GuildUserChat
        // CRUD methods for GuildUserChat
        public async Task CreateGuildUserChatAsync(GuildUserChat guildUserChat)
        {
            var sql = BuildInsertQuery(typeof (GuildUserChat));
            await Command(sql, guildUserChat);
        }

        public async Task CreateGuildUserChatWithChildrenAsync(GuildUserChat guildUserChat)
        {
            if(guildUserChat.Chat!= null)
            {
                await CreateChatAsync(guildUserChat.Chat);
            }
            var sql = BuildInsertQuery(typeof(GuildUserChat));
            await Command(sql,guildUserChat);
        }

        public async Task<GuildUserChat> ReadGuildUserChatAsync(int id)
        {
            // Implement your read logic here
            return new GuildUserChat(); // Placeholder return
        }
        public async Task<GuildUserChat> ReadGuildUserChatWithMessagesAsync(Guid guildUserId)
        {
            var sql = "select top 1 * from guilduserchats where guilduserid = @GuildUserId";
            var guildUserChats =  await Query<GuildUserChat>(sql, new { GuildUserId = guildUserId });
            if(guildUserChats == null)
            {
                throw new Exception("No GuildUserChats found for Id: " + guildUserId.ToString()); 
            }
            var guildUserChat = guildUserChats.FirstOrDefault();
            sql = "select * from chats c left join chatmessages cm on c.Id = cm.chat_id where c.Id = @ChatId";
            using (var connection = _db.CreateConnection())
            {
                Dictionary<Guid,Chat> queryDict = new Dictionary<Guid,Chat>();
                DefaultTypeMap.MatchNamesWithUnderscores = true;
                var result = await connection.QueryAsync<Chat,ChatMessage,Chat>(sql, (chat, chatmessages) => 
                {
                    if(queryDict.TryGetValue(guildUserChat.ChatId, out var existingChat))
                    {
                        chat = existingChat;
                    }
                    else
                    {
                        queryDict.Add(guildUserChat.ChatId,chat);
                    }

                    if(chatmessages != null)
                        chat.Messages.Add(chatmessages);

                    return chat;  
                },
                splitOn: "chat_id", param: new {ChatId = guildUserChat.ChatId});
                guildUserChat.Chat = result.FirstOrDefault(new Chat());
            } 
            return guildUserChat;
        }

        public async Task UpdateGuildUserChatAsync(GuildUserChat guildUserChat)
        {
            // Implement your update logic here
        }

        public async Task DeleteGuildUserChatAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion

        #region GuildUser
        // CRUD methods for GuildUser
        public async Task CreateGuildUserAsync(GuildUser guildUser)
        {
            var sql = BuildInsertQuery(typeof(GuildUser));
            await Command(sql, guildUser);
        }

        public async Task CreateGuildUserWithChildrenAsync(GuildUser guildUser, bool addGuild = false, bool addUser = false)
        {
            if(guildUser.Guild != null && addGuild)
            {
                await CreateGuildAsync(guildUser.Guild);
            }
            if(guildUser.User!= null && addUser)
            {
                await CreateUserAsync(guildUser.User);
            }
            //Insert the guild user here to satify foreign key constraints 
            var sql = BuildInsertQuery(typeof (GuildUser));
            await Command(sql, guildUser);
            if (guildUser.GuildUserChat != null)
            {
                await CreateGuildUserChatWithChildrenAsync(guildUser.GuildUserChat);
            }

        }

        public async Task<GuildUser?> ReadGuildUserAsync(decimal guildId, decimal userId)
        {
            var sql = "select top 1 * from guildusers where guild_id = @GuildId and user_id = @UserId";
            var guildUsers = await Query<GuildUser>(sql, new {GuildId = guildId, UserId = userId});
            return guildUsers.Any() ? guildUsers.FirstOrDefault() : null;
        }

        public async Task UpdateGuildUserAsync(GuildUser guildUser)
        {
            // Implement your update logic here
        }

        public async Task DeleteGuildUserAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion

        #region User
        // CRUD methods for User
        public async Task CreateUserAsync(User user)
        {
            var sql = BuildInsertQuery(typeof(User));
            await Command(sql, user);
        }

        public async Task<User?> ReadUserAsync(decimal id)
        {
            var sql = "select Top 1 * from users where id = @Id";
            var users = await Query<User>(sql, new {Id = id});
            if(users!= null)
            {
                return users.FirstOrDefault();
            }
            return null;
        }

        public async Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
            // Implement your update logic here
        }

        public async Task DeleteUserAsync(int id)
        {
            // Implement your delete logic here
        }
        #endregion
        /// <summary>
        /// TODO reflection based query generator, to be moved to a source generator later down the line
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string BuildInsertQuery(Type t)
        {
            var table = "";
            var columnList = "";
            var fieldList = "";
            Attribute[] attrs = Attribute.GetCustomAttributes(t, false);
            foreach (Attribute attr in attrs)
            {
                if(attr is TableName tn)
                {
                    table = tn.Value;
                }
            }

            var props = t.GetProperties();
            foreach (var p in props)
            {
                object[] propAttrs = p.GetCustomAttributes(typeof(QueryField), true);
                foreach(object pa in propAttrs)
                {
                    QueryField? qf = pa as QueryField;
                    if (qf != null)
                    {

                        columnList = columnList+ qf.ColumnName + ", ";
                        fieldList = fieldList + "@" + qf.FieldName + ", ";
                    }
                }
            }

            columnList = columnList.Substring(0, columnList.Length -2);
            fieldList = fieldList.Substring(0, fieldList.Length -2);
            var result = "insert into " + table + "(" + columnList + ") values (" + fieldList + ")" ;
            return result;
        }
    }
}