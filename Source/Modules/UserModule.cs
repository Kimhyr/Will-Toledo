using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Services;
using PenileNET.Utilities;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    [Group("user", "Commands for getting and manipulating users.")]
    public class UserModule : InteractionModuleBase<SocketInteractionContext> {
        private InteractionHandler _handler;

        public UserModule(InteractionHandler handler) {
            _handler = handler;
        }

        public InteractionService Commands { get; set; }

        [SlashCommand("info", "Gets the user's information.")]
        public async Task Info(IGuildUser user = null) {
            user = Utility.CheckUser(user, Context);

            var embed = new EmbedBuilder {
                Color = GetUserStatus(user),
                Author = new EmbedAuthorBuilder {
                    Name = $"[{GetUserType(user)}] {user.Id}"
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder {
                    Text = $"Joined at {user.JoinedAt.ToString().Split(' ')[0]}"
                }
            };

            var activities = GetUserActivities(user);
            if (activities.Count > 0) {
                embed.Description += $"{activities.First()}\n\n";
            }

            embed.Description += $"Created at `{Utility.ConvertDateTimeOffset(user.CreatedAt)}`";

            var roles = GetUserRoles(user);
            if (roles.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Roles [{roles.Count}]",
                        Value = $">>> {string.Join(' ', roles)}"
                    }
                );
            }

            var states = GetUserStates(user);
            if (states.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"States [{states.Count}]",
                        Value = $">>> {string.Join(' ', states)}"
                    }
                );
            }

            var perms = GetUserPerms(user);
            if (perms.Count > 0) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = false,
                        Name = $"Permissions [{perms.Count}]",
                        Value = $">>> {string.Join(' ', perms)}"
                    }
                );
            }

            await RespondAsync( 
                embed: embed.Build()
            );
        }

        [SlashCommand("nick", "Get or set the user's nickname.")]
        public async Task Nick(IGuildUser user = null, string nickname = null) {
            user = Utility.CheckUser(user, Context);

            var embed = Response.User(user);

            if (nickname == null) {
                var field = new EmbedFieldBuilder() {
                    Name = "Nickname"
                };
                
                if (user.Nickname != null) {
                    field.Value = $"`{user.Nickname}`";
                } else {
                    field.Value = "`No nickname`";
                }

                embed.AddField(field);
            } else {
                await user.ModifyAsync(x => 
                    x.Nickname = nickname
                );

                embed.AddField(
                    new EmbedFieldBuilder() {
                        Name = "Nickname Set",
                        Value = $"`{user.Nickname}`"
                    }    
                );
            }

            await RespondAsync(
                embed: embed.Build()   
            );
        }
        
        [DefaultMemberPermissions(GuildPermission.ManageRoles)]
        [SlashCommand("roles", "Get or set the user's roles.")]
        public async Task Perms(IGuildUser user = null, IRole role = null) {
            user = Utility.CheckUser(user, Context);

            var embed = Response.User(user);

            if (role == null) {
                var roles = GetUserRoles(user);
                if (roles.Count > 0) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            Name = $"Roles [{roles.Count}]",
                            Value = $">>> {string.Join(' ', roles)}"
                        }
                    );
                }
            } else {
                await user.AddRoleAsync(role);

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Role Added",
                        Value = $">>> {role.Mention}"
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.MuteMembers)]
        [SlashCommand("mute", "If the user is muted, mute; Otherwise, un-mute.")]
        public async Task Mute(IGuildUser user = null) {
            user = Utility.CheckUser(user, Context);

            var embed = Response.User(user);

            if (user.IsMuted) {
                await user.ModifyAsync(x =>
                    x.Mute = false
                );

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Muted",
                        Value = "> `false`"
                    }
                );
            } else {
                await user.ModifyAsync(x =>
                    x.Mute = true
                );

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Muted",
                        Value = "> `true`"
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.MuteMembers)]
        [SlashCommand("deafen", "If the user is deafened, un-deafen; Otherwise, deafen.")]
        public async Task Deafen(IGuildUser user = null) {
            user = Utility.CheckUser(user, Context);

            var embed = Response.User(user);

            if (user.IsDeafened) {
                await user.ModifyAsync(x =>
                    x.Deaf = false
                );

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Deafened",
                        Value = "> `false`"
                    }
                );
            } else {
                await user.ModifyAsync(x =>
                    x.Deaf = true
                );

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Deafened",
                        Value = "> `true`"
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.MuteMembers)]
        [SlashCommand("move", "If the channel is null, disconnect the user; Otherwise, move the user to the channel.")]
        public async Task Move(IGuildUser user = null, IVoiceChannel? channel = null) {
            user = Utility.CheckUser(user, Context);

            var embed = Response.User(user);

            if (channel == null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Disconnected",
                        Value = $"> {user.VoiceChannel.Mention}"
                    }
                );

                await user.ModifyAsync(x =>
                    x.ChannelId = null
                );
            } else {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Moved",
                        Value = $"> {user.VoiceChannel.Mention} {Symbol.RightArrow} {channel.Mention}"
                    }
                );

                await user.ModifyAsync(x =>
                    x.ChannelId = channel.Id
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.MuteMembers)]
        [SlashCommand("kick", "Kicks the user.")]
        public async Task Kick(IGuildUser user, string? reason = null) {
            try {
                await user.KickAsync(reason);

                var embed = new EmbedBuilder {
                    Color = MyColor.Blurple,
                    Author = new EmbedAuthorBuilder {
                        Name = user.Id.ToString()
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    ThumbnailUrl = user.GetAvatarUrl()
                };

                var field = new EmbedFieldBuilder {
                    Name = "Kicked",
                    Value = ">>> "
                };

                if (!string.IsNullOrWhiteSpace(reason)) {
                    field.Value += $"```{reason}```";
                } else {
                    field.Value += "`No reason.`";
                }

                embed.AddField(field);

                await RespondAsync(
                    embed: embed.Build()
                );
            } catch {
                var embed = new EmbedBuilder {
                    Color = Color.Red,
                    Title = $"{user.Username}#{user.Discriminator} is not kick-able.",
                    Description = $"{user.Mention}"
                };

                await RespondAsync(
                    ephemeral: true,
                    embed: embed.Build()
                );
            }
        }

        [DefaultMemberPermissions(GuildPermission.MuteMembers)]
        [SlashCommand("ban", "Bans the user.")]
        public async Task Ban(IGuildUser user, [MinValue(0)] [MaxValue(7)] int pruneDays = 0, string? reason = null) {
            try {
                await user.BanAsync(pruneDays, reason);

                var embed = new EmbedBuilder {
                    Color = MyColor.Blurple,
                    Author = new EmbedAuthorBuilder {
                        Name = user.Id.ToString()
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    ThumbnailUrl = user.GetAvatarUrl()
                };

                var field = new EmbedFieldBuilder {
                    Name = "Banned",
                    Value = ">>> "
                };

                if (!string.IsNullOrWhiteSpace(reason)) {
                    field.Value += $"```{reason}```";
                } else {
                    field.Value += "`No reason.`";
                }

                embed.AddField(field);

                await RespondAsync(
                    embed: embed.Build()
                );
            } catch {
                var embed = new EmbedBuilder {
                    Color = Color.Red,
                    Title = $"{user.Username}#{user.Discriminator} is not Ban-able.",
                    Description = $"{user.Mention}"
                };

                await RespondAsync(
                    ephemeral: true,
                    embed: embed.Build()
                );
            }
        }

        private Color GetUserStatus(IGuildUser user) {
            switch (user.Status) {
            case UserStatus.Online: return Color.Green;
            case UserStatus.Idle: return Color.Orange;
            case UserStatus.AFK: return Color.Orange;
            case UserStatus.DoNotDisturb: return Color.Red;
            default: return Color.DarkerGrey;
            }
        }

        private string GetUserType(IGuildUser user) {
            if (user.IsBot) {
                return "BOT";
            }

            if (user.IsWebhook) {
                return "WEBHOOK";
            }

            return "USER";
        }

        private List<string> GetUserActivities(IGuildUser user) {
            var activities = new List<string>();

            foreach (var userActivity in user.Activities) {
                var activity = "";

                switch (userActivity.Type) {
                case ActivityType.Competing:
                    activity += "> Competing in ";

                    break;
                case ActivityType.Listening:
                    activity += "> Listening to ";

                    break;
                case ActivityType.Playing:
                    activity += "> Playing ";

                    break;
                case ActivityType.Streaming:
                    activity += "> Streaming on ";

                    break;
                case ActivityType.Watching:
                    activity += "> Watching ";

                    break;
                }

                activity += $"**{userActivity.Name}**";
                if (!string.IsNullOrWhiteSpace(userActivity.Details)) {
                    activity += $"\n> {userActivity.Details}";
                }

                activities.Add(activity);
            }

            return activities;
        }

        private List<string> GetUserRoles(IGuildUser user) {
            var roles = new List<string>();
            var userRoles = ((SocketGuildUser) user).Roles;

            foreach (var role in userRoles) {
                roles.Add(role.Mention);
            }

            return roles;
        }

        private List<string> GetUserStates(IGuildUser user) {
            var states = new List<string>();
            var hasStates = false;

            if (user.VoiceChannel != null) {
                hasStates = true;
                states.Add("`VoiceChannel`");
            }

            if ((bool) user.IsPending) {
                hasStates = true;
                states.Add("`Pending`");
            }

            if (user.IsStreaming) {
                hasStates = true;
                states.Add("`Streaming`");
            }

            if (user.IsVideoing) {
                hasStates = true;
                states.Add("`Videoing`");
            }

            if (user.IsSelfMuted) {
                hasStates = true;
                states.Add("`SelfMuted`");
            }

            if (user.IsSelfDeafened) {
                hasStates = true;
                states.Add("`SelfDeafened`");
            }

            if (user.IsMuted) {
                hasStates = true;
                states.Add("`Muted`");
            }

            if (user.IsDeafened) {
                hasStates = true;
                states.Add("`Deafened`");
            }

            if (user.IsSuppressed) {
                hasStates = true;
                states.Add("`Suppressed`");
            }

            return states;
        }

        private List<string> GetUserPerms(IGuildUser user) {
            var perms = new List<string>();

            foreach (var perm in user.GuildPermissions.ToList()) {
                if (!perm.ToString().All(char.IsDigit)) {
                    perms.Add($"`{perm.ToString()}`");
                }
            }

            return perms;
        }
    }
}