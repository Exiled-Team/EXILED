// -----------------------------------------------------------------------
// <copyright file="Locker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Lockers
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using MapGeneration.Distributors;
    using UnityEngine;

    using BaseLocker = MapGeneration.Distributors.Locker;

    /// <summary>
    /// Represents a basic locker.
    /// </summary>
    public class Locker : TypeCastObject<Locker>, IWrapper<BaseLocker>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="BaseLocker"/> and <see cref="Locker"/>.
        /// </summary>
        internal static readonly Dictionary<BaseLocker, Locker> BaseToExiledLockers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Locker"/> class.
        /// </summary>
        /// <param name="locker">The <see cref="BaseLocker"/> instance.</param>
        public Locker(BaseLocker locker)
        {
            Base = locker;
            Chambers = locker.Chambers.Select(x => new Chamber(x)).ToList();

            BaseToExiledLockers.Add(locker, this);
        }

        /// <summary>
        /// Gets the list with all <see cref="Locker"/>.
        /// </summary>
        public static IReadOnlyCollection<Locker> List => BaseToExiledLockers.Values;

        /// <inheritdoc/>
        public BaseLocker Base { get; }

        /// <summary>
        /// Gets or sets list of all <see cref="LockerLoot"/> in this locker.
        /// </summary>
        public IEnumerable<LockerLoot> Loot
        {
            get => Base.Loot;
            set => Base.Loot = value.ToArray();
        }

        /// <summary>
        /// Gets the list with all <see cref="Chambers"/> in this locker.
        /// </summary>
        public IReadOnlyCollection<Chamber> Chambers { get; }

        /// <summary>
        /// Gets the <see cref="Locker"/> by it's basegame analog.
        /// </summary>
        /// <param name="locker"><see cref="BaseLocker"/> instance.</param>
        /// <returns><see cref="Locker"/> instance.</returns>
        public static Locker Get(BaseLocker locker) => BaseToExiledLockers.TryGetValue(locker, out var lk)
            ? lk
            : locker switch
            {
                PedestalScpLocker psl => new PedestalLocker(psl),
                _ => new Locker(locker)
            };

        /// <summary>
        /// Gets the list of <see cref="Locker"/> which matches predicate.
        /// </summary>
        /// <param name="predicate">Predicate to match.</param>
        /// <returns>List of <see cref="Locker"/> which matches predicate.</returns>
        public static IEnumerable<Locker> Get(System.Func<Locker, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Interacts with a specific chamber.
        /// </summary>
        /// <param name="chamber">Chamber. If <see langword="null"/>, will interact with random</param>
        /// <param name="player">Player who interacts.</param>
        public void Interact(Chamber chamber = null, Player player = null) => Base.ServerInteract(player?.ReferenceHub, (byte)(chamber == null ? Random.Range(0, Chambers.Count + 1) : Chambers.ToList().IndexOf(chamber)));

        /// <summary>
        /// Fills chamber.
        /// </summary>
        /// <param name="chamber">Chamber to fill.</param>
        public void FillChamber(Chamber chamber) => Base.FillChamber(chamber.Base);
    }
}