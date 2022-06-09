using Discord;
using Discord.WebSocket;
using PenileNET.Utilities.Constants;

namespace PenileNET.Utilities {
    public class GuildTools {
        public static EmbedBuilder GuildEmbed(SocketGuild guild) {
            var embed = new EmbedBuilder {
                Color = Colors.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = guild.Id.ToString()
                },
                Title = guild.Name,
                ThumbnailUrl = guild.IconUrl,
                Description = $"**Owner** {guild.Owner.Mention}\n"
                    + $"**Created at** `{guild.CreatedAt.ToString("MMM d, yyyy")}`\n",
                ImageUrl = guild.BannerUrl,
                Fields = {
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Channels [{guild.Channels.Count}]",
                        Value = FormatChannels(guild)
                    },
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Roles [{guild.Roles.Count}]",
                        Value = FormatRoles(guild.Roles)
                    },
                    new EmbedFieldBuilder {
                        Name = $"Users [{guild.Users.Count}]",
                        Value = FormatUsers(guild.Users)
                    },
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Premium [{guild.PremiumSubscriptionCount}]",
                        Value = $"**Tier** `{guild.PremiumTier}`\n"
                    }
                }
            };

            if (guild.Events.Count > 0) {
                embed.Description += FormatEvent(guild.Events.First());
            }

            return embed;
        }

        public static string FormatChannels(SocketGuild guild) {
            var str = "";
            foreach (var channel in guild.Channels) {
                if (!guild.CategoryChannels.Contains(channel)) {
                    str += $"<#{channel.Id}> ";
                }
            }

            return str;
        }

        public static string FormatUsers(IReadOnlyCollection<SocketGuildUser> users) {
            var str = "";
            foreach (var user in users) {
                str += $"{user.Mention} ";
            }

            return str;
        }

        public static string FormatRoles(IReadOnlyCollection<SocketRole> roles) {
            var str = "";
            foreach (var role in GeneralTools.GetSorted(roles.ToList())) {
                if (!role.IsManaged) {
                    if (!role.IsEveryone) {
                        str += $"{role.Mention} ";
                    }
                }
            }

            return str;
        }

        public static string FormatEvent(SocketGuildEvent guildEvent) {
            var str = "\n**[";
            switch (guildEvent.Status) {
            case GuildScheduledEventStatus.Active:
                str += "Active";

                break;
            case GuildScheduledEventStatus.Cancelled:
                str += "Cancelled";

                break;
            case GuildScheduledEventStatus.Completed:
                str += "Completed";

                break;
            case GuildScheduledEventStatus.Scheduled:
                str += "Scheduled";

                break;
            }

            str += "]**\n"
                + $">>> **{guildEvent.Name}** ";

            if (guildEvent.UserCount != null) {
                str += $"[{guildEvent.UserCount}] ";
            }

            str += $"by {guildEvent.Creator.Mention}";

            var location = guildEvent.Location;
            if (!string.IsNullOrWhiteSpace(location)) {
                str += $" at **{location}**\n";
            } else {
                str += '\n';
            }

            var description = guildEvent.Description;
            if (!string.IsNullOrWhiteSpace(description)) {
                str += $"`{description}`\n";
            } else {
                str += '\n';
            }

            str += $"at **{guildEvent.StartTime.ToString("h:mm tt, MMM d")}**";

            if (guildEvent.EndTime != null) {
                str += $" to **{guildEvent.EndTime.Value.ToString("h:mm tt, MMM d")}**\n";
            } else {
                str += '\n';
            }

            return str;
        }
    }
}