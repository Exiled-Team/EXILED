// -----------------------------------------------------------------------
// <copyright file="ContainingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before containing SCP-106.
    /// </summary>
    public class ContainingEventArgs : IPlayerEvent, IDeniableEvent
    {
#pragma warning disable SA1625 // Element documentation should not be copied and pasted
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContainingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Scp106" />
        /// </param>
        /// <param name="buttonPresser">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
#pragma warning restore SA1625 // Element documentation should not be copied and pasted
        public ContainingEventArgs(Player player, Player buttonPresser, bool isAllowed = true)
        {
            Scp106 = player;
            Player = buttonPresser;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Scp106 { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-106 can be recontained.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who pressed the button.
        /// </summary>
        public Player Player { get; }
    }
}