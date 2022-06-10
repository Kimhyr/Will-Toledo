using Discord;
using Discord.Interactions;

namespace PenileNET.Modals {
    public class EmbedModal : IModal {
        [RequiredInput(false)]
        [InputLabel("Color")]
        [ModalTextInput(
            "embedColor_textInput",
            TextInputStyle.Short,
            "In the format 'R G B'"
        )]
        public string? Color { get; set; }

        [RequiredInput(false)]
        [InputLabel("Author")]
        [ModalTextInput(
            "embedAuthor_textInput",
            TextInputStyle.Short,
            maxLength: 256
        )]
        public string? Author { get; set; }

        [InputLabel("Title")]
        [ModalTextInput(
            "embedTitle_textInput",
            TextInputStyle.Short,
            maxLength: 256
        )]
        public string? Name { get; set; }

        [RequiredInput(false)]
        [InputLabel("Thumbnail")]
        [ModalTextInput(
            "embedThumbnail_textInput",
            TextInputStyle.Short,
            "URL to an image."
        )]
        public string? Thumbnail { get; set; }

        [RequiredInput(false)]
        [InputLabel("Description")]
        [ModalTextInput(
            "embedDescription_textInput",
            TextInputStyle.Paragraph,
            maxLength: 4000
        )]
        public string? Description { get; set; }

        public string Title => "Embed Modal";
    }
}