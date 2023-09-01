// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using global::Scp914;

    using UnityEngine;

    /// <summary>
    /// A set of tools to modify SCP-914's behaviour.
    /// </summary>
    public static class Scp914
    {
        /// <summary>
        /// Gets the cached <see cref="global::Scp914.Scp914Controller"/>.
        /// </summary>
        public static Scp914Controller Scp914Controller => Scp914Controller.Singleton;

        /// <summary>
        /// Gets or sets SCP-914's <see cref="Scp914KnobSetting"/>.
        /// </summary>
        public static Scp914KnobSetting KnobStatus
        {
            get => Scp914Controller.Network_knobSetting;
            set => Scp914Controller.Network_knobSetting = value;
        }

        /// <summary>
        /// Gets or sets SCP-914's config mode.
        /// </summary>
        public static Scp914Mode ConfigMode
        {
            get => Scp914Controller._configMode.Value;
            set => Scp914Controller._configMode.Value = value;
        }

        /// <summary>
        /// Gets SCP-914's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public static GameObject GameObject => Scp914Controller.gameObject;

        /// <summary>
        /// Gets SCP-914's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform Transform => Scp914Controller.transform;

        /// <summary>
        /// Gets the position of SCP-914's intake chamber.
        /// </summary>
        public static Vector3 IntakePosition => Scp914Controller.IntakeChamber.localPosition;

        /// <summary>
        /// Gets the position of SCP-914's output chamber.
        /// </summary>
        public static Vector3 OutputPosition => Scp914Controller.OutputChamber.localPosition;

        /// <summary>
        /// Gets a value indicating whether SCP-914 is active and currently processing items.
        /// </summary>
        public static bool IsWorking => Scp914Controller._isUpgrading;

        /// <summary>
        /// Gets the intake booth <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform IntakeBooth => Scp914Controller.IntakeChamber;

        /// <summary>
        ///  Gets the output booth <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform OutputBooth => Scp914Controller.OutputChamber;

        /// <summary>
        /// Plays the SCP-914's sound.
        /// </summary>
        /// <param name="soundId">The soundId to play.</param>
        /// <remarks>There are two sounds only.
        /// The values to identify them are <c>0</c>, which stands for the soundId played when SCP-914 is being activated,
        /// and <c>1</c>, which stands for the soundId played when SCP-914's knob state is being changed.</remarks>
        public static void PlaySound(Scp914InteractCode soundId) => Scp914Controller.RpcPlaySound((byte)soundId);

        /// <summary>
        /// Starts SCP-914.
        /// </summary>
        /// <param name="player">Player who interacts with Scp914.</param>
        /// <param name="code">Activated code.</param>
        public static void Start(Player player = null, Scp914InteractCode code = Scp914InteractCode.Activate) => Scp914Controller.ServerInteract((player ?? Server.Host).ReferenceHub, (byte)code);
    }
}