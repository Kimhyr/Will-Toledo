using System;
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
        public InteractionService Commands { get; set; }
        private InteractionHandler _handler;

        public UserModule(InteractionHandler handler) {
            _handler = handler;
        }

        [SlashCommand("info", "Gets the user's information.")]
        public async Task Info(IGuildUser user=null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }
            
            var embed = new EmbedBuilder {
                Title = $"{user.Username}#{user.Discriminator}",
                ThumbnailUrl = user.GetAvatarUrl(),
                Description = $"Created at: `{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}`\n",
                Footer = new EmbedFooterBuilder() {
                    Text = $"Joined at: {user.JoinedAt.ToString().Split(' ')[0]}\n"
                }
            };

            if (user.PremiumSince != null) {
                embed.Description += $"Premium since: `{user.PremiumSince.ToString().Split(' ')[0]}\n\n";
            } else {
                embed.Description += "\n";
            }

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

            var author = new EmbedAuthorBuilder();
            
            if (user.IsBot) {
                author.Name = $"[BOT] {user.Id}";
            } else if (user.IsWebhook) {
                author.Name = $"[WEB_HOOK] {user.Id}";
            } else {
                author.Name = $"{user.Id}";
            }

            embed.WithAuthor(author);

            if (user.Activities.Count != 0) {
                var activity = user.Activities.First();

                embed.Description += ">>> ";
                switch (activity.Type) {
                case ActivityType.Competing:
                    embed.Description += $"Competing in ";
                    
                    break;
                case ActivityType.Listening:
                    embed.Description += $"Listening to ";
                    
                    break;
                case ActivityType.Playing:
                    embed.Description += $"Playing ";
                    
                    break;
                case ActivityType.Streaming:
                    embed.Description += $"Streaming on ";
                    
                    break;
                case ActivityType.Watching:
                    embed.Description += $"Watching ";
                    
                    break;
                }
                embed.Description += $"**{activity.Name}**\n";
                if (!string.IsNullOrWhiteSpace(activity.Details)) {
                    embed.Description += $"{activity.Details}\n";
                }
            }
            
            var roles = ((SocketGuildUser) user).Roles;
            var roleField = new EmbedFieldBuilder() {
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

            var hasFlags = false;
            var flagList = new List<string>();
            
            if (user.VoiceChannel != null) {
                hasFlags = true;
                flagList.Add("`Voice-Channel`");
            }
            if (user.IsStreaming) {
                hasFlags = true;
                flagList.Add("`Streaming`");
            }
            if (user.IsVideoing) {
                hasFlags = true;
                flagList.Add("`Videoing`");
            }
            if (user.IsSelfMuted) {
                hasFlags = true;
                flagList.Add("`Self-Muted`");
            }
            if (user.IsSelfDeafened) {
                hasFlags = true;
                flagList.Add("`Self-Deafened`");
            }
            if (user.IsMuted) {
                hasFlags = true;
                flagList.Add("`Muted`");
            }
            if (user.IsDeafened) {
                hasFlags = true;
                flagList.Add("`Deafened`");
            }
            if (user.IsSuppressed) {
                hasFlags = true;
                flagList.Add("`Suppressed`");
            }

            if (hasFlags) {
                var flagField = new EmbedFieldBuilder() {
                    IsInline = true,
                    Name = $"Flags [{flagList.Count}]",
                    Value = $">>> {string.Join(' ', flagList)}"
                };
            
                embed.AddField(flagField);
            }

            await RespondAsync(
                embed: embed.Build()
            );
        }
        
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [SlashCommand("roles", "Get or set role to the user.")]
        public async Task Roles(IGuildUser user=null, IRole role=null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }
            
            var embed = new EmbedBuilder() {
                Color = Color.Green,
                Author = new EmbedAuthorBuilder() {
                    Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                },
                Title = $"{user.Username}#{user.Discriminator}",
                Description = user.Mention
            }; 
            
            if (role == null) {
                var userRoles = ((SocketGuildUser) user).Roles;
                var field = new EmbedFieldBuilder() {
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
                var field = new EmbedFieldBuilder() {
                    IsInline = true,
                    Value = $">>> {role.Mention}"
                };
                
                if (user.RoleIds.Contains(role.Id)) {
                    try {
                        await user.RemoveRoleAsync(role);
                    } catch {
                        embed = new EmbedBuilder() {
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

                    field.Name = "Removed";
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

        [RequireUserPermission(GuildPermission.MuteMembers)]
        [SlashCommand("mute", "If the user is muted, mute; Otherwise, un-mute.")]
        public async Task Mute(IGuildUser user=null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }
            
            var embed = new EmbedBuilder() {
                Color = Color.Green,
                Title = $"{user.Username}#{user.Discriminator}"
            };
            
            if (user.IsMuted) {
                await user.ModifyAsync(x =>
                    x.Mute = false
                );

                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Un-Muted"
                };
            } else {
                await user.ModifyAsync(x =>
                    x.Mute = true
                );

                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Muted"
                };
            }
            
            await RespondAsync(
                embed: embed.Build()
            );
        }
        
        [RequireUserPermission(GuildPermission.DeafenMembers)]
        [SlashCommand("deafen", "If the user is deafened, un-deafen; Otherwise, deafen.")]
        public async Task Deafen(IGuildUser user=null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }
            
            var embed = new EmbedBuilder() {
                Color = Color.Green,
                Title = $"{user.Username}#{user.Discriminator}"
            };
            
            if (user.IsDeafened) {
                await user.ModifyAsync(x => 
                    x.Deaf = false    
                );

                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Un-Deafened"
                };
            } else {
                await user.ModifyAsync(x => 
                    x.Deaf = true
                );

                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Deafened"
                };
            }

            await RespondAsync(
                embed: embed.Build()    
            );
        }

        [RequireUserPermission(ChannelPermission.MoveMembers)]
        [SlashCommand("move", "If the channel is null, disconnect the user; Otherwise, move the user to the channel.")]
        public async Task Move(IGuildUser user=null, IVoiceChannel? channel=null) {
            if (user == null) {
                user = Context.Guild.GetUser(Context.User.Id);
            }
            
            var embed = new EmbedBuilder() {
                Color = Color.Green,
                Title = $"{user.Username}#{user.Discriminator}"
            };
            
            if (channel == null) {
                embed.Description = $"{user.VoiceChannel.Mention}";
                
                await user.ModifyAsync(x =>
                    x.ChannelId = null
                );

                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Disconnected"
                };
            } else {
                embed.Description = $"{user.VoiceChannel.Mention} **{Symbol.RightArrow}** {channel.Mention}";
                
                await user.ModifyAsync(x =>
                    x.ChannelId = channel.Id
                );
                
                embed.Author = new EmbedAuthorBuilder() {
                    Name = "Moved"
                };
            }

            await RespondAsync(
                embed: embed.Build()    
            );
        }
        
        [RequireUserPermission(GuildPermission.KickMembers)]
        [SlashCommand("kick", "Kicks the user.")]
        public async Task Kick(IGuildUser user, string? reason=null) {
            try {
                await user.KickAsync(reason);
                
                var embed = new EmbedBuilder() {
                    Color = Color.Green,
                    Author = new EmbedAuthorBuilder() {
                        Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    Description = $"{user.Mention}\n\n>>> {reason}",
                    Fields = {
                        new EmbedFieldBuilder() {
                            Name = "Kicked"
                        }
                    }
                };

                await RespondAsync(
                    embed: embed.Build()
                );
            } catch {
                var embed = new EmbedBuilder() {
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
        public async Task Ban( IGuildUser user, [MinValue(0), MaxValue(7)] int pruneDays=0, string? reason=null) {
            try {
                await user.BanAsync(pruneDays, reason);
            
                var embed = new EmbedBuilder() {
                    Color = Color.Green,
                    Author = new EmbedAuthorBuilder() {
                        Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                    },
                    Title = $"{user.Username}#{user.Discriminator}",
                    Description = $"{user.Mention}\n\n>>> {reason}",
                    Footer = new EmbedFooterBuilder() {
                        Text = "Banned"
                    }
                };

                await RespondAsync(
                    embed: embed.Build()
                );
            } catch {
                var embed = new EmbedBuilder() {
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