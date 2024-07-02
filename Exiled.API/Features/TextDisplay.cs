// -----------------------------------------------------------------------
// <copyright file="TextDisplay.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Core.Interfaces;
    using Hints;

    using static global::Broadcast;

    /// <summary>
    /// Useful class to save text display configs in a cleaner way.
    /// </summary>
    [EClass(category: nameof(TextDisplay))]
    public class TextDisplay : TypeCastObject<TextDisplay>, IAssetFragment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextDisplay"/> class.
        /// </summary>
        public TextDisplay()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDisplay"/> class.
        /// </summary>
        /// <param name="content">The content of the text display.</param>
        /// <param name="duration">The duration of the text display, in seconds.</param>
        /// <param name="canBeDisplayed">Whether or not the text display should be displayed.</param>
        /// <param name="channel">The text channel to be used to display the content.</param>
        public TextDisplay(string content, ushort duration = 10, bool canBeDisplayed = true, TextChannelType channel = TextChannelType.None)
        {
            Content = content;
            Duration = duration;
            CanBeDisplayed = canBeDisplayed;
            Channel = channel;
        }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> content.
        /// </summary>
        [Description("The text display content")]
        [EProperty(category: nameof(TextDisplay))]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> duration.
        /// </summary>
        [Description("The text display duration")]
        [EProperty(category: nameof(TextDisplay))]
        public ushort Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TextDisplay"/> should be shown or not.
        /// </summary>
        [Description("Indicates whether the text display should be shown or not")]
        [EProperty(category: nameof(TextDisplay))]
        public bool CanBeDisplayed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextChannelType"/>.
        /// </summary>
        [Description("The text display channel to be used to display the content")]
        [EProperty(category: nameof(TextDisplay))]
        public TextChannelType Channel { get; set; }

        /// <summary>
        /// Shows the content to a player.
        /// </summary>
        /// <param name="player">The player to display the content to.</param>
        /// <param name="content">The content to display.</param>
        /// <param name="duration">The duration of the text display.</param>
        /// <param name="textChannel">The <see cref="TextChannelType"/> to rely on.</param>
        /// <param name="args">Additional arguments.</param>
        public static void Show(Player player, string content, ushort duration, TextChannelType textChannel, params object[] args)
        {
            if (!player || textChannel is TextChannelType.None)
                return;

            new TextDisplay(content, duration, true, textChannel).Show(player, args);
        }

        /// <summary>
        /// Shows the content to a player.
        /// </summary>
        /// <param name="player">The player to display the content to.</param>
        /// <param name="args">Additional arguments.</param>
        public void Show(Player player, params object[] args)
        {
            if (!player || Channel is TextChannelType.None || !CanBeDisplayed)
                return;

            if (Channel is TextChannelType.Broadcast)
            {
                player.Broadcast(
                    Duration,
                    Content,
                    args.Length > 0 && args[0] is BroadcastFlags flags ? flags : BroadcastFlags.Normal,
                    args.Length > 1 && args[1] is bool clearPreviousBroadcasts && clearPreviousBroadcasts);

                return;
            }

            if (args.Length > 0)
            {
                player.HintDisplay.Show(new TextHint(
                    Content,
                    args[0] as HintParameter[] ?? new HintParameter[] { new StringHintParameter(Content) },
                    args.Length > 1 && args[1] is HintEffect[] hintEffects ? hintEffects : null,
                    Duration));

                return;
            }

            player.ShowHint(Content, Duration);
        }

        /// <summary>
        /// Shows the content to a player.
        /// </summary>
        /// <param name="player">The player to display the content to.</param>
        /// <param name="channel">The text channel to be used to display the content.</param>
        /// <param name="args">Additional arguments.</param>
        public void Show(Player player, TextChannelType channel, params object[] args)
        {
            if (!player || channel is TextChannelType.None)
                return;

            if (channel is TextChannelType.Broadcast)
            {
                player.Broadcast(
                    Duration,
                    Content,
                    args.Length > 0 && args[0] is BroadcastFlags flags ? flags : BroadcastFlags.Normal,
                    args.Length > 1 && args[1] is bool clearPreviousBroadcasts && clearPreviousBroadcasts);

                return;
            }

            if (args.Length > 0)
            {
                player.HintDisplay.Show(new TextHint(
                    Content,
                    args[0] as HintParameter[] ?? new HintParameter[] { new StringHintParameter(Content) },
                    args.Length > 1 && args[1] is HintEffect[] hintEffects ? hintEffects : null,
                    Duration));

                return;
            }

            player.ShowHint(Content, Duration);
        }

        /// <summary>
        /// Returns the <see cref="TextDisplay"/> in a human-readable format.
        /// </summary>
        /// <returns>A string containing <see cref="TextDisplay"/>-related data.</returns>
        public override string ToString() => $"({Content}) {Duration}";
    }
}