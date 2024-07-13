// -----------------------------------------------------------------------
// <copyright file="Chamber.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Lockers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using MapGeneration.Distributors;
    using UnityEngine;

    /// <summary>
    /// A wrapper for <see cref="LockerChamber"/>.
    /// </summary>
    public class Chamber : GameEntity, IWrapper<LockerChamber>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> with <see cref="LockerChamber"/> and <see cref="Chamber"/>.
        /// </summary>
        internal static readonly Dictionary<LockerChamber, Chamber> Chambers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Chamber"/> class.
        /// </summary>
        /// <param name="chamber"><see cref="LockerChamber"/> instance.</param>
        /// <param name="locker"><see cref="Lockers.Locker"/> where this chamber is located.</param>
        public Chamber(LockerChamber chamber, Locker locker)
            : base(chamber.gameObject)
        {
            Base = chamber;
            Locker = locker;

            Chambers.Add(chamber, this);
        }

        /// <summary>
        /// Gets all chambers.
        /// </summary>
        public static new IReadOnlyCollection<Chamber> List => Chambers.Values;

        /// <inheritdoc/>
        public LockerChamber Base { get; }

        /// <summary>
        /// Gets or sets all pickups that should be spawned when the door is initially opened.
        /// </summary>
        public IEnumerable<Pickup> ToBeSpawned
        {
            get => Base._toBeSpawned.Select(Pickup.Get);
            set
            {
                Base._toBeSpawned.Clear();

                foreach (Pickup pickup in value)
                    Base._toBeSpawned.Add(pickup.Base);
            }
        }

        /// <summary>
        /// Gets or sets all pickups in the chamber.
        /// </summary>
        public IEnumerable<Pickup> AllPickups
        {
            get => Base._content.Select(Pickup.Get);
            set
            {
                Base._content.Clear();

                foreach (Pickup pickup in value)
                    Base._content.Add(pickup.Base);
            }
        }

        /// <summary>
        /// Gets or sets all spawn points.
        /// </summary>
        /// <remarks>
        /// Used if <see cref="UseMultipleSpawnpoints"/> is set to <see langword="true"/>.
        /// </remarks>
        public IEnumerable<Transform> Spawnpoints
        {
            get => Base._spawnpoints;
            set => Base._spawnpoints = value.ToArray();
        }

        /// <summary>
        /// Gets or sets all the acceptable items which can be spawned in this chamber.
        /// </summary>
        public IEnumerable<ItemType> AcceptableTypes
        {
            get => Base.AcceptableItems;
            set => Base.AcceptableItems = value.ToArray();
        }

        /// <summary>
        /// Gets or sets required permissions to open this chamber.
        /// </summary>
        public KeycardPermissions RequiredPermissions
        {
            get => (KeycardPermissions)Base.RequiredPermissions;
            set => Base.RequiredPermissions = (Interactables.Interobjects.DoorUtils.KeycardPermissions)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple spawn points  should be used.
        /// </summary>
        /// <remarks>
        /// If <see langword="true"/>, <see cref="Spawnpoints"/> will be used over <see cref="Spawnpoint"/>.
        /// </remarks>
        public bool UseMultipleSpawnpoints
        {
            get => Base._useMultipleSpawnpoints;
            set => Base._useMultipleSpawnpoints = value;
        }

        /// <summary>
        /// Gets or sets a spawn point for the items in the chamber.
        /// </summary>
        /// <remarks>
        /// Used if <see cref="UseMultipleSpawnpoints"/> is set to <see langword="false"/>.
        /// </remarks>
        public Transform Spawnpoint
        {
            get => Base._spawnpoint;
            set => Base._spawnpoint = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not items should be spawned as soon as they are initialized.
        /// </summary>
        public bool InitiallySpawn
        {
            get => Base._spawnOnFirstChamberOpening;
            set => Base._spawnOnFirstChamberOpening = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before a player can interact with the chamber again.
        /// </summary>
        public float Cooldown
        {
            get => Base._targetCooldown;
            set => Base._targetCooldown = value;
        }

        /// <summary>
        /// Gets the <see cref="Stopwatch"/> of current cooldown.
        /// </summary>
        /// <remarks>Used in <see cref="CanInteract"/> check.</remarks>
        public Stopwatch CurrentCooldown => Base._stopwatch;

        /// <summary>
        /// Gets a value indicating whether the chamber is interactable.
        /// </summary>
        public bool CanInteract => Base.CanInteract;

        /// <summary>
        /// Gets the locker where this chamber is located at.
        /// </summary>
        public Locker Locker { get; }

        /// <summary>
        /// Spawns a specified item from <see cref="AcceptableTypes"/>.
        /// </summary>
        /// <param name="type"><see cref="ItemType"/> from <see cref="AcceptableTypes"/>.</param>
        /// <param name="amount">Amount of items that should be spawned.</param>
        public void SpawnItem(ItemType type, int amount) => Base.SpawnItem(type, amount);

        /// <summary>
        /// Gets the chamber by its <see cref="LockerChamber"/>.
        /// </summary>
        /// <param name="chamber"><see cref="LockerChamber"/>.</param>
        /// <returns><see cref="Chamber"/>.</returns>
        internal static Chamber Get(LockerChamber chamber) => Chambers.TryGetValue(chamber, out Chamber chmb) ? chmb : new(chamber, Locker.Get(l => l.Chambers.Any(c => c.Base == chamber)).FirstOrDefault());
    }
}