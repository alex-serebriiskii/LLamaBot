namespace SchizoLlamaBot.DataModels.LLM.Llama
{
    public class LlamaResponse
    {
        public string? model {get; set;}
        public DateTime created_at {get; set;}
        public string? response {get; set;}
        public LlamaChatMessage? message{get; set;}
        public bool? done {get; set;}
        public long total_duration {get; set;}
        public long load_duration {get; set;}
        public long prompt_eval_count {get; set;}
        public long prompt_eval_duration {get; set;}
        public long eval_count{get; set;}
        public long eval_duration{get; set;}
    }

    public class LlamaPromptMessage
    {
        public string? model {get; set;}
        public string? prompt {get; set;}
        public IEnumerable<LlamaChatMessage>? messages {get; set;}
        public bool stream {get; set;} = false;
    }
    public class LlamaChatMessage
    {
        public string? role {get; set;}
        public string? content {get; set;}
        public string? images {get; set;}
    }
}