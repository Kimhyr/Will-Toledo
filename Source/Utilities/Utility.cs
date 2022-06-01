using System;
using Discord;
using Discord.Interactions;

namespace PenileNET.Utilities {
    public class Utility {
        public static string ConvertDateTimeOffset(DateTimeOffset date) {
            return $"{date.Month}/{date.Day}/{date.Year}";
        }

        public static IGuildUser CheckUser(IGuildUser user, SocketInteractionContext context) {
            if (user == null) {
                return context.Guild.GetUser(context.User.Id);
            }

            return user;
        }
    }
}