// -----------------------------------------------------------------------
// <copyright file="ThrowRequest.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Enums
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
    }
}
