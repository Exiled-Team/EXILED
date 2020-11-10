// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using global::Scp914;

    using Mirror;

    using UnityEngine;

    using Utils.ConfigHandler;

    /// <summary>
    /// A set of tools to modify SCP-914's behaviour.
    /// </summary>
    public static class Scp914
    {
        /// <summary>
        /// Gets or sets SCP-914 <see cref="Scp914Knob"/>.
        /// </summary>
        public static Scp914Knob KnobStatus
        {
            get => Scp914Machine.singleton.NetworkknobState;
            set => Scp914Machine.singleton.NetworkknobState = value;
        }

        /// <summary>
        /// Gets or sets SCP-914 recipes.
        /// </summary>
        public static Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> Recipes
        {
            get => Scp914Machine.singleton.recipesDict;
            set => Scp914Machine.singleton.recipesDict = value;
        }

        /// <summary>
        /// Gets or sets SCP-914 config mode.
        /// </summary>
        public static ConfigEntry<Scp914Mode> ConfigMode
        {
            get => Scp914Machine.singleton.configMode;
            set => Scp914Machine.singleton.configMode = value;
        }

        /// <summary>
        /// Gets a value indicating whether the SCP-914 was activated and is currently processing items.
        /// </summary>
        public static bool IsWorking => Scp914Machine.singleton.working;

        /// <summary>
        /// Gets the intake booth <see cref="Transform"/>.
        /// </summary>
        public static Transform IntakeBooth => Scp914Machine.singleton.intake;

        /// <summary>
        ///  Gets the output booth <see cref="Transform"/>.
        /// </summary>
        public static Transform OutputBooth => Scp914Machine.singleton.output;

        /// <summary>
        /// Starts the SCP-914.
        /// </summary>
        public static void Start() => Scp914Machine.singleton.RpcActivate(NetworkTime.time);
    }
}
