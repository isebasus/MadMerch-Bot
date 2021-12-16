using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MadBot.Services;

namespace MadBot
{
    public class Madmerch
    {

        private static int _ticket = 1000;
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public static void Main(string[] args)
            => new Madmerch().MainAsync().GetAwaiter().GetResult();
        public static int Ticket
        {
            get { return _ticket; }
            set { _ticket = value; }
        }
        public async Task MainAsync()
        {
            string token = "OTE4NzU3MTQyMTg0NDg0ODc1.YbL5RA.zd_azFLK9hGkUBrnRG8h3ZokZ2o";

            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot,
                token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            

            int argPos = 0;
            if (message.HasStringPrefix("~", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}