// -----------------------------------------------------------------------
// <copyright file="DamagingWindowEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using API.Features.DamageHandlers;

    using Interfaces;

    using AttackerDamageHandler = PlayerStatsSystem.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information before damage is dealt to a <see cref="BreakableWindow" />.
    /// </summary>
    public class DamagingWindowEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DamagingWindowEventArgs" /> class.
        /// </summary>
        /// <param name="window">
        ///     <inheritdoc cref="Window" />
        /// </param>
        /// <param name="damage">The damage being dealt.</param>
        /// <param name="handler">
        ///     <inheritdoc cref="Handler" />
        /// </param>
        public DamagingWindowEventArgs(BreakableWindow window, float damage, DamageHandlerBase handler)
        {
            Window = Window.Get(window);
            Handler = new DamageHandler(handler is AttackerDamageHandler attackerDamageHandler ? Player.Get(attackerDamageHandler.Attacker.Hub) : null, handler);
            Handler.Damage = damage;
            Player = Handler.Attacker;
        }

        /// <summary>
        ///     Gets the <see cref="Window" /> object that is damaged.
        /// </summary>
        public Window Window { get; }

        /// <summary>
        ///     Gets or sets the damage handler for this event.
        /// </summary>
        public DamageHandler Handler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the window can be broken.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> causing the damage.
        /// </summary>
        public Player Player { get; }
    }
}