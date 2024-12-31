using System;
using System.Collections.Generic;

namespace SchizoLlamaBot.DataModels.DB;

public partial class Log
{
    public string? Level { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }
}
