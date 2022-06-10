using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PenileNET.Utilities;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    [Group("user", "Commands for getting and manipulating users.")]
    public class UserModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("display", "Displays the user's information.")]
        public async Task Display(
            SocketGuildUser? user = null,
            [Choice("all", 0)] [Choice("user", 1)] [Choice("guild", 2)]
            int? option = null
        ) {
            user = UserTools.GetUser(user, Context);

            switch (option) {
            case 0:
                if (user.Status == UserStatus.Offline || user.Status ==  UserStatus.Invisible) {
                    await RespondAsync(
                        embed: UserTools.ProfileEmbed(user).Build()
                    );
                }

                var embed = new EmbedBuilder {
                    Color = UserTools.GetStatusColor(user),
                    Author = new EmbedAuthorBuilder {
                        Name = user.Id.ToString()
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    ThumbnailUrl = user.GetDisplayAvatarUrl()
                };
                
                if (user.Nickname != null) {
                    embed.Description += $"**Nickname** {user.Mention}\n";
                }

                var flags = user.PublicFlags;
                if (flags != null) {
                    embed.Description += $"**Flag** `{UserTools.FormatFlag(flags.Value)}`\n";
                }
                
                var activity = user.Activities.First();
                if (activity != null) {
                    embed.Description += $"\n{UserTools.FormatActivity(activity)}\n\n";
                }

                embed.Description += $"**Created at** `{user.CreatedAt.ToString("MMM d, yyyy")}`\n";

                var joined = user.JoinedAt;
                if (joined != null) {
                    embed.Description += $"**Joined at** `{joined.Value.ToString("MMM d, yyyy")}`\n";
                }           

                var roles = user.Roles;
                if (roles.Count > 0) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = $"Roles [{roles.Count}]",
                            Value = UserTools.FormatRoles(roles)
                        }
                    );
                }
                

                var channel = user.VoiceChannel;
                if (channel != null) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Voice Channel",
                            Value = GuildTools.FormatVoiceChannel(channel)
                        }
                    );
                }

                await RespondAsync(
                    embed: embed.Build()
                );

                break;
            case 1:
                embed = new EmbedBuilder {
                    Color = UserTools.GetStatusColor(user),
                    Author = new EmbedAuthorBuilder() {
                        Name = user.Id.ToString()
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    ThumbnailUrl = user.GetAvatarUrl()
                };

                flags = user.PublicFlags;
                if (flags != null) {
                    embed.Author = new EmbedAuthorBuilder {
                        Name = UserTools.FormatFlag(flags.Value)
                    };
                } 
                
                activity = user.Activities.First();
                if (activity != null) {
                    embed.Description += UserTools.FormatActivity(activity);
                }

                channel = user.VoiceChannel;
                if (channel != null) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Voice Channel",
                            Value = GuildTools.FormatVoiceChannel(channel)
                        }
                    );
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Created At",
                            Value = $"`{user.CreatedAt.ToString("MMM d, yyyy")}`"
                        }
                    );

                await RespondAsync(
                    embed: embed.Build()
                );

                break;
            case 2:
                embed = new EmbedBuilder {
                    Color = UserTools.GetStatusColor(user),
                    Author = new EmbedAuthorBuilder {
                        Name = GeneralTools.GetSorted(user.Roles.ToList()).First().Name
                    },
                    Title = $"{user.DisplayName}#{user.Discriminator}",
                    ThumbnailUrl = user.GetDisplayAvatarUrl()
                };

                activity = user.Activities.First();
                if (activity != null) {
                    embed.Description += UserTools.FormatActivity(activity);
                }

                channel = user.VoiceChannel;
                if (channel != null) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Voice Channel",
                            Value = GuildTools.FormatVoiceChannel(channel)
                        }
                    );
                }

                joined = user.JoinedAt;
                if (joined != null) {
                    embed.AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Joined At",
                            Value = $"`{joined.Value.ToString("MMM d, yyyy")}`"
                        }
                    );
                }

                await RespondAsync(
                    embed: embed.Build()
                );

                break;
            default:
                await RespondAsync(
                    embed: UserTools.ProfileEmbed(user).Build()
                );
 
                break;
            }
        }

        [SlashCommand("nick", "Display or set the user's nickname.")]
        public async Task Nick(SocketGuildUser? user = null, string? nickname = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (nickname == null) {
                var field = new EmbedFieldBuilder {
                    Name = "Nickname"
                };

                if (string.IsNullOrWhiteSpace(user.Nickname)) {
                    field.Value = "`No nickname`";
                } else {
                    field.Value = user.Mention;
                }

                embed.AddField(field);
            } else {
                if (user.GuildPermissions.ManageNicknames) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Nickname = nickname
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error setting the user's nickname."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Nickname Set",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("roles", "Display or modify the user's roles.")]
        public async Task Roles(SocketGuildUser? user = null, SocketRole? role = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);
            
            var roles = user.Roles;
            if (role == null) {
                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = $"Roles [{roles.Count}]",
                        Value = UserTools.FormatRoles(roles)
                    }
                );
            } else {
                if (user.GuildPermissions.ManageRoles) {
                    return;
                }

                if (roles.Contains(role)) {
                    try {
                        await user.RemoveRoleAsync(role);
                    } catch {
                        await RespondAsync(
                            embed: new EmbedBuilder {
                                Color = Colors.Offline,
                                Title = "There was an error removing the role from the user."
                            }.Build(),
                            ephemeral: true
                        );

                        return;
                    }

                    embed.AddField(
                        new EmbedFieldBuilder {
                            Name = "Role Removed",
                            Value = role.Mention
                        }
                    );
                } else {
                    try {
                        await user.AddRoleAsync(role);
                    } catch {
                        await RespondAsync(
                            embed: new EmbedBuilder {
                                Color = Colors.Offline,
                                Title = "There was an error adding the role to the user."
                            }.Build(),
                            ephemeral: true
                        );

                        return;
                    }

                    embed.AddField(
                        new EmbedFieldBuilder {
                            Name = "Role Added",
                            Value = role.Mention
                        }
                    );
                }
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("move", "If the channel is not specified, disconnect the user; Otherwise, move the user to that channel")]
        public async Task Move(SocketGuildUser? user = null, SocketVoiceChannel? voiceChannel = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            var channel = user.VoiceChannel;
            if (channel == null) {
                await RespondAsync(
                    embed: new EmbedBuilder() {
                        Color = Colors.Offline,
                        Title = "The user must be in a voice channel."
                    }.Build(),
                    ephemeral: true
                );

                return;
            } else {
                if (voiceChannel == null) {
                    await channel.DisconnectAsync();

                    embed.AddField(
                        new EmbedFieldBuilder() {
                            Name = "Disconnected",
                            Value = channel.Mention
                        }
                    );
                } else {
                    await user.ModifyAsync(x => 
                        x.Channel = voiceChannel
                    );

                    embed.AddField(
                        new EmbedFieldBuilder() {
                            Name = "Moved",
                            Value = channel.Mention
                        }
                    ); 
                }
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("mute", "If the user is muted, un-mute. Otherwise, mute.")]
        public async Task Mute(SocketGuildUser? user = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (user.IsMuted) {
                try {
                    await user.ModifyAsync(x =>
                        x.Mute = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error un-muting the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Un-muted",
                        Value = user.Mention
                    }
                );
            } else {
                if (user.GuildPermissions.MuteMembers) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Mute = true
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error muting the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Muted",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [SlashCommand("deafen", "If the user is deafened, un-deafen. Otherwise, deafen.")]
        public async Task Deafen(SocketGuildUser? user = null) {
            user = UserTools.GetUser(user, Context);
            var embed = UserTools.ProfileEmbed(user);

            if (user.IsDeafened) {
                try {
                    await user.ModifyAsync(x =>
                        x.Deaf = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error un-deafening the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Un-deafened",
                        Value = user.Mention
                    }
                );
            } else {
                if (user.GuildPermissions.DeafenMembers) {
                    return;
                }

                try {
                    await user.ModifyAsync(x =>
                        x.Deaf = false
                    );
                } catch {
                    await RespondAsync(
                        embed: new EmbedBuilder {
                            Color = Colors.Offline,
                            Title = "There was an error deafening the user."
                        }.Build(),
                        ephemeral: true
                    );

                    return;
                }

                embed.AddField(
                    new EmbedFieldBuilder {
                        Name = "Deafened",
                        Value = user.Mention
                    }
                );
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.KickMembers)]
        [SlashCommand("kick", "Kicks the user.")]
        public async Task KicK(SocketGuildUser user, string reason = "There is no reason.") {
            user = UserTools.GetUser(user, Context);

            try {
                await user.KickAsync(reason);
            } catch {
                await RespondAsync(
                    embed: new EmbedBuilder {
                        Color = Colors.Offline,
                        Title = "There was an error kicking the user."
                    }.Build(),
                    ephemeral: true
                );

                return;
            }

            await RespondAsync(
                embed: UserTools.ProfileEmbed(user)
                    .AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Kicked",
                            Value = $"```{reason}```"
                        }
                    ).Build()
            );
        }

        [DefaultMemberPermissions(GuildPermission.BanMembers)]
        [SlashCommand("ban", "Bans the user.")]
        public async Task Ban(
            SocketGuildUser user,
            [MinValue(0)] [MaxValue(7)] int days = 0,
            string reason = "There is no reason."
        ) {
            user = UserTools.GetUser(user, Context);

            try {
                await user.BanAsync(days, reason);
            } catch {
                await RespondAsync(
                    embed: new EmbedBuilder {
                        Color = Colors.Offline,
                        Title = "There was an error banning the user."
                    }.Build(),
                    ephemeral: true
                );

                return;
            }

            await RespondAsync(
                embed: UserTools.ProfileEmbed(user)
                    .AddField(
                        new EmbedFieldBuilder {
                            IsInline = true,
                            Name = "Banned",
                            Value = $"```{reason}```"
                        }
                    ).Build()
            );
        }
    }
}