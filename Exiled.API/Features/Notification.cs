// -----------------------------------------------------------------------
// <copyright file="Notification.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using Exiled.API.Enums;

    /// <summary>
    /// Used to configure <see cref="Broadcast"/> or <see cref="Hint"/> in a cleaner way.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        public Notification()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="content">The content of the notification>.</param>
        /// <param name="duration">The duration of the notification, in seconds.</param>
        /// <param name="show">Whether or not the notification should be shown.</param>
        /// <param name="clear">Whether or not the active notifications should be cleared.</param>
        /// <param name="type">The type of the notification.</param>
        public Notification(string content, ushort duration = 5, bool show = true, bool clear = false, NotificationType type = NotificationType.Broadcast)
        {
            Content = content;
            Duration = duration;
            Show = show;
            Clear = clear;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the notification content.
        /// </summary>
        [Description("The notification content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the notification duration.
        /// </summary>
        [Description("The notification duration")]
        public ushort Duration { get; set; }

        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        [Description("The notification type")]
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notification should be shown or not.
        /// </summary>
        [Description("Indicates whether the notification should be shown or not")]
        public bool Show { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the active notifications should be cleared or not.
        /// </summary>
        [Description("Indicates whether the active notifications should be cleared or not")]
        public bool Clear { get; set; }

        /// <summary>
        /// Returns notification with formatted content.
        /// </summary>
        /// <param name="args">The list of formation arguments.</param>
        /// <returns>Notification with formatted content.</returns>
        public Notification Format(params object[] args) => new(string.Format(Content, args), Duration, Show, Clear, Type);

        /// <summary>
        /// Returns the notification in a human-readable format.
        /// </summary>
        /// <returns>A string containing notification-related data.</returns>
        public override string ToString() => $"({Content}) {Duration} {Type} {Clear}";
    }
}
