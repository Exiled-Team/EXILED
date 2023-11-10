// -----------------------------------------------------------------------
// <copyright file="NotificationType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Represents the type of notification.
    /// </summary>
    /// <seealso cref="Notification"/>
    public enum NotificationType
    {
        /// <summary>
        /// Represents <see cref="Features.Broadcast"/>.
        /// </summary>
        Broadcast,

        /// <summary>
        /// Represents <see cref="Features.Hint"/>.
        /// </summary>
        Hint,
    }
}