// -----------------------------------------------------------------------
// <copyright file="Detonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1313
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.Detonate"/>.
    /// Adds the WarheadDetonated event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonated
    {
        private static void Prefix() => Handlers.Warhead.OnDetonated();
    }
}
