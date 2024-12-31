using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("guilduserchats")]
public partial class GuildUserChat
{
    [QueryField("guilduserid")]
    public Guid GuildUserId { get; set; }

    [QueryField("chat_id")]
    public Guid ChatId { get; set; }

    [QueryField("created_at")]
    public DateTime? CreatedAt { get; set; }

    public virtual Chat? Chat { get; set; }
}
