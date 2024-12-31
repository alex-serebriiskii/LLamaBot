using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("chats")]
public partial class Chat
{
    [QueryField("id")]
    public Guid Id { get; set; }

    [QueryField("chat_history")]
    public string? ChatHistory { get; set; }//Rename this to be ChatContent

    [QueryField("model_file")]
    public string? ModelFile { get; set; }

    [QueryField("created_at")]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChatMessage> Messages{ get; set; } = new List<ChatMessage>();
}
