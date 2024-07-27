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

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using MapGeneration.Distributors;

    using BaseLocker = MapGeneration.Distributors.Locker;

    /// <summary>
    /// Represents a basic locker.
    /// </summary>
    public class Locker : GameEntity, IWrapper<BaseLocker>
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
            : base(locker.gameObject)
        {
            Base = locker;
            Chambers = locker.Chambers.Select(x => new Chamber(x, this)).ToList();

            if (!BaseToExiledLockers.ContainsKey(locker))
                BaseToExiledLockers.Add(locker, this);
        }

        /// <summary>
        /// Gets the all <see cref="Locker"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Locker> List => BaseToExiledLockers.Values;

        /// <inheritdoc/>
        public BaseLocker Base { get; }

        /// <summary>
        /// Gets or sets all <see cref="LockerLoot"/> instances in this locker.
        /// </summary>
        public IEnumerable<LockerLoot> Loot
        {
            get => Base.Loot;
            set => Base.Loot = value.ToArray();
        }

        /// <summary>
        /// Gets the all <see cref="Chambers"/> in this locker.
        /// </summary>
        public IReadOnlyCollection<Chamber> Chambers { get; }

        /// <summary>
        /// Gets or sets an id for manipulating opened chambers.
        /// </summary>
        public ushort OpenedChambers
        {
            get => Base.OpenedChambers;
            set => Base.NetworkOpenedChambers = value;
        }

        /// <summary>
        /// Gets the <see cref="Locker"/> given the <see cref="BaseLocker"/> instance.
        /// </summary>
        /// <param name="locker"><see cref="BaseLocker"/> instance.</param>
        /// <returns><see cref="Locker"/> instance.</returns>
        public static Locker Get(BaseLocker locker) => BaseToExiledLockers.TryGetValue(locker, out Locker lk)
            ? lk
            : locker switch
            {
                PedestalScpLocker psl => new PedestalLocker(psl),
                _ => new Locker(locker)
            };

        /// <summary>
        /// Gets the all <see cref="Locker"/> instances matching the predicate.
        /// </summary>
        /// <param name="predicate">Predicate to match.</param>
        /// <returns>All <see cref="Locker"/> instances matching the predicate.</returns>
        public static IEnumerable<Locker> Get(System.Func<Locker, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Interacts with a specific chamber.
        /// </summary>
        /// <param name="chamber">If <see langword="null"/>, the interaction will be randomized.</param>
        /// <param name="player">The player who interacts.</param>
        public void Interact(Chamber chamber = null, Player player = null)
        {
            chamber ??= Chambers.Random();

            Base.ServerInteract(player?.ReferenceHub, (byte)Chambers.ToList().IndexOf(chamber));
        }

        /// <summary>
        /// Fills the chamber.
        /// </summary>
        /// <param name="chamber">Chamber to fill.</param>
        public void FillChamber(Chamber chamber) => Base.FillChamber(chamber.Base);
    }
}
