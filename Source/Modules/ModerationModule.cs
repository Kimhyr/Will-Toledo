using Discord.Interactions;
using PenileNET.Services;

namespace PenileNET.Modules {
    [Group("mod", "Commands and automatics for moderators.")]
    public class ModeratorModule {
        private InteractionHandler _handler;

        public ModeratorModule(InteractionHandler handler) {
            _handler = handler;
        }

        public InteractionService Commands { get; set; }
        
        // TODO: mail, logging, and automod
    }
}