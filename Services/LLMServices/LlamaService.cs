using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using SchizoLlamaBot.DataModels.LLM.Llama;
using SchizoLlamaBot.DataModels.DB;
using Discord.WebSocket;
using SchizoLlamaBot.Services.Database;
using SchizoLlamaBot.Repositories;
using SchizoLlamaBot.Database;
namespace SchizoLlamaBot.Services.LLMServices
{
    public class LlamaService: ILLMService
    {
        private readonly HttpClient _httpClient;
        private readonly IDatabaseService _db;
        private readonly string _model = "llama3.1";
        private readonly string _promptModifier = "place any prompt modifiers here  Format your response in discord markdown. ";
        private readonly LlamaChatMessage _systemChatMessage;        
        public LlamaService(HttpClient httpClient, IDatabaseService db, DatabaseRepository repository)
        {
            _httpClient = httpClient;
            _db = db;
            _systemChatMessage = new LlamaChatMessage(){
                role = "system",
                content = _promptModifier 
            };
        }
        public async Task<List<string>> Generate(string question)
        {
            var modifiedQuestion = _promptModifier +  question;
            using StringContent jsonContent = new (
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    model = _model,
                    prompt = modifiedQuestion,
                    //stream = false,
                }),
                Encoding.UTF8,
                "application/json");
            
            try{
                using HttpResponseMessage response =  await _httpClient.PostAsync("generate", jsonContent);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStreamAsync();
                var test = AssembleAskResponse(responseString, false);
                //dynamic r = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
                var llamaResponse = "get bent retard";// r.response;
                Console.WriteLine(llamaResponse);
                return test.Item1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>() { ex.Message};
            }
        }

        public async Task<List<string>> Chat(string input, SocketGuild guild , SocketUser user )
        {
            //TODO implement caching here
            try
            {
                var userChatHistory = await _db.GetGuildUserChatHistory(guild, user);
                var history = userChatHistory.Item1 != null ? userChatHistory.Item1.ToList() : new List<LlamaChatMessage>();
                if (!history.Any())
                {
                    history.Add(_systemChatMessage);
                    await _db.UpdateGuildUserChatHistory(userChatHistory.Item2, _systemChatMessage);
                }
                var userMessage = new LlamaChatMessage
                {
                    role = "user",
                    content = input
                };
 
                history.Add(userMessage);
                await _db.UpdateGuildUserChatHistory(userChatHistory.Item2, userMessage);
                using StringContent jsonContent = new(
                    System.Text.Json.JsonSerializer.Serialize(new LlamaPromptMessage
                    {
                        model = _model,
                        messages = history
                    }), Encoding.UTF8,
                    "application/json");

                using HttpResponseMessage response = await _httpClient.PostAsync("chat", jsonContent);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStreamAsync();
                var askResponse = AssembleAskResponse(responseString, true);
                await _db.UpdateGuildUserChatHistory(userChatHistory.Item2, askResponse.Item2);
                return askResponse.Item1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>() { ex.Message };
            }
        }
        //private LlamaPromptMessage BuildLlamaPromptMessage(string input,)

        private (List<string>, LlamaChatMessage) AssembleAskResponse(Stream stream, bool useMessages)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();
            var streamReader = new StreamReader(stream, new UTF8Encoding());
            var results = new List<LlamaResponse>();
            using (var reader = new JsonTextReader(streamReader)) 
            {
                reader.CloseInput = false;
                // important part
                reader.SupportMultipleContent = true;
                while (reader.Read()) 
                {
                    var res = serializer.Deserialize<LlamaResponse>(reader);
                    if(res != null)
                    {
                        results.Add(res);
                    }
                }
            }
            var tokens = results.OrderBy(x=>x.created_at).Select(x =>
                {
                    if(useMessages)
                    {
                        return x.message.content;
                    }
                    return x.response;
                }
            ).ToList();
            var fullMessage = new LlamaChatMessage { role = "assistant", content = string.Join("",tokens) };
            //string.Join("",tokens);
            var result = new List<string>();
            var resLen = 0;
            var tempStr = "";
            for(int i = 0; i < tokens.Count(); i++)
            {
                if(resLen + tokens[i].Length > 2000)
                {
                    result.Add(tempStr);
                    tempStr = "";
                    resLen = 0;
                }
                tempStr = tempStr + tokens[i];
                resLen += tokens[i].Length;
            }
            result.Add(tempStr);
            return (result,fullMessage);
        }
    }

}