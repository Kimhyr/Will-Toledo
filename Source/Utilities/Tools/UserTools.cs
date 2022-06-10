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
                Color = GetStatusColor(user),
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetDisplayAvatarUrl()
            };

            var flags = user.PublicFlags;
            if (flags != null) {
                embed.Author = new EmbedAuthorBuilder() {
                    Name = FormatFlag(flags.Value)
                };
            } else {
                embed.Author = new EmbedAuthorBuilder() {
                    Name = user.Id.ToString()
                };
            }

            if (user.Activities.Count > 0) {
                embed.Description = FormatActivity(user.Activities.First());
            }

            return embed;
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