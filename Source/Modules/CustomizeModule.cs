using Discord.Interactions;
using PenileNET.Services;

namespace PenileNET.Modules {
    [Group("customize", "Commands for customizing guilds.")]
    public class EmbedModule {
        private InteractionHandler _handler;

        public EmbedModule(InteractionHandler handler) {
            _handler = handler;
        }

        public InteractionService Commands { get; set; }

        // TODO: embeds, reactions, and events
    }
}