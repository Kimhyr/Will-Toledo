using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using PenileNET.Services;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    [Group("testing", "Commands for testing and debugging.")]
    public class TestingModule : InteractionModuleBase<SocketInteractionContext> {
        private InteractionHandler _handler;

        public TestingModule(InteractionHandler handler) {
            _handler = handler;
        }

        public InteractionService Commands { get; set; }

        [SlashCommand("ping", "Responds with 'Ping!'.")]
        public async Task Ping() {
            var embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Title = "Pong!"
            };

            await RespondAsync(
                embed: embed.Build()
            );
        }
    }
}