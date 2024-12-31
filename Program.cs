using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using SchizoLlamaBot.Services.Commands;
using SchizoLlamaBot.Services.LLMServices;
using SchizoLlamaBot.DataModels.DB;
using SchizoLlamaBot.Services.Database;
using SchizoLlamaBot.Database;
using SchizoLlamaBot.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;

namespace SchizoLlamaBot;

class Program
{
    private static IServiceProvider _serviceProvider;
    
    
    static async Task Main(string[] args)
    {
        _serviceProvider = CreateProvider();
        await RunAsync(args);
    }
    static IServiceProvider CreateProvider()
    {
        Console.WriteLine("Looking for configuration in: " + Environment.CurrentDirectory);
        IConfiguration appconfig = new ConfigurationBuilder()
            //.SetBasePath(Environment.CurrentDirectory)
            //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json")
            //.AddEnvironmentVariables()
            .Build();
        var discordConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All,
            //...
        };
        

        var collection = new ServiceCollection()
            .AddLogging( loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            })
            .Configure<DbConfig>(appconfig.GetSection("ConnectionStrings"))
            .AddOptions()
            .AddSingleton<LoggingService>()
            .AddSingleton(discordConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<IDbContext, LlamaSqlServerDbContext>()
            .AddSingleton<DatabaseRepository>()
            .AddSingleton<IDatabaseService, LlamaDbService>();


        //Client addition has to happen separately from service addition to avoid the conversion to HttpBuilder
        collection
            .AddHttpClient<ILLMService,LlamaService>( "Ollama",client =>{
                client.BaseAddress = new Uri("");
                client.Timeout = TimeSpan.FromSeconds(300);
            });
            

        return collection.BuildServiceProvider(); 
    }
    static async Task RunAsync(string[] args)
    {
        var token = "";
        // Request the instance from the client.
        // Because we're requesting it here first, its targetted constructor will be called and we will receive an active instance.
        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        await _serviceProvider.GetRequiredService<CommandHandlingService>().InitializeAsync();

        
        client.Ready += () => 
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };
        
        await client.LoginAsync(TokenType.Bot, token); 
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}
