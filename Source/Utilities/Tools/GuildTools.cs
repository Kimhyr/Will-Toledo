using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities.Constants;

namespace PenileNET.Utilities {
    public class GuildTools {
        public static SocketVoiceChannel CheckVoiceChannel(SocketVoiceChannel voiceChannel, SocketInteractionContext context) {
            if (voiceChannel == null) {
                return context.Guild.GetUser(context.User.Id).VoiceChannel;
            } else {
                return voiceChannel;
            }
        } 

        public static EmbedBuilder GuildEmbed(SocketGuild guild) {
            return new EmbedBuilder() {
                Color = Colors.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = guild.Id.ToString()
                },
                Title = guild.Name,
                Description = $"**Owner** {guild.Owner.Mention}\n"
            };
        }

        public static SocketGuildEvent? GetActiveEvent(IReadOnlyCollection<SocketGuildEvent> events) {
            foreach (var guildEvent in events) {
                switch (guildEvent.Status) {
                case GuildScheduledEventStatus.Active: return guildEvent;
                case GuildScheduledEventStatus.Scheduled: return guildEvent;
                case GuildScheduledEventStatus.Cancelled: return guildEvent;
                default: return guildEvent;
                }
            }

            return null;
        }

        public static string FormatVoiceChannel(SocketVoiceChannel channel) {
            var limit = channel.UserLimit.ToString();
            if (channel.UserLimit == null) {
                limit = "No limit";
            }

            var region = channel.RTCRegion;
            if (string.IsNullOrWhiteSpace(region)) {
                region = "No region";
            }

            return $"{channel.Mention}\n"
                + $"> **Limit** `{limit}`\n"
                + $"> **Bitrate** `{channel.Bitrate}`\n"
                + $"> **Region** `{region}`";
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
                + $"> **{guildEvent.Name}** ";

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
                str += $"> ```{description}```";
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