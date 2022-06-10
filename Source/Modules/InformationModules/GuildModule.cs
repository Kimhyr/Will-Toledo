using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Modals;
using PenileNET.Utilities;
using PenileNET.Utilities.Constants;

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

        [SlashCommand("channels", "If there is no channel given, create a channel; Otherwise, display the channel.")]
        public async Task Channels(string? name = null) {
            var embed = GuildTools.GuildEmbed(Context.Guild);
            
            var guild = Context.Guild;
            if (name == null) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        Name = $"Channels [{guild.Channels.Count}]",
                        Value = GuildTools.FormatChannels(guild)
                    }
                );
            } else {
                await RespondWithModalAsync<ChannelModal>("channel_modal");
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [ModalInteraction("channel_modal")]
        public async Task EmbedModal(ChannelModal modal) {
            var guild = Context.Guild;

            var channelId = new ulong();
            switch (modal.Type) {
            case "text":
                var channel = await guild.CreateTextChannelAsync(modal.Name);
                await channel.ModifyAsync(x =>
                    x.Topic = modal.Topic
                );

                if (ulong.TryParse(modal.Category, out var outCategory)) {
                    await channel.ModifyAsync(x => 
                        x.CategoryId = outCategory
                    );
                }
                
                channelId = channel.Id;

                break;
            case "voice":
                channel = await guild.CreateTextChannelAsync(modal.Name);
                await channel.ModifyAsync(x =>
                    x.Topic = modal.Topic
                );

                if (ulong.TryParse(modal.Category, out outCategory)) {
                    await channel.ModifyAsync(x => 
                        x.CategoryId = outCategory
                    );
                }

                channelId = channel.Id;

                break;
            }

            var embed = GuildTools.GuildEmbed(guild);
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = "Voice Channel",
                    Value = $"<#{channelId}>"
                }
            );

             await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("purge", "Bulk deletes messages.")]
        public async Task Purge([MaxValue(100)] int limit) {
            var messages = await Context.Channel.GetMessagesAsync(limit).FlattenAsync();
            var filteredMessages = messages.Where(x => 
                (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14
            );
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(filteredMessages);
            
            var embed = GuildTools.GuildEmbed(Context.Guild); 
            embed.AddField(
                new EmbedFieldBuilder() {
                    Name = "Purged",
                    Value = $"**Messages** `{filteredMessages.Count()}`"
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