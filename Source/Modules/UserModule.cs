using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Services;
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
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            var embed = new EmbedBuilder {
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder {
                    Text = $"Joined at {user.JoinedAt.ToString().Split(' ')[0]}"
                }
            };

            if (user.Activities.Count != 0) {
                var activity = user.Activities.First();

                embed.Description += "> ";
                switch (activity.Type) {
                case ActivityType.Competing:
                    embed.Description += "Competing in ";

                    break;
                case ActivityType.Listening:
                    embed.Description += "Listening to ";

                    break;
                case ActivityType.Playing:
                    embed.Description += "Playing ";

                    break;
                case ActivityType.Streaming:
                    embed.Description += "Streaming on ";

                    break;
                case ActivityType.Watching:
                    embed.Description += "Watching ";

                    break;
                }

                embed.Description += $"**{activity.Name}**\n";
                if (!string.IsNullOrWhiteSpace(activity.Details)) {
                    embed.Description += $"> {activity.Details}\n\n";
                } else {
                    embed.Description += "\n";
                }
            }

            embed.Description += $"Created at `{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}`\n";

            switch (user.Status) {
            case UserStatus.Online:
                embed.Color = Color.Green;

                break;
            case UserStatus.Idle:
                embed.Color = Color.Orange;

                break;
            case UserStatus.AFK:
                embed.Color = Color.Orange;

                break;
            case UserStatus.DoNotDisturb:
                embed.Color = Color.Red;

                break;
            default:
                embed.Color = Color.DarkerGrey;

                break;
            }

            // TODO: Do something with this shit
            // var genField = new EmbedFieldBuilder() {
            //     IsInline = true,
            //     Name = "General",
            //     Value = ">>> "
            // };
            //
            // if (user.PremiumSince != null) {
            //     genField.Value += $"Premium since `{user.PremiumSince.ToString().Split(' ')[0]}\n";
            // }
            //
            // switch (user.PublicFlags) {
            // case UserProperties.HypeSquadBalance:
            //     genField.Value += "Hype Squad `Balance`\n";
            //
            //     break;
            // case UserProperties.HypeSquadBravery:
            //     genField.Value += "Hype Squad `Bravery`\n";
            //     
            //     break;
            // case UserProperties.HypeSquadBrilliance:
            //     genField.Value += "Hype Squad `Brilliance`\n";
            //     
            //     break;
            // }
            // if ((user.PublicFlags & UserProperties.HypeSquadEvents) != 0) {
            //     genField.Value += "Hype Squad Events `true`";
            // }
            // switch (user.PublicFlags) {
            // case UserProperties.BugHunterLevel1:
            //     genField.Value += "Bug Hunter Level `1`\n";
            //     
            //     break;
            // case UserProperties.BugHunterLevel2:
            //     genField.Value += "Bug Hunter Level `2`\n";
            //     
            //     break;
            // default:
            //     genField.Value += "\n";
            //     
            //     break;
            // }
            //
            // embed.AddField(genField);

            var author = new EmbedAuthorBuilder();

            if (user.IsBot) {
                author.Name = $"[BOT] {user.Id}";
            } else if (user.IsWebhook) {
                author.Name = $"[WEB_HOOK] {user.Id}";
            } else {
                author.Name = $"{user.Id}";
            }

            embed.WithAuthor(author);

            var hasFlags = false;
            var flags = new List<string>();

            if ((user.PublicFlags & UserProperties.Partner) != 0) {
                hasFlags = true;
                flags.Add("`Partner`");
            }

            if ((user.PublicFlags & UserProperties.Staff) != 0) {
                hasFlags = true;
                flags.Add("`Staff`");
            }

            if ((user.PublicFlags & UserProperties.System) != 0) {
                hasFlags = true;
                flags.Add("`System`");
            }

            if ((user.PublicFlags & UserProperties.EarlySupporter) != 0) {
                hasFlags = true;
                flags.Add("`Early Supporter`");
            }

            if ((user.PublicFlags & UserProperties.TeamUser) != 0) {
                hasFlags = true;
                flags.Add("`Team User`");
            }

            if ((user.PublicFlags & UserProperties.VerifiedBot) != 0) {
                hasFlags = true;
                flags.Add("`Verified Bot`");
            }

            if ((user.PublicFlags & UserProperties.DiscordCertifiedModerator) != 0) {
                hasFlags = true;
                flags.Add("`Discord Certified Moderator`");
            }

            if ((user.PublicFlags & UserProperties.EarlyVerifiedBotDeveloper) != 0) {
                hasFlags = true;
                flags.Add("`Early Verified Bot Developer`");
            }

            if ((user.PublicFlags & UserProperties.BotHTTPInteractions) != 0) {
                hasFlags = true;
                flags.Add("`Bot HTTP Interactions`");
            }

            if (hasFlags) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"Flags [{flags.Count}]",
                        Value = $">>> {string.Join(' ', flags)}"
                    }
                );
            }

            var hasStates = false;
            var states = new List<string>();

            if (user.VoiceChannel != null) {
                hasStates = true;
                states.Add("`Voice-Channel`");
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
                states.Add("`Self-Muted`");
            }

            if (user.IsSelfDeafened) {
                hasStates = true;
                states.Add("`Self-Deafened`");
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

            if (hasStates) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        IsInline = true,
                        Name = $"States [{states.Count}]",
                        Value = $">>> {string.Join(' ', states)}"
                    }
                );
            }

            var roles = ((SocketGuildUser) user).Roles;

            if (roles.Count != 1) {
                var roleField = new EmbedFieldBuilder {
                    IsInline = true,
                    Name = $"Roles [{roles.Count}]",
                    Value = ">>>  "
                };

                foreach (var role in roles) {
                    if (!role.IsEveryone) {
                        roleField.Value += $"{role.Mention} ";
                    }
                }

                embed.AddField(roleField);
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [SlashCommand("roles", "Get or set a role to the user.")]
        public async Task Roles(IGuildUser user = null, IRole role = null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            var embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl()
            };

            if (role == null) {
                var userRoles = ((SocketGuildUser) user).Roles;
                var field = new EmbedFieldBuilder {
                    IsInline = true,
                    Name = $"Roles [{userRoles.Count}]",
                    Value = ">>>  "
                };

                foreach (var userRole in userRoles) {
                    if (!userRole.IsEveryone) {
                        field.Value += $"{userRole.Mention} ";
                    }
                }

                embed.AddField(field);
            } else {
                var field = new EmbedFieldBuilder {
                    IsInline = true,
                    Value = $">>> {role.Mention}"
                };

                if (user.RoleIds.Contains(role.Id)) {
                    try {
                        await user.RemoveRoleAsync(role);
                        
                        field.Name = "Removed";
                    } catch {
                        embed = new EmbedBuilder {
                            Color = Color.Red,
                            Title = $"{role.Name} is not removable.",
                            Description = $"{role.Mention}"
                        };

                        await RespondAsync(
                            ephemeral: true,
                            embed: embed.Build()
                        );

                        return;
                    }
                } else {
                    await user.AddRoleAsync(role);

                    field.Name = "Added";
                }

                embed.AddField(field);
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [SlashCommand("perms", "Get or set the permissions of the user.")]
        public async Task Perms(IGuildUser user = null) {
            // TODO: Perms method
        }

        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        [SlashCommand("nick", "Get or set the nickname of the user.")]
        public async Task Nick(IGuildUser user = null, string nickname = null) {
            var embed = new EmbedBuilder();

            if (nickname.Length > 32) {
                embed = new EmbedBuilder {
                    Color = Color.Red,
                    Title = "The nickname's length is out of range.",
                    Description = "`nickname.Length < 32`"
                };

                await RespondAsync(
                    embed: embed.Build()
                );

                return;
            }

            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl()
            };

            var field = new EmbedFieldBuilder {
                Name = "Nickname",
                Value = ">>> "
            };

            if (string.IsNullOrWhiteSpace(nickname)) {
                if (string.IsNullOrWhiteSpace(user.Nickname)) {
                    field.Value += "`No nickname`";
                } else {
                    field.Value += $"`{user.Nickname}`";
                }
            } else {
                user.ModifyAsync(x =>
                    x.Nickname = nickname
                );

                field.Value += $"`{user.Nickname}`";
            }

            embed.AddField(field);

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [RequireUserPermission(GuildPermission.MuteMembers)]
        [SlashCommand("mute", "If the user is muted, mute; Otherwise, un-mute.")]
        public async Task Mute(IGuildUser user = null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            var embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl()
            };

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

        [RequireUserPermission(GuildPermission.DeafenMembers)]
        [SlashCommand("deafen", "If the user is deafened, un-deafen; Otherwise, deafen.")]
        public async Task Deafen(IGuildUser user = null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            var embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl()
            };

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

        [RequireUserPermission(ChannelPermission.MoveMembers)]
        [SlashCommand("move", "If the channel is null, disconnect the user; Otherwise, move the user to the channel.")]
        public async Task Move(IGuildUser user = null, IVoiceChannel? channel = null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }

            var embed = new EmbedBuilder {
                Color = MyColor.Blurple,
                Author = new EmbedAuthorBuilder {
                    Name = user.Id.ToString()
                },
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl()
            };

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

        [RequireUserPermission(GuildPermission.KickMembers)]
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

        [RequireUserPermission(GuildPermission.BanMembers)]
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
    }
}