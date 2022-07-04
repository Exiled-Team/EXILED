// -----------------------------------------------------------------------
// <copyright file="FriendlyFireStatusCode.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    /// <summary>
    /// Status code for adding FriendlyFire rules.
    /// </summary>
    public enum FriendlyFireStatusCode
    {
        /// <summary>
        /// Adding specific role failed, unable to be added.
        /// </summary>
        UnableToAdd,

        /// <summary>
        /// Partial roles were added, cannot guarantee all.
        /// </summary>
        PartialSuccess,

        /// <summary>
        /// All roles were successfully added
        /// </summary>
        Success,
    }
}
