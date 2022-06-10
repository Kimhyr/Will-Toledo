using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PenileNET.Services;

namespace PenileNET {
    public class Program {
        private readonly IConfiguration _config;
        private readonly ulong _testGuildId;
        private DiscordSocketClient? _client;
        private InteractionService? _interactions;

        private Program() {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json")
                .Build();
            _testGuildId = ulong.Parse(_config["TEST_GUILD_ID"]);
        }

        public static Task Main(string[] args) {
            return new Program().MainAsync();
        }

        public async Task MainAsync() {
            var services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton(x =>
                    new DiscordSocketClient(
                        new DiscordSocketConfig {
                            GatewayIntents = GatewayIntents.All,
                            LogGatewayIntentWarnings = false,
                            AlwaysDownloadUsers = true,
                            LogLevel = LogSeverity.Debug
                        }
                    )
                )
                .AddSingleton(x =>
                    new InteractionService(x.GetRequiredService<DiscordSocketClient>())
                )
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();

            _client = services.GetRequiredService<DiscordSocketClient>();
            _interactions = services.GetRequiredService<InteractionService>();

            _client.Log += LogAsync;
            _interactions.Log += LogAsync;
            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, _config["TOKEN"]);
            await _client.StartAsync();

            await services.GetRequiredService<InteractionHandler>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private async Task ReadyAsync() {
            if (_interactions != null) {
                if (IsDebug()) {
                    Console.WriteLine($"[DEBUG] Registering commands to '{_testGuildId}'...");

                    await _interactions.RegisterCommandsToGuildAsync(_testGuildId);
                } else {
                    Console.WriteLine("Registering commands globally...");

                    await _interactions.RegisterCommandsGloballyAsync();
                }
            }

            if (_client != null) {
                Console.WriteLine($"Connected as '{_client.CurrentUser}'.");
            }
        }

        private static bool IsDebug() {
        #if DEBUG
            return true;
        #else
            return false;
        #endif
        }
    }
}