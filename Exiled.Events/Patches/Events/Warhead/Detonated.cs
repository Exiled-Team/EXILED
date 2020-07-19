// -----------------------------------------------------------------------
// <copyright file="Detonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1313
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.Detonate"/>.
    /// Adds the WarheadDetonated event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonated
    {
        private static void Prefix() => Warhead.OnDetonated();
    }
}
