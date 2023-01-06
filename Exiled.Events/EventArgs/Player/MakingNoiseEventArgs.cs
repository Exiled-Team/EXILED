// -----------------------------------------------------------------------
// <copyright file="MakingNoiseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.FirstPersonControl.Thirdperson;

    /// <summary>
    ///     Contains all information before a player makes noise.
    /// </summary>
    public class MakingNoiseEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MakingNoiseEventArgs" /> class.
        /// </summary>
        /// <param name="animatedCharacterModel">
        ///     The <see cref="AnimatedCharacterModel"/> instance.
        /// </param>
        /// <param name="distance">
        ///     <inheritdoc cref="Distance" />
        /// </param>
        public MakingNoiseEventArgs(AnimatedCharacterModel animatedCharacterModel, float distance)
        {
            Player = Player.Get(animatedCharacterModel.OwnerHub);
            Distance = distance;
        }

        /// <summary>
        ///     Gets the player who's making noise.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the footsteps distance.
        /// </summary>
        public float Distance { get; }
    }
}