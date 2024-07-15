// -----------------------------------------------------------------------
// <copyright file="ChangingDangerStateEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using CustomPlayerEffects.Danger;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player changes danger.
    /// </summary>
    public class ChangingDangerStateEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingDangerStateEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player changing danger.</param>
        /// <param name="danger">The <see cref="DangerStackBase"/>.</param>
        /// <param name="type">The <see cref="DangerType"/>.</param>
        /// <param name="activating">Whether the danger is being activated. If false it is ending.</param>
        /// <param name="encounteredPlayer">The player that has been encountered if the danger is an encounter or null if it isn't.</param>
        /// <param name="isAllowed">Whether it is allowed to activate/deactivate.</param>
        public ChangingDangerStateEventArgs(Player player, DangerStackBase danger, DangerType type, bool activating, Player encounteredPlayer = null, bool isAllowed = true)
        {
            Player = player;
            Danger = danger;
            Type = type;
            IsActivating = activating;
            EncounteredPlayer = encounteredPlayer;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="DangerStackBase"/>.
        /// </summary>
        public DangerStackBase Danger { get; }

        /// <summary>
        /// Gets the <see cref="DangerType"/> of Danger changing.
        /// </summary>
        public DangerType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the danger is being activated.
        /// </summary>
        public bool IsActivating { get; }

        /// <summary>
        /// Gets a value indicating whether the danger is ending.
        /// </summary>
        public bool IsEnding => !IsActivating;

        /// <summary>
        /// Gets the encountered player if the danger is an encounter or null if it isn't.
        /// </summary>
        public Player EncounteredPlayer { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}