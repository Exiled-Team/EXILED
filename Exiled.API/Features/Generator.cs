// -----------------------------------------------------------------------
// <copyright file="Generator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Interfaces;
    using MapGeneration.Distributors;
    using UnityEngine;

    /// <summary>
    /// Wrapper class for <see cref="Scp079Generator"/>.
    /// </summary>
    [EClass(category: nameof(Generator))]
    public class Generator : GameEntity, IWrapper<Scp079Generator>
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Generator"/> on the map.
        /// </summary>
        internal static readonly Dictionary<Scp079Generator, Generator> Scp079GeneratorToGenerator = new(new ComponentsEqualityComparer());
        private Room room;

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="scp079Generator">The <see cref="Scp079Generator"/>.</param>
        internal Generator(Scp079Generator scp079Generator)
            : base(scp079Generator.gameObject)
        {
            Base = scp079Generator;
            Scp079GeneratorToGenerator.Add(scp079Generator, this);
        }

        /// <summary>
        /// Gets the prefab's type.
        /// </summary>
        public static PrefabType PrefabType => PrefabType.GeneratorStructure;

        /// <summary>
        /// Gets the prefab's object.
        /// </summary>
        public static GameObject PrefabObject => PrefabHelper.PrefabToGameObject[PrefabType];

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains all the <see cref="Generator"/> instances.
        /// </summary>
        public static new IReadOnlyCollection<Generator> List => Scp079GeneratorToGenerator.Values;

        /// <summary>
        /// Gets a randomly selected <see cref="Generator"/>.
        /// </summary>
        /// <returns>A randomly selected <see cref="Generator"/> object.</returns>
        public static Generator Random => List.Random();

        /// <summary>
        /// Gets the base <see cref="Scp079Generator"/>.
        /// </summary>
        public Scp079Generator Base { get; }

        /// <summary>
        /// Gets the generator's <see cref="Room"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Generator))]
        public Room Room => room ??= Room.FindParentRoom(GameObject);

        /// <summary>
        /// Gets or sets the generator's state.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public GeneratorState State
        {
            get => (GeneratorState)Base.Network_flags;
            set => Base.Network_flags = (byte)value;
        }

        /// <summary>
        /// Gets or sets the generator's current time.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public short CurrentTime
        {
            get => Base.Network_syncTime;
            set => Base.Network_syncTime = value;
        }

        /// <summary>
        /// Gets the generator's dropdown speed.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Generator))]
        public float DropdownSpeed => Base.DropdownSpeed;

        /// <summary>
        /// Gets a value indicating whether the generator is ready to be activated.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Generator))]
        public bool IsReady => Base.ActivationReady;

        /// <summary>
        /// Gets or sets a value indicating whether the generator is engaged.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public bool IsEngaged
        {
            get => Base.Engaged;
            set => Base.Engaged = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is activating.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public bool IsActivating
        {
            get => Base.Activating;
            set => Base.Activating = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is open.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public bool IsOpen
        {
            get => Base.HasFlag(Base.Network_flags, Scp079Generator.GeneratorFlags.Open);
            set => Base.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is unlocked.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public bool IsUnlocked
        {
            get => Base.HasFlag(Base.Network_flags, Scp079Generator.GeneratorFlags.Unlocked);
            set => Base.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, value);
        }

        /// <summary>
        /// Gets or sets the generator's lever delay.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float LeverDelay
        {
            get => Base._leverDelay;
            set => Base._leverDelay = value;
        }

        /// <summary>
        /// Gets or sets current interaction cooldown.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float InteractionCooldown
        {
            get => Base._targetCooldown;
            set => Base._targetCooldown = value;
        }

        /// <summary>
        /// Gets or sets the generator's activation time.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float ActivationTime
        {
            get => Base._totalActivationTime;
            set => Base._totalActivationTime = value;
        }

        /// <summary>
        /// Gets or sets the generator's deactivation time.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float DeactivationTime
        {
            get => Base._totalDeactivationTime;
            set => Base._totalDeactivationTime = value;
        }

        /// <summary>
        /// Gets or sets the cooldown to wait before toggling the generator's panel.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float TogglePanelCooldown
        {
            get => Base._doorToggleCooldownTime;
            set => Base._doorToggleCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the interaction cooldown to wait after unlocking the generator.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float UnlockCooldown
        {
            get => Base._unlockCooldownTime;
            set => Base._unlockCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the interaction cooldown to wait after failing the generator's unlock interaction.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public float DeniedUnlockCooldown
        {
            get => Base._deniedCooldownTime;
            set => Base._deniedCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the last activator for the generator.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public Player LastActivator
        {
            get => Player.Get(Base._lastActivator.Hub);
            set => Base._lastActivator = value.Footprint;
        }

        /// <summary>
        /// Gets the generator position.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Generator))]
        public override Vector3 Position => Transform.position;

        /// <summary>
        /// Gets the generator rotation.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Generator))]
        public override Quaternion Rotation => Transform.rotation;

        /// <summary>
        /// Gets or sets the required permissions to interact with the generator.
        /// </summary>
        [EProperty(category: nameof(Generator))]
        public KeycardPermissions KeycardPermissions
        {
            get => (KeycardPermissions)Base._requiredPermission;
            set => Base._requiredPermission = (Interactables.Interobjects.DoorUtils.KeycardPermissions)value;
        }

        /// <summary>
        /// Spawns a <see cref="Generator"/>.
        /// </summary>
        /// <param name="position">The position to spawn it at.</param>
        /// <param name="rotation">The rotation to spawn it as.</param>
        /// <returns>The <see cref="Generator"/> that was spawned.</returns>
        public static Generator Spawn(Vector3 position, Quaternion rotation = default)
        {
            Scp079Generator generator = PrefabHelper.Spawn<Scp079Generator>(PrefabType, position, rotation);
            return Get(generator);
        }

        /// <summary>
        /// Gets the <see cref="Generator"/> belonging to the <see cref="Scp079Generator"/>, if any.
        /// </summary>
        /// <param name="scp079Generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <returns>A <see cref="Generator"/> or <see langword="null"/> if not found.</returns>
        public static Generator Get(Scp079Generator scp079Generator) => scp079Generator == null ? null :
            Scp079GeneratorToGenerator.TryGetValue(scp079Generator, out Generator generator) ? generator : new(scp079Generator);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> given the specified <see cref="GeneratorState"/>.
        /// </summary>
        /// <param name="state">The <see cref="GeneratorState"/> to search for.</param>
        /// <returns>The <see cref="Generator"/> with the given <see cref="GeneratorState"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Generator> Get(GeneratorState state)
            => List.Where(generator => generator.Base.HasFlag(generator.Base.Network_flags, (Scp079Generator.GeneratorFlags)state));

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Generator> Get(Func<Generator, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Try-get a <see cref="Generator"/> belonging to the <see cref="Scp079Generator"/>, if any.
        /// </summary>
        /// <param name="scp079Generator">The <see cref="Scp079Generator"/> instance.</param>
        /// <param name="generator">A <see cref="Generator"/> or <see langword="null"/> if not found.</param>
        /// <returns>Whether or not a generator was found.</returns>
        public static bool TryGet(Scp079Generator scp079Generator, out Generator generator)
        {
            generator = Get(scp079Generator);
            return generator is not null;
        }

        /// <summary>
        /// Try-get a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> given the specified <see cref="GeneratorState"/>.
        /// </summary>
        /// <param name="state">The <see cref="GeneratorState"/> to search for.</param>
        /// <param name="generators">A <see cref="IEnumerable{T}"/> of <see cref="Generator"/> matching the <see cref="GeneratorState"/>.</param>
        /// <returns>Whether or not at least one generator was found.</returns>
        public static bool TryGet(GeneratorState state, out IEnumerable<Generator> generators)
        {
            generators = Get(state);
            return generators.Any();
        }

        /// <summary>
        /// Try-get a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="generators">A <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains elements that satisfy the condition.</param>
        /// <returns>Whether or not at least one generator was found.</returns>
        public static bool TryGet(Func<Generator, bool> predicate, out IEnumerable<Generator> generators)
        {
            generators = Get(predicate);
            return generators.Any();
        }

        /// <summary>
        /// Denies the unlock.
        /// </summary>
        public void DenyUnlock() => Base.RpcDenied();

        /// <summary>
        /// Denies the unlock and resets the interaction cooldown.
        /// </summary>
        public void DenyUnlockAndResetCooldown()
        {
            InteractionCooldown = UnlockCooldown;
            DenyUnlock();
        }

        /// <summary>
        /// Sets the specified <see cref="KeycardPermissions"/> flag.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        /// <param name="isEnabled">A value indicating whether the flag is enabled.</param>
        public void SetPermissionFlag(KeycardPermissions flag, bool isEnabled)
        {
            Interactables.Interobjects.DoorUtils.KeycardPermissions permission = (Interactables.Interobjects.DoorUtils.KeycardPermissions)flag;

            Base._requiredPermission = Base._requiredPermission.ModifyFlags(isEnabled, permission);
        }

        /// <summary>
        /// Returns the Generator in a human-readable format.
        /// </summary>
        /// <returns>A string containing Generator-related data.</returns>
        public override string ToString() => $"{State} ({KeycardPermissions})";
    }
}