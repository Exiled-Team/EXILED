// -----------------------------------------------------------------------
// <copyright file="ThrowRequest.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Possible throwable throw types.
    /// </summary>
    public enum ThrowRequest
    {
        /// <summary>
        /// Requesting to begin throwing a throwable item.
        /// </summary>
        BeginThrow,

        /// <summary>
        /// Requesting to confirm a weak throw.
        /// </summary>
        WeakThrow,

        /// <summary>
        /// Requesting to confirm a strong throw.
        /// </summary>
        FullForceThrow,

        /// <summary>
        /// Requesting to cancel a throw.
        /// </summary>
        CancelThrow,
    }
}