// -----------------------------------------------------------------------
// <copyright file="Snowpile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces;
    using UnityEngine;

    using Basepile = global::Snowpile;

    /// <summary>
    /// A wrapper for <see cref="Basepile"/>.
    /// </summary>
    public class Snowpile : IWorldSpace, IWrapper<Basepile>
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> with <see cref="Basepile"/> to <see cref="Snowpile"/>.
        /// </summary>
        internal static readonly Dictionary<Basepile, Snowpile> BaseToWrapper = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowpile"/> class.
        /// </summary>
        /// <param name="snowpile">The <see cref="Basepile"/> instance.</param>
        public Snowpile(Basepile snowpile)
        {
            Base = snowpile;

            BaseToWrapper.Add(snowpile, this);
        }

        /// <summary>
        /// Gets the list of all snowpiles.
        /// </summary>
        public static IReadOnlyCollection<Snowpile> List => BaseToWrapper.Values;

        /// <inheritdoc/>
        public Basepile Base { get; }

        /// <inheritdoc/>
        public Vector3 Position
        {
            get => Base._position;
            set => Base.Network_position = value;
        }

        /// <inheritdoc/>
        public Quaternion Rotation
        {
            get => Base._rotation;
            set => Base.Network_rotation = value;
        }

        /// <summary>
        /// Gets or sets amount of uses that this snowpile can handle.
        /// </summary>
        public int RemainingUses
        {
            get => Base._remainingUses;
            set => Base.Network_remainingUses = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this snowpile should regenerate amount of usages.
        /// </summary>
        public bool Regenerate
        {
            get => Base._regenerate;
            set => Base._regenerate = value;
        }

        /// <summary>
        /// Gets or sets a current timer of regeneration.
        /// </summary>
        public float RegenerationTimer
        {
            get => Base._regenerationTimer;
            set => Base._regenerationTimer = value;
        }

        /// <summary>
        /// Gets or sets time how much snowpile should regenerate 1 snow ball.
        /// </summary>
        public float RegenerationDuration
        {
            get => Base._regenerationDuration;
            set => Base._regenerationDuration = value;
        }

        /// <summary>
        /// Gets or sets maximal amount of uses that this snowpile can has..
        /// </summary>
        public int InitUses
        {
            get => Base._initialUses;
            set => Base._initialUses = value;
        }

        /// <summary>
        /// Gets a <see cref="Snowpile"/> which is connected to it's base game analog.
        /// </summary>
        /// <param name="snowpile">Base game analog.</param>
        /// <returns>A <see cref="Snowpile"/> instance or <see langword="null"/>.</returns>
        public static Snowpile Get(Basepile snowpile) => BaseToWrapper.TryGetValue(snowpile, out Snowpile pile) ? pile : new Snowpile(snowpile);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Snowpile"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Snowpile> Get(Func<Snowpile, bool> predicate) => List.Where(predicate);
    }
}