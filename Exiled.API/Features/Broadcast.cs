// -----------------------------------------------------------------------
// <copyright file="Broadcast.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.ComponentModel;

    using static global::Broadcast;

    /// <summary>
    /// Useful class to save broadcast configs in a cleaner way.
    /// </summary>
    public class Broadcast
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Broadcast"/> class.
        /// </summary>
        public Broadcast()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Broadcast"/> class.
        /// </summary>
        /// <param name="content">The content of the broadcast>.</param>
        /// <param name="duration">The duration of the broadcast, in seconds.</param>
        /// <param name="show">Whether or not the broadcast should be shown.</param>
        /// <param name="type">The type of the broadcast.</param>
        public Broadcast(string content, ushort duration = 10, bool show = true, BroadcastFlags type = BroadcastFlags.Normal)
        {
            Content = content;
            Duration = duration;
            Show = show;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the broadcast content.
        /// </summary>
        [Description("The broadcast content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the broadcast duration.
        /// </summary>
        [Description("The broadcast duration")]
        public ushort Duration { get; set; }

        /// <summary>
        /// Gets or sets the broadcast type.
        /// </summary>
        [Description("The broadcast type")]
        public BroadcastFlags Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the broadcast should be shown or not.
        /// </summary>
        [Description("Indicates whether the broadcast should be shown or not")]
        public bool Show { get; set; }

        /// <summary>
        /// Returns the Broadcast in a human-readable format.
        /// </summary>
        /// <returns>A string containing Broadcast-related data.</returns>
        public override string ToString() => $"({Content}) {Duration} {Type}";
    }
}