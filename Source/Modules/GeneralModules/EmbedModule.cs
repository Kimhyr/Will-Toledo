using Discord;
using Discord.Interactions;
using PenileNET.Modals;
using PenileNET.Utilities;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    public class EmbedModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("embed", "Creates an embed.")]
        public async Task Embed() {
            await RespondWithModalAsync<EmbedModal>("embed_modal");
        }

        [ModalInteraction("embed_modal")]
        public async Task EmbedModal(EmbedModal modal) {
            var embed = new EmbedBuilder {
                Title = modal.Name
            };

            var color = modal.Color;
            if (!string.IsNullOrWhiteSpace(color)) {
                try {
                    embed.Color = GeneralTools.StringToColor(color);
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "Color is formatted incorrectly.",
                            Description = "`Color must be formatted in 'R G B'`"
                        }.Build()
                    );
                }
            }

            var author = modal.Author;
            if (!string.IsNullOrWhiteSpace(author)) {
                embed.Author = new EmbedAuthorBuilder {
                    Name = author
                };
            }

            var thumbnail = modal.Thumbnail;
            if (!string.IsNullOrWhiteSpace(thumbnail)) {
                embed.ThumbnailUrl = thumbnail;
            }

            var description = modal.Description;
            if (!string.IsNullOrWhiteSpace(description)) {
                embed.Description = description;
            }

            try {
                await RespondAsync(
                    embed: embed.Build()
                );
            } catch {
                await RespondAsync(
                    embed: new EmbedBuilder {
                        Color = Colors.Offline,
                        Title = "There was an error creating an embed."
                    }.Build(),
                    ephemeral: true
                );
            }
        }
    }
}