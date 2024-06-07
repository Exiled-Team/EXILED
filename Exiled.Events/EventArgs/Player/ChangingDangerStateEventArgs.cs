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
        /// <param name="activating">Whether or not the danger is being activated. If false it is ending.</param>
        /// <param name="isAllowed">Whether or not it is allowed to activate/deactivate.</param>
        public ChangingDangerStateEventArgs(Player player, DangerStackBase danger, DangerType type, bool activating, bool isAllowed = true)
        {
            Player = player;
            Danger = danger;
            Type = type;
            IsActivating = activating;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the <see cref="DangerStackBase"/>.
        /// </summary>
        public DangerStackBase Danger { get; }

        /// <summary>
        /// Gets the <see cref="DangerType"/> of Danger changing.
        /// </summary>
        public DangerType Type { get; }

        /// <summary>
        /// Gets a value indicating whether or not the danger is being activated. If false it is ending.
        /// </summary>
        public bool IsActivating { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}