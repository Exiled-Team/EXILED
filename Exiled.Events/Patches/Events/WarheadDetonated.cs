// -----------------------------------------------------------------------
// <copyright file="WarheadDetonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.Detonate"/>.
    /// Adds the WarheadDetonated event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    public class WarheadDetonated
    {
        /// <summary>
        /// Prefix of <see cref="AlphaWarheadController.Detonate"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="AlphaWarheadController"/> instance.</param>
        public static void Prefix(AlphaWarheadController __instance) => Map.OnWarheadDetonated();
    }
}