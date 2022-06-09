using Discord;
using Discord.Interactions;
using PenileNET.Utilities;

namespace PenileNET.Modules {
    [Group("guild", "Commands for getting and manipulating guilds.")]
    public class GuildModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("display", "Displays the guild's information")]
        public async Task Display() {
            await RespondAsync(
                embed: GuildTools.GuildEmbed(Context.Guild).Build()
            );
        }

        [SlashCommand("users", "Displays the guild's users.")]
        public async Task Users() {
            await RespondAsync(
                embed: new EmbedBuilder {
                    Title = Context.Guild.Name,
                    Fields = {
                        new EmbedFieldBuilder {
                            Name = "Users",
                            Value = GuildTools.FormatUsers(Context.Guild.Users)
                        }
                    }
                }.Build()
            );
        }
    }
}