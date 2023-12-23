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
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;

    using MapGeneration.Distributors;

    using UnityEngine;

    /// <summary>
    /// Wrapper class for <see cref="Scp079Generator"/>.
    /// </summary>
    public class Generator : GameEntity, IWrapper<Scp079Generator>, IWorldSpace
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Generator"/> on the map.
        /// </summary>
        internal static readonly Dictionary<Scp079Generator, Generator> Scp079GeneratorToGenerator = new();
        private Room room;

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="scp079Generator">The <see cref="Scp079Generator"/>.</param>
        internal Generator(Scp079Generator scp079Generator)
        {
            Base = scp079Generator;
            Scp079GeneratorToGenerator.Add(scp079Generator, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains all the <see cref="Generator"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Generator> List => Scp079GeneratorToGenerator.Values;

        /// <summary>
        /// Gets the base <see cref="Scp079Generator"/>.
        /// </summary>
        public Scp079Generator Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the generator.
        /// </summary>
        public override GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/> of the generator.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the generator's <see cref="Room"/>.
        /// </summary>
        public Room Room => room ??= Room.FindParentRoom(GameObject);

        /// <summary>
        /// Gets or sets the generator' state.
        /// </summary>
        public GeneratorState State
        {
            get => (GeneratorState)Base.Network_flags;
            set => Base.Network_flags = (byte)value;
        }

        /// <summary>
        /// Gets or sets the generator's current time.
        /// </summary>
        public short CurrentTime
        {
            get => Base.Network_syncTime;
            set => Base.Network_syncTime = value;
        }

        /// <summary>
        /// Gets the generator's dropdown speed.
        /// </summary>
        public float DropdownSpeed => Base.DropdownSpeed;

        /// <summary>
        /// Gets a value indicating whether the generator is ready to be activated.
        /// </summary>
        public bool IsReady => Base.ActivationReady;

        /// <summary>
        /// Gets or sets a value indicating whether the generator is engaged.
        /// </summary>
        public bool IsEngaged
        {
            get => Base.Engaged;
            set => Base.Engaged = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is activating.
        /// </summary>
        public bool IsActivating
        {
            get => Base.Activating;
            set => Base.Activating = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is open.
        /// </summary>
        public bool IsOpen
        {
            get => Base.HasFlag(Base.Network_flags, Scp079Generator.GeneratorFlags.Open);
            set => Base.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the generator is unlocked.
        /// </summary>
        public bool IsUnlocked
        {
            get => Base.HasFlag(Base.Network_flags, Scp079Generator.GeneratorFlags.Unlocked);
            set => Base.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, value);
        }

        /// <summary>
        /// Gets or sets the generator's lever delay.
        /// </summary>
        public float LeverDelay
        {
            get => Base._leverDelay;
            set => Base._leverDelay = value;
        }

        /// <summary>
        /// Gets or sets current interaction cooldown.
        /// </summary>
        public float InteractionCooldown
        {
            get => Base._targetCooldown;
            set => Base._targetCooldown = value;
        }

        /// <summary>
        /// Gets or sets the generator's activation time.
        /// </summary>
        public float ActivationTime
        {
            get => Base._totalActivationTime;
            set => Base._totalActivationTime = value;
        }

        /// <summary>
        /// Gets or sets the generator's deactivation time.
        /// </summary>
        public float DeactivationTime
        {
            get => Base._totalDeactivationTime;
            set => Base._totalDeactivationTime = value;
        }

        /// <summary>
        /// Gets or sets the cooldown to wait before toggling the generator's panel.
        /// </summary>
        public float TogglePanelCooldown
        {
            get => Base._doorToggleCooldownTime;
            set => Base._doorToggleCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the interaction cooldown to wait after unlocking the generator.
        /// </summary>
        public float UnlockCooldown
        {
            get => Base._unlockCooldownTime;
            set => Base._unlockCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the interaction cooldown to wait after failing the generator's unlock interaction.
        /// </summary>
        public float DeniedUnlockCooldown
        {
            get => Base._deniedCooldownTime;
            set => Base._deniedCooldownTime = value;
        }

        /// <summary>
        /// Gets or sets the last activator for the generator.
        /// </summary>
        public Player LastActivator
        {
            get => Player.Get(Base._lastActivator.Hub);
            set => Base._lastActivator = value.Footprint;
        }

        /// <summary>
        /// Gets the generator position.
        /// </summary>
        public Vector3 Position => Base.transform.position;

        /// <summary>
        /// Gets the generator rotation.
        /// </summary>
        public Quaternion Rotation => Base.transform.rotation;

        /// <summary>
        /// Gets or sets the required permissions to interact with the generator.
        /// </summary>
        public KeycardPermissions KeycardPermissions
        {
            get => (KeycardPermissions)Base._requiredPermission;
            set => Base._requiredPermission = (Interactables.Interobjects.DoorUtils.KeycardPermissions)value;
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
        /// <param name="predicate">The condition to satify.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains elements that satify the condition.</returns>
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
        /// <param name="predicate">The condition to satify.</param>
        /// <param name="generators">A <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains elements that satify the condition.</param>
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

            if (isEnabled)
                Base._requiredPermission |= permission;
            else
                Base._requiredPermission &= ~permission;
        }

        /// <summary>
        /// Returns the Generator in a human-readable format.
        /// </summary>
        /// <returns>A string containing Generator-related data.</returns>
        public override string ToString() => $"{State} ({KeycardPermissions})";
    }
}