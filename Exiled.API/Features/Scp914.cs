// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using global::Scp914;

    using UnityEngine;

    using Utils.ConfigHandler;

    /// <summary>
    /// A set of tools to modify SCP-914's behaviour.
    /// </summary>
    public static class Scp914 {
        private static Scp914Controller scp914Controller;

        /// <summary>
        /// Gets the cached <see cref="Scp914Controller"/>.
        /// </summary>
        public static Scp914Controller Scp914Controller {
            get {
                if (scp914Controller == null)
                    scp914Controller = Object.FindObjectOfType<Scp914Controller>();

                return scp914Controller;
            }
        }

        /// <summary>
        /// Gets or sets SCP-914 <see cref="Scp914KnobSetting"/>.
        /// </summary>
        public static Scp914KnobSetting KnobStatus {
            get => Scp914Controller.Network_knobSetting;
            set => Scp914Controller.Network_knobSetting = value;
        }

        /// <summary>
        /// Gets or sets SCP-914 config mode.
        /// </summary>
        public static ConfigEntry<Scp914Mode> ConfigMode {
            get => Scp914Controller._configMode;
            set => Scp914Controller._configMode = value;
        }

        /// <summary>
        /// Gets a value indicating whether the SCP-914 was activated and is currently processing items.
        /// </summary>
        public static bool IsWorking => Scp914Controller._isUpgrading;

        /// <summary>
        /// Gets the intake booth <see cref="Transform"/>.
        /// </summary>
        public static Transform IntakeBooth => Scp914Controller._intakeChamber;

        /// <summary>
        ///  Gets the output booth <see cref="Transform"/>.
        /// </summary>
        public static Transform OutputBooth => Scp914Controller._outputChamber;

        /// <summary>
        /// Plays the Scp914's sound.
        /// </summary>
        /// <param name="soundId">The soundId to play.</param>
        /// <remarks>There are two sounds only.
        /// The values to identify them are 0, which stands for the soundId played when the Scp914 is being activated,
        /// and 1, which stands for the soundId played when the Scp914's knob state is being changed.</remarks>
        public static void PlaySound(byte soundId) => scp914Controller.RpcPlaySound(soundId);

        /// <summary>
        /// Starts the SCP-914.
        /// </summary>
        public static void Start() => Scp914Controller.ServerInteract(Server.Host.ReferenceHub, (byte)Scp914InteractCode.Activate);
    }
}
