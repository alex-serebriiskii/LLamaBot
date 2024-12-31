using System;
using System.Collections.Generic;
using SchizoLlamaBot.DataModels.LLM.Llama;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("chatmessages")]
public partial class ChatMessage
{
    [QueryField("chat_id")]
    public Guid ChatId { get; set; }

    [QueryField("role")]
    public string? Role { get; set; }

    [QueryField("content")]
    public string? Content { get; set; }

    [QueryField("images")]
    public string? Images { get; set; }

    [QueryField("created_at")]
    public DateTime CreatedAt { get; set; }
    public Chat Chat { get; set; }

    public LlamaChatMessage ToLlamaChatMessage()
    {
        return new LlamaChatMessage(){
            role = Role,
            content = Content,
            images = Images,
        };
    }
}
