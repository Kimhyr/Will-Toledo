using System.Text.Encodings.Web;
using Discord;
using Discord.Interactions;

namespace PenileNET.Modals {
    public class ChannelModal : IModal {
        [InputLabel("Type")]
        [ModalTextInput(
            "channelType_textInput",
            TextInputStyle.Short,
            "text/voice"
        )]
        public string? Type { get; set; }

        [InputLabel("Name")]
        [ModalTextInput(
            "channelName_textInput",
            TextInputStyle.Short,
            maxLength: 100
        )]
        public string? Name { get; set; }

        [InputLabel("Topic")]
        [ModalTextInput(
            "channelTopic_textInput",
            TextInputStyle.Paragraph,
            maxLength: 1024
        )]
        public string? Topic { get; set; }

        [InputLabel("Category")]
        [ModalTextInput(
            "channelPrivate_textInput",
            TextInputStyle.Short,
            "Category ID"
        )]
        public string? Category { get; set; }

        public string Title => "Embed Modal";
    }
}