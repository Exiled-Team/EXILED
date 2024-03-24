// -----------------------------------------------------------------------
// <copyright file="TextChannelType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All available text channels.
    /// </summary>
    public enum TextChannelType
    {
        /// <summary>
        /// Means text won't be displayed.
        /// </summary>
        None,

        /// <summary>
        /// Means the text will be displayed through the broadcast system.
        /// </summary>
        Broadcast,

        /// <summary>
        /// Means the text will be displayed through hint display system.
        /// </summary>
        Hint,
    }
}
