using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities;

namespace PenileNET.Modules {
    [Group("guild", "Commands for getting and manipulating guilds.")]
    public class GuildModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("display", "Displays the guild's information")]
        public async Task Display() {
            var embed = GuildTools.GuildEmbed(Context.Guild);
            
            var guild = Context.Guild;
            embed.ThumbnailUrl = guild.IconUrl;
            
            var events = guild.Events;
            if (events.Count > 0) {
                var _event = GuildTools.GetActiveEvent(events);
                if (_event != null) {
                    embed.Description += $"{GuildTools.FormatEvent(_event)}\n";
                }
            }

            embed.Description += $"**Created at** `{guild.CreatedAt.ToString("MMM d, yyyy")}`";
            embed.ImageUrl = guild.BannerUrl;
            
            var channels = guild.Channels;
            if (channels.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = $"Channels [{guild.Channels.Count}]",
                        Value = GuildTools.FormatChannels(guild)
                    }
                );
            }
            
            var roles = guild.Roles;
            if (roles.Count > 1) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = $"Roles [{roles.Count}]",
                        Value = GuildTools.FormatRoles(roles)
                    }
                );
            }

            var users = guild.Users;
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = $"Users [{users.Count}]",
                    Value = GuildTools.FormatUsers(users)
                }
            );

            var premiumCount = guild.PremiumSubscriptionCount;
            if (premiumCount > 0) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Premium [{premiumCount}]",
                        Value = $"**Tier** `{guild.PremiumTier}`\n"
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("channels", "Display or create the guild's channels")]
        public async Task Channels(SocketGuildChannel? channel = null) {
            var embed = GuildTools.GuildEmbed(Context.Guild);

            var guild = Context.Guild;
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = $"Channels [{guild.Channels.Count}]",
                    Value = GuildTools.FormatChannels(guild)
                }
            );

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("roles", "Display or create the guild's roles.")]
        public async Task Roles(SocketGuildChannel? channel = null) {
            var embed = GuildTools.GuildEmbed(Context.Guild);

            var roles = Context.Guild.Roles;
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = $"Roles [{roles.Count}]",
                    Value = GuildTools.FormatRoles(roles)
                }
            );

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("users", "Displays the guild's users.")]
        public async Task Users() {
            var embed = GuildTools.GuildEmbed(Context.Guild);

            var users = Context.Guild.Users;
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = $"Users [{users.Count}]",
                    Value = GuildTools.FormatUsers(users)
                }  
            );

            await RespondAsync(
                embed: embed.Build()
            );
        }
    }
}