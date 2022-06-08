using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    public class UUser {
        public static IGuildUser GetUser(IGuildUser? user, SocketInteractionContext context) {
            if (user == null) {
                return context.Guild.GetUser(context.User.Id);
            } else {
                return user;
            }
        }

        public static EmbedBuilder ProfileEmbed(IGuildUser user) {
            var embed = new EmbedBuilder {
                Color = Colors.Blurple,
                Title = GetGuildTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            if (user.PublicFlags != null) {
                embed.Author = new EmbedAuthorBuilder {
                    Name = FormatFlag(user, user.PublicFlags.Value)
                };
            }

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(GetActivities(user).First());
            }

            if (user.VoiceChannel != null) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(user)
                    }
                );
            }

            return embed;
        }

        public static EmbedBuilder UserProfileEmbed(IGuildUser user) {
            var embed = new EmbedBuilder {
                Color = GetStatusColor(user),
                Title = GetTag(user),
                ThumbnailUrl = user.GetAvatarUrl(),
                Fields = {
                    new EmbedFieldBuilder {
                        Name = "Created At",
                        Value = $"`{user.CreatedAt.ToString("MMM dd, yyyy")}`"
                    }
                }
            };

            if (user.PublicFlags != null) {
                embed.Author = new EmbedAuthorBuilder {
                    Name = FormatFlag(user, user.PublicFlags.Value)
                };
            } 

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(GetActivities(user).First());
            }

            return embed;
        }

        public static EmbedBuilder GuildProfileEmbed(IGuildUser user) {
            var embed = new EmbedBuilder {
                Color = GetStatusColor(user),
                Author = new EmbedAuthorBuilder {
                    Name = GetTopRole(user).Name
                },
                Title = GetGuildTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(GetActivities(user).First());
            }
            
            if (user.VoiceChannel != null) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(user)
                    }
                );
            }

            if (user.JoinedAt != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Joined At",
                        Value = $"`{user.JoinedAt.Value.ToString("MMM dd, yyyy")}`"
                    }
                );
            }

            return embed;
        }

        
        public static EmbedBuilder AllProfileEmbed(IGuildUser user) {
            var embed= new EmbedBuilder() {
                Color = GetStatusColor(user),
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = GetTag(user),
                ThumbnailUrl = user.GetDisplayAvatarUrl(),
                Description = FormatActivity(GetActivities(user).First()),
                Fields = {
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = "Created At",
                        Value = $"`{user.CreatedAt.ToString("MMM dd, yyyy")}`"
                    }
                }
            };

            if (user.JoinedAt != null) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = "Joined at",
                        Value = $"`{user.JoinedAt.Value.ToString("MMM dd, yyyy")}`"
                    }
                );
            }

            if (user.PublicFlags != null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = "Public Flag",
                        Value = $"`{FormatFlag(user, user.PublicFlags.Value)}`"
                    }
                );
            }

            var roles = GetRoles(user);
            if (roles.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = $"Roles [{roles.Count}]",
                        Value = FormatRoles(roles)
                    }
                );
            }

            if (user.VoiceChannel != null) {
                embed.AddField(
                    new EmbedFieldBuilder() {
                        IsInline = true,
                        Name = "Voice Channel",
                        Value = FormatVoiceChannel(user)
                    }
                );
            }

            return embed;
        }

        public static string FormatVoiceChannel(IGuildUser user) {
            var channel = user.VoiceChannel;

            var limit = channel.UserLimit.ToString();
            if (channel.UserLimit == null) {
                limit = "No limit";
            }

            var region = channel.RTCRegion;
            if (string.IsNullOrWhiteSpace(region)) {
                region = "No region";
            }

            return $"{user.VoiceChannel.Mention}\n"
                + $"**Limit** `{limit}`\n"
                + $"**Bitrate** `{user.VoiceChannel.Bitrate}`\n"
                + $"**Region** `{region}`";
        }

        public static Color GetStatusColor(IGuildUser user) {
            switch (user.Status) {
            case UserStatus.Online: return Colors.Online;
            case UserStatus.DoNotDisturb: return Colors.Offline;
            case UserStatus.Idle: return Colors.Idle;
            case UserStatus.AFK: return Colors.Idle;
            default: return Colors.Grayple;
            }
        }

        public static string FormatFlag(IGuildUser user, UserProperties properties) {
            switch (user.PublicFlags) {
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

        public static List<SocketRole> GetRoles(IGuildUser user) {
            var roles = new List<SocketRole>();
            foreach (var role in ((SocketGuildUser) user).Roles) {
                if (!role.IsEveryone) {
                    roles.Add(role);
                }
            }

            roles.Sort();
            roles.Reverse();

            return roles;
        }

        public static string FormatRoles(List<SocketRole> roles) {
            var str = "";
            foreach (var role in roles) {
                if (!role.IsEveryone) {
                    str += $"{role.Mention} ";
                }
            }

            return str;
        }

        public static SocketRole GetTopRole(IGuildUser user) {
            var posList = new Dictionary<int, SocketRole>();
            foreach (var role in ((SocketGuildUser) user).Roles) {
                posList.Add(role.Position, role);
            }

            return posList[posList.Keys.Max()];
        }

        public static string GetTag(IGuildUser user) {
            return $"{user.Username}#{user.Discriminator}";
        }

        public static string GetGuildTag(IGuildUser user) {
            return $"{user.DisplayName}#{user.Discriminator}";
        }

        public static List<IActivity> GetActivities(IGuildUser user) {
            var activities = new List<IActivity>();
            foreach (var activity in user.Activities) {
                activities.Add(activity);
            }

            return activities;
        }

        public static string FormatActivity(IActivity activity) {
            if (activity == null) {
                return "";
            }

            var str = ">>> ";
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
            if (!string.IsNullOrWhiteSpace(activity.Details)) {
                str += $"\n{activity.Details}";
            }

            return str;
        }
    }
}