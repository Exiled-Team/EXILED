// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before freeing a handcuffed player.
    /// </summary>
    public class RemovingHandcuffsEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemovingHandcuffsEventArgs" /> class.
        /// </summary>
        /// <param name="cuffer">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public RemovingHandcuffsEventArgs(Player cuffer, Player target, bool isAllowed = true)
        {
            Player = cuffer;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}