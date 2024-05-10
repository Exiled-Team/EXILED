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
    using PlayerRoles.PlayableScps.Scp939;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="EnvironmentalHazard"/>.
    /// </summary>
    public class Hazard : GameEntity, IWrapper<EnvironmentalHazard>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="EnvironmentalHazard"/> to it's <see cref="Hazard"/>.
        /// </summary>
        internal static readonly Dictionary<EnvironmentalHazard, Hazard> EnvironmentalHazardToHazard = new(new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Hazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="EnvironmentalHazard"/> instance.</param>
        public Hazard(EnvironmentalHazard hazard)
            : base(hazard.gameObject)
        {
            Base = hazard;

            EnvironmentalHazardToHazard.Add(hazard, this);
        }

        /// <summary>
        /// Gets the list of all hazards.
        /// </summary>
        public static new IReadOnlyCollection<Hazard> List => EnvironmentalHazardToHazard.Values;

        /// <summary>
        /// Gets the <see cref="EnvironmentalHazard"/>.
        /// </summary>
        public EnvironmentalHazard Base { get; }

        /// <summary>
        /// Gets or sets the list with all affected by this hazard players.
        /// </summary>
        public IEnumerable<Player> AffectedPlayers
        {
            get => Base.AffectedPlayers.Select(Player.Get);
            set
            {
                Base.AffectedPlayers.Clear();
                Base.AffectedPlayers.AddRange(value.Select(x => x.ReferenceHub));
            }
        }

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
        public Room Room => Room.FindParentRoom(Base.gameObject);

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override Vector3 Position
        {
            get => Base.SourcePosition;
            set => Base.SourcePosition = value;
        }

        /// <summary>
        /// Gets the <see cref="Hazard"/> by <see cref="EnvironmentalHazard"/>.
        /// </summary>
        /// <param name="environmentalHazard">The <see cref="EnvironmentalHazard"/> instance.</param>
        /// <returns><see cref="Hazard"/> for <see cref="EnvironmentalHazard"/>.</returns>
        public static Hazard Get(EnvironmentalHazard environmentalHazard) =>
            EnvironmentalHazardToHazard.TryGetValue(environmentalHazard, out Hazard hazard) ? hazard
            : environmentalHazard switch
        {
            TantrumEnvironmentalHazard tantrumEnvironmentalHazard => new TantrumHazard(tantrumEnvironmentalHazard),
            Scp939AmnesticCloudInstance scp939AmnesticCloudInstance => new AmnesticCloudHazard(scp939AmnesticCloudInstance),
            SinkholeEnvironmentalHazard sinkholeEnvironmentalHazard => new SinkholeHazard(sinkholeEnvironmentalHazard),
            global::Hazards.TemporaryHazard temporaryHazard => new TemporaryHazard(temporaryHazard),
            _ => new Hazard(environmentalHazard)
        };

        /// <summary>
        /// Gets the hazard by the room where it's located.
        /// </summary>
        /// <param name="room">Room.</param>
        /// <returns><see cref="Hazard"/> in given <see cref="Features.Room"/>.</returns>
        public static Hazard Get(Room room) => Get(x => x.Room == room).FirstOrDefault();

        /// <summary>
        /// Gets the hazard by it's <see cref="GameObject"/>.
        /// </summary>
        /// <param name="obj">Game object.</param>
        /// <returns><see cref="Hazard"/> in given <see cref="Features.Room"/>.</returns>
        public static Hazard Get(GameObject obj) => Get(x => x.Base.gameObject == obj).FirstOrDefault();

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