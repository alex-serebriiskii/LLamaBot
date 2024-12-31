using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

public partial class GuildChat
{
    public ulong GuildId { get; set; }

    public Guid? Id { get; set; }

    public string? Description { get; set; }

    public Guid? ChatId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual Guild Guild { get; set; } = null!;
}
