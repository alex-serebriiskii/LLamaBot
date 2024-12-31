using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

[TableName("users")]
public partial class User
{
    [QueryField("id")]
    public decimal Id { get; set; }

    [QueryField("username")]
    public string? Username { get; set; }

    [QueryField("created_at")]
    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<GuildUser> GuildUsers { get; set; } = new List<GuildUser>();
}
