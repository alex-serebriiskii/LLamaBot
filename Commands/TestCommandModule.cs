using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SchizoLlamaBot.Services.LLMServices;

namespace SchizoLlamaBot.Modules
{
    public class LlamaModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILLMService _llmService;
        public LlamaModule(ILLMService llmService)
        {
            _llmService = llmService;
        }
        [Command("test")]
        [Alias("about", "whoami", "owner")]
        public async Task InfoAsync()
        {
            var app = await Context.Client.GetApplicationInfoAsync();

            await ReplyAsync(
                $"Test Successful.\n\n");
        }

        [Command("ask")]
        public async Task Ask([Remainder]string question)
        {
            var llmReply = await _llmService.Generate(question);
            for (int i = 0;i < llmReply.Count; i++)
            {
                await ReplyAsync(llmReply[i]);
            }
        }

        [Command ("Chat")]
        public async Task Chat([Remainder]string input)
        {
            var chatReply = await _llmService.Chat(input, Context.Guild, Context.User);
            for (int i = 0;i < chatReply.Count; i++)
            {
                await ReplyAsync(chatReply[i]);
            }
        }

    }
}