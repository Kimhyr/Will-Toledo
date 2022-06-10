using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using PenileNET.Utilities.Constants;

namespace PenileNET.Modules {
    public class RulesModule : InteractionModuleBase<SocketInteractionContext> {
        public InteractionService? Commands { get; set; }

        [SlashCommand("rules", "Creates an embed tailored for rules.")]
        public async Task Embed() {
            await RespondAsync(
                embed: new EmbedBuilder {
                    Color = Colors.Blurple,
                    Description = "*rules as of **Jun 9, 2022***",
                    Fields = {
                        new EmbedFieldBuilder {
                            Name = "Discord ToS",
                            Value = "> Anything against **Discord's ToS** will result in a **ban** and a **report**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Self Promotions",
                            Value =
                                "> Only a **select few** are authorized to **promote anywhere**; If you want to promote anywhere, please promote in the **proper channel**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Inappropriate Content",
                            Value =
                                "> **Any content** that is **sexual** or **gory** will result in a **warning**, **kick**, or **ban**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Harassment",
                            Value =
                                "> **Seriously** harassing someone anywhere — *even in DMs* — will result in a **warning**, **kick**, or **ban**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Hate Speech",
                            Value = "> **Any form** of hate speech will result in a **kick** or **ban**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Controversial Content",
                            Value =
                                "> **Arguing** about politics and/or religious topics will result in a **warning** or **kick**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Behavior",
                            Value =
                                "> **Purposefully** being annoying (*e.g. spamming, being loud, acting dumb, begging*) will result in a **warning** or **kick**."
                        },
                        new EmbedFieldBuilder {
                            Name = "Private Information",
                            Value =
                                "> Exposing someone's private information **un-consensually** will result in a **ban**; If you expose your own or someone's private information **consensually**, it must be nothing severe (*i.e. exact location, card info, social security number*)."
                        },
                        new EmbedFieldBuilder {
                            Name = "Channels",
                            Value =
                                "> Using any channel **improperly** will result in a **warning** or **kick**."
                        }
                    }
                }.Build()
            );
        }
    }
}