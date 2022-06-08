using Discord.Interactions;
using PenileNET.Services;

namespace PenileNET.Modules {
    [Group("embed", "Commands for creating custom things.")]
    public class EmbedModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }
        private InteractionHandler _handler;

        public EmbedModule(InteractionHandler handler) {
            _handler = handler;
        }

        [SlashCommand("rules", "Creates a rules-tailored embed.")]
        public async Task Rules() {
            
        }
    } 
}