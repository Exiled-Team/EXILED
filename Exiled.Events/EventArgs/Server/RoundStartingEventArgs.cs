// -----------------------------------------------------------------------
// <copyright file="RoundStartingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    /// <summary>
    /// Contains all information before the start of the round.
    /// </summary>
    public class RoundStartingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundStartingEventArgs" /> class.
        /// </summary>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public RoundStartingEventArgs(bool isAllowed = true)
        {
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the round can start or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
