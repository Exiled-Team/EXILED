// -----------------------------------------------------------------------
// <copyright file="Hazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Hazards;
    using UnityEngine;

    public class Hazard
    {
        internal static Dictionary<EnvironmentalHazard, Hazard> EnviromentalHazardToHazard { get; } = new();

        public static IEnumerable<Hazard> List => EnviromentalHazardToHazard.Values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hazard"/> class.
        /// </summary>
        /// <param name="hazard"></param>
        protected Hazard(EnvironmentalHazard hazard)
        {
            Base = hazard;

            EnviromentalHazardToHazard.Add(hazard, this);
        }

        /// <summary>
        /// Gets the <see cref="EnvironmentalHazard"/>.
        /// </summary>
        public EnvironmentalHazard Base { get; }

        /// <summary>
        /// Gets the list with all affected by this hazard players.
        /// </summary>
        public IEnumerable<Player> AffectedPlayers => Base.AffectedPlayers.Select(Player.Get);

        /// <summary>
        /// Gets or sets max hazard distance.
        /// </summary>
        public float MaxDistance
        {
            get => Base.MaxDistance;
            set => Base.MaxDistance = value;
        }

        /// <summary>
        /// Gets or sets max hazard height.
        /// </summary>
        public float MaxHeightDistance
        {
            get => Base.MaxHeightDistance;
            set => Base.MaxHeightDistance = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not hazard is active.
        /// </summary>
        public bool IsActive => Base.IsActive;

        /// <summary>
        /// Gets or sets offset for position.
        /// </summary>
        public Vector3 PositionOffset
        {
            get => Base.SourceOffset;
            set => Base.SourceOffset = value;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.SourcePosition;
            set => Base.SourcePosition = value;
        }

        public static Hazard Get(EnvironmentalHazard hazard)
        {
            if (EnviromentalHazardToHazard.TryGetValue(hazard, out Hazard result))
                return result;

            return new(hazard);
        }

        /// <summary>
        /// Checks if player is in hazard zone.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns><see langword="true"/> if player is in hazard zone. Otherwise, false.</returns>
        public bool IsInArea(Player player) => Base.IsInArea(Position, player.Position);
    }
}