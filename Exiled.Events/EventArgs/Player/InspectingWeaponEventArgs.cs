// -----------------------------------------------------------------------
// <copyright file="InspectingWeaponEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Exiled.API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Firearms.BasicMessages;

    /// <summary>
    ///     Contains all information before a player inspects a weapon.
    /// </summary>
    public class InspectingWeaponEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InspectingWeaponEventArgs" /> class.
        /// </summary>
        /// <param name="inspector">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="firearm">
        ///     <inheritdoc cref="Firearm" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public InspectingWeaponEventArgs(Player inspector, Firearm firearm, bool isAllowed = true)
        {
            Player = inspector;
            Firearm = firearm;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's inspecting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the target <see cref="API.Features.Items.Firearm" /> to be inspected.
        /// </summary>
        public Firearm Firearm { get; }

        /// <inheritdoc/>
        public Item Item => Firearm;

        /// <summary>
        ///     Gets or sets a value indicating whether or not spectators will see the inspect animation.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
