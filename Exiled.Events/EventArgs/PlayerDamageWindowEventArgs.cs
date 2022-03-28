// -----------------------------------------------------------------------
// <copyright file="PlayerDamageWindowEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;

    /// <summary>
    /// Contains all informations before damage is dealt to a <see cref="BreakableWindow"/>.
    /// </summary>
    public class PlayerDamageWindowEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDamageWindowEventArgs"/> class.
        /// </summary>
        /// <param name="window"><inheritdoc cref="Window"/></param>
        /// <param name="damage"><inheritdoc cref="Damage"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="handlerBase"><inheritdoc cref="HandlerBase"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PlayerDamageWindowEventArgs(BreakableWindow window, Player player, DamageHandlerBase handlerBase, float damage, bool isAllowed = true)
        {
            Window = Window.Get(window);
            Damage = damage;
            Player = player;
            HandlerBase = handlerBase;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Window"/> object that is damaged.
        /// </summary>
        public Window Window { get; }

        /// <summary>
        /// Gets or sets the damage the window will receive.
        /// </summary>
        public float Damage { get; set; }

        /// <summary>
        /// Gets the Player who hit the window.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the DamageHandlerBase the window will receive.
        /// </summary>
        public DamageHandlerBase HandlerBase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the window can be broken.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
