// -----------------------------------------------------------------------
// <copyright file="AddingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;

    using Interfaces;

    /// <summary>
    ///     Contains all information before adding a target to SCP-096.
    /// </summary>
    public class AddingTargetEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AddingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isForLook">
        ///     <inheritdoc cref="IsLooking" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public AddingTargetEventArgs(Player scp096, Player target, bool isForLook, bool isAllowed = true)
        {
            Player = scp096;
            Target = target;
            IsLooking = isForLook;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Player Target { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the target was being target cause of looking it's face.
        /// </summary>
        public bool IsLooking { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}