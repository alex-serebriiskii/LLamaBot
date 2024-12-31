using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("guilds")]
public partial class Guild
{
    [QueryField("id")]
    public decimal Id { get; set; }

    [QueryField("guildname")]
    public string? GuildName { get; set; }

    [QueryField("created_at")]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GuildChannelChat> GuildChannelChats { get; set; } = new List<GuildChannelChat>();

    public virtual GuildChat? GuildChat { get; set; }

    public virtual ICollection<GuildUser> GuildUsers { get; set; } = new List<GuildUser>();
}
