// -----------------------------------------------------------------------
// <copyright file="TogglingFlashlightEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;
    using InventorySystem.Items.ToggleableLights;

    /// <summary>
    /// Contains all information before a player toggles a flashlight.
    /// </summary>
    public class TogglingFlashlightEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent
    {
        private readonly bool initialState;

        /// <summary>
        /// Initializes a new instance of the <see cref="TogglingFlashlightEventArgs" /> class.
        /// </summary>
        /// <param name="hub">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="flashlight">
        /// <inheritdoc cref="Flashlight" />
        /// </param>
        /// <param name="newState">
        /// <inheritdoc cref="NewState" />
        /// </param>
        public TogglingFlashlightEventArgs(ReferenceHub hub, ToggleableLightItemBase flashlight, bool newState)
        {
            Player = Player.Get(hub);
            Flashlight = (Flashlight)Item.Get(flashlight);
            initialState = newState;
            NewState = newState;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Flashlight" /> being toggled.
        /// </summary>
        public Flashlight Flashlight { get; }

        /// <inheritdoc/>
        public Item Item => Flashlight;

        /// <summary>
        /// Gets or sets a value indicating whether or not the flashlight should be on.
        /// </summary>
        public bool NewState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can toggle the flashlight.
        /// </summary>
        public bool IsAllowed
        {
            get => NewState == initialState;
            set => NewState = value ? initialState : !initialState;
        }

        /// <summary>
        /// Gets the player who's toggling the flashlight.
        /// </summary>
        public Player Player { get; }
    }
}