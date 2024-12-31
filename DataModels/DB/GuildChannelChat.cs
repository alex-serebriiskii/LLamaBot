using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

public partial class GuildChannelChat
{
    public ulong GuildId { get; set; }

    public ulong ChannelId { get; set; }

    public Guid? ChatId { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual Guild Guild { get; set; } = null!;
}
