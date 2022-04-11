// -----------------------------------------------------------------------
// <copyright file="Generator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;

    using MapGeneration.Distributors;

    using UnityEngine;

    /// <summary>
    /// The in-game Scp079Generator.
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Generator"/> on the map.
        /// </summary>
        internal static readonly List<Generator> GeneratorValues = new List<Generator>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="scp079Generator">The <see cref="Scp079Generator"/>.</param>
        internal Generator(Scp079Generator scp079Generator) => Base = scp079Generator;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> which contains all the <see cref="Generator"/> instances.
        /// </summary>
        public static IEnumerable<Generator> List => GeneratorValues;

        /// <summary>
        /// Gets the base <see cref="Scp079Generator"/>.
        /// </summary>
        public Scp079Generator Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the generator.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the generator's <see cref="Room"/>.
        /// </summary>
        public Room Room => Map.FindParentRoom(GameObject);

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
        public static Generator Get(Scp079Generator scp079Generator) => List.FirstOrDefault(generator => generator.Base == scp079Generator);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Generator"/> given the specified <see cref="GeneratorState"/>.
        /// </summary>
        /// <param name="state">The <see cref="GeneratorState"/> to search for.</param>
        /// <returns>The <see cref="Generator"/> with the given <see cref="GeneratorState"/> or <see langword="null"/> if not found.</returns>
        public static IEnumerable<Generator> Get(GeneratorState state) => List.Where(generator => generator.Base.HasFlag(generator.Base.Network_flags, (Scp079Generator.GeneratorFlags)state));

        /// <summary>
        /// Denies the unlock.
        /// </summary>
        public void DenyUnlock() => Base.RpcDenied();

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
    }
}
