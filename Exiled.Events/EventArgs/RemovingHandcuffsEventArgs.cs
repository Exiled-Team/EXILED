// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before freeing a handcuffed player.
    /// </summary>
    public class RemovingHandcuffsEventArgs : HandcuffingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingHandcuffsEventArgs"/> class.
        /// </summary>
        /// <param name="cuffer">The cuffer.</param>
        /// <param name="target">The target that will be uncuffed.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public RemovingHandcuffsEventArgs(Player cuffer, Player target, bool isAllowed = true)
            : base(cuffer, target, isAllowed)
        {
        }
    }
}
