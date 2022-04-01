// -----------------------------------------------------------------------
// <copyright file="Broadcast.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System.ComponentModel;

    /// <summary>
    /// Useful class to save broadcast configs in a cleaner way.
    /// </summary>
    public class Broadcast {
        /// <summary>
        /// Initializes a new instance of the <see cref="Broadcast"/> class.
        /// </summary>
        public Broadcast()
            : this(string.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Broadcast"/> class.
        /// </summary>
        /// <param name="content"><inheritdoc cref="Content"/></param>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="show"><inheritdoc cref="Show"/></param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        public Broadcast(string content, ushort duration = 10, bool show = true, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal) {
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
        public global::Broadcast.BroadcastFlags Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the broadcast should be shown or not.
        /// </summary>
        [Description("Indicates whether the broadcast should be shown or not")]
        public bool Show { get; set; }
    }
}
