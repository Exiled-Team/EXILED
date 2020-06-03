// -----------------------------------------------------------------------
// <copyright file="AntiFly.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    #pragma warning disable SA1313
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.AntiFly(Vector3, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AntiFly))]
    public class AntiFly
    {
        /// <summary>
        /// Prefix of <see cref="PlayerMovementSync.AntiFly(Vector3, bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerMovementSync"/> instance.</param>
        /// <param name="pos">The player's position.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerMovementSync __instance, Vector3 pos) => Config.IsAntyFlyEnabled;
    }
}