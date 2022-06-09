using Discord;
using Discord.Interactions;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    [Group("testing", "Commands for testing and debugging.")]
    public class TestingModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("ping", "Responds with 'Ping!'.")]
        public async Task Ping() {
            var embed = new EmbedBuilder {
                Color = Colors.Blurple,
                Title = "Pong!"
            };

            await RespondAsync(
                embed: embed.Build()
            );
        }
    }
}