using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities.Constants;

namespace PenileNET.Utilities {
    public class UserTools {
        public static SocketGuildUser GetUser(SocketGuildUser? user, SocketInteractionContext context) {
            if (user == null) {
                return context.Guild.GetUser(context.User.Id);
            }

            return user;
        }

        public static EmbedBuilder ProfileEmbed(SocketGuildUser user) {
            var embed = new EmbedBuilder {
                Color = Colors.Blurple,
                Title = GetTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            if (user.PublicFlags != null) {
                embed.Author = new EmbedAuthorBuilder {
                    Name = FormatFlag(user.PublicFlags.Value)
                };
            }

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(user.Activities.First());
            }

            if (user.VoiceChannel != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(user.VoiceChannel)
                    }
                );
            }

            return embed;
        }

        public static EmbedBuilder UserProfileEmbed(SocketGuildUser user) {
            var embed = new EmbedBuilder {
                Color = GetStatusColor(user),
                Title = GetTag(user),
                ThumbnailUrl = user.GetAvatarUrl(),
                Fields = {
                    new EmbedFieldBuilder {
                        Name = "Created At",
                        Value = $"`{user.CreatedAt.ToString("MMM d, yyyy")}`"
                    }
                }
            };

            if (user.PublicFlags != null) {
                embed.Author = new EmbedAuthorBuilder {
                    Name = FormatFlag(user.PublicFlags.Value)
                };
            }

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(user.Activities.First());
            }

            return embed;
        }

        public static EmbedBuilder GuildProfileEmbed(SocketGuildUser user) {
            var embed = new EmbedBuilder {
                Color = GetStatusColor(user),
                Author = new EmbedAuthorBuilder {
                    Name = GeneralTools.GetSorted(user.Roles.ToList()).First().Name
                },
                Title = GetDisplayTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(user.Activities.First());
            }

            if (user.VoiceChannel != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(user.VoiceChannel)
                    }
                );
            }

            if (user.JoinedAt != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Joined At",
                        Value = $"`{user.JoinedAt.Value.ToString("MMM d, yyyy")}`"
                    }
                );
            }

            return embed;
        }


        public static EmbedBuilder AllProfileEmbed(SocketGuildUser user) {
            var embed = new EmbedBuilder {
                Color = GetStatusColor(user),
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = GetTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            if (user.Nickname != null) {
                embed.Description += $"**Nickname** {user.Mention}\n";
            }

            embed.Description += $"**Created at** `{user.CreatedAt.ToString("MMM d, yyyy")}`\n";

            if (user.JoinedAt != null) {
                embed.Description += $"**Joined at** `{user.JoinedAt.Value.ToString("MMM d, yyyy")}`\n";
            }

            var flags = user.PublicFlags;
            if (flags != null) {
                embed.Description += $"**Flag** `{FormatFlag(flags.Value)}`\n";
            }

            embed.Description += $"\n{FormatActivity(user.Activities.First())}";

            var roles = GeneralTools.GetSorted(user.Roles.ToList());
            if (roles.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Roles [{roles.Count}]",
                        Value = FormatRoles(roles)
                    }
                );
            }
            
            var channel = user.VoiceChannel;
            if (channel != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(channel)
                    }
                );
            }

            return embed;
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
                + $"**Limit** `{limit}`\n"
                + $"**Bitrate** `{channel.Bitrate}`\n"
                + $"**Region** `{region}`";
        }

        public static Color GetStatusColor(SocketGuildUser user) {
            switch (user.Status) {
            case UserStatus.Online: return Colors.Online;
            case UserStatus.DoNotDisturb: return Colors.Offline;
            case UserStatus.Idle: return Colors.Idle;
            case UserStatus.AFK: return Colors.Idle;
            default: return Colors.Grayple;
            }
        }

        public static string FormatFlag(UserProperties properties) {
            switch (properties) {
            case UserProperties.Staff: return "Staff";
            case UserProperties.System: return "System";
            case UserProperties.EarlySupporter: return "Early Supporter";
            case UserProperties.TeamUser: return "Team User";
            case UserProperties.VerifiedBot: return "Verified Bot";
            case UserProperties.BugHunterLevel1: return "Bug Hunter Level 1";
            case UserProperties.BugHunterLevel2: return "Bug Hunter Level 2";
            case UserProperties.DiscordCertifiedModerator: return "Certified Moderator";
            case UserProperties.HypeSquadBalance: return "HypeSquad Balance";
            case UserProperties.HypeSquadBravery: return "HypSquad Bravery";
            case UserProperties.HypeSquadBrilliance: return "HypeSquad Brilliance";
            case UserProperties.HypeSquadEvents: return "HypeSquad Events";
            case UserProperties.EarlyVerifiedBotDeveloper: return "Verified Bot Developer";
            case UserProperties.BotHTTPInteractions: return "Webhook";
            default: return "";
            }
        }

        public static string FormatRoles(IReadOnlyCollection<SocketRole> roles) {
            var str = "";
            foreach (var role in roles) {
                if (!role.IsEveryone) {
                    str += $"{role.Mention} ";
                }
            }

            return str;
        }

        public static string GetTag(SocketGuildUser user) {
            return $"{user.Username}#{user.Discriminator}";
        }

        public static string GetDisplayTag(SocketGuildUser user) {
            return $"{user.DisplayName}#{user.Discriminator}";
        }

        public static string FormatActivity(IActivity activity) {
            if (activity == null) {
                return "";
            }

            var str = "> ";
            switch (activity.Type) {
            case ActivityType.Competing:
                str += "Competing in ";

                break;
            case ActivityType.Listening:
                str += "Listening to ";

                break;
            case ActivityType.Playing:
                str += "Playing ";

                break;
            case ActivityType.Streaming:
                str += "Streaming on ";

                break;
            case ActivityType.Watching:
                str += "Watching ";

                break;
            }

            str += $"**{activity.Name}**";

            var details = activity.Details;
            if (!string.IsNullOrWhiteSpace(details)) {
                str += $"\n> `{details}`";
            }

            return str;
        }
    }
}