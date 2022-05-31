using System.Threading.Tasks;

using Discord;
using Discord.Interactions;

using PenileNET.Services;

namespace PenileNET.Modules {
    [Group("testing", "Commands for testing and debugging.")]
    public class TestingModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService Commands { get; set; }
        private InteractionHandler _handler;

        public TestingModule(InteractionHandler handler) {
            _handler = handler;
        }

        [SlashCommand("ping", "Responds with 'Ping!'.")]
        public async Task Ping() {
            var embed = new EmbedBuilder {
                Color = Color.Green,
                Title = "Pong!"
            };

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("echo", "Responds with the given input.")]
        public async Task Echo(string? input) {
            var embed = new EmbedBuilder {
                Color = Color.Green,
                Title = input
            };

            await RespondAsync(
                embed: embed.Build()
            );
        }
    }
}