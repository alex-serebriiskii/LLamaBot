using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("guildusers")]
public partial class GuildUser
{
    [QueryField("user_id")]
    public decimal UserId { get; set; }

    [QueryField("guild_id")]
    public decimal GuildId { get; set; }

    [QueryField("id")]
    public Guid Id { get; set; }

    [QueryField("created_at")]
    public DateTime? CreatedAt { get; set; }

    public virtual Guild? Guild { get; set; } 

    public virtual GuildUserChat? GuildUserChat { get; set; }

    public virtual User? User { get; set; } 
}
