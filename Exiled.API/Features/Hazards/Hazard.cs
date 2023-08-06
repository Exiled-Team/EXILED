// -----------------------------------------------------------------------
// <copyright file="Hazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using global::Hazards;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="EnvironmentalHazard"/>.
    /// </summary>
    public class Hazard : TypeCastObject<Hazard>, IWrapper<EnvironmentalHazard>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="EnvironmentalHazard"/> to it's <see cref="Hazard"/>.
        /// </summary>
        internal static readonly Dictionary<EnvironmentalHazard, Hazard> EnvironmentalHazardToHazard = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Hazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="EnvironmentalHazard"/> instance.</param>
        public Hazard(EnvironmentalHazard hazard)
        {
            Base = hazard;

            EnvironmentalHazardToHazard.Add(hazard, this);
        }

        /// <summary>
        /// Gets the list of all hazards.
        /// </summary>
        public static IEnumerable<Hazard> List => EnvironmentalHazardToHazard.Values;

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
        /// Gets the room where this hazard is located.
        /// </summary>
        public Room Room => Base.GetComponentInParent<Room>();

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.SourcePosition;
            set => Base.SourcePosition = value;
        }

        /// <summary>
        /// Gets the <see cref="Hazard"/> by <see cref="EnvironmentalHazard"/>.
        /// </summary>
        /// <param name="hazard">The <see cref="EnvironmentalHazard"/> instance.</param>
        /// <returns><see cref="Hazard"/> for <see cref="EnvironmentalHazard"/>.</returns>
        public static Hazard Get(EnvironmentalHazard hazard)
        {
            if (EnvironmentalHazardToHazard.TryGetValue(hazard, out Hazard result))
                return result;

            if (hazard is global::Hazards.TemporaryHazard thazard)
            {
                if (thazard.IsActive)
                {
                    return thazard switch
                    {
                        TantrumEnvironmentalHazard tantrumEnvironmentalHazard => new TantrumHazard(tantrumEnvironmentalHazard),
                        _ => new TemporaryHazard(thazard)
                    };
                }

                return new Hazard(thazard);
            }

            return hazard switch
            {
                SinkholeEnvironmentalHazard sinkholeEnvironmentalHazard => new SinkholeHazard(sinkholeEnvironmentalHazard),
                _ => new Hazard(hazard)
            };
        }

        /// <summary>
        /// Gets the hazard by the room where it's located.
        /// </summary>
        /// <param name="room">Room.</param>
        /// <returns><see cref="Hazard"/> in given <see cref="Exiled.API.Features.Room"/>.</returns>
        public static Hazard Get(Room room) => Get(room.GetComponentInChildren<EnvironmentalHazard>());

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of <see cref="Hazard"/> based on predicate.
        /// </summary>
        /// <param name="predicate">Condition to satisfy.</param>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="Hazard"/> based on predicate.</returns>
        public static IEnumerable<Hazard> Get(Func<Hazard, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Checks if player is in hazard zone.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns><see langword="true"/> if player is in hazard zone. Otherwise, false.</returns>
        public bool IsInArea(Player player) => Base.IsInArea(Position, player.Position);
    }
}