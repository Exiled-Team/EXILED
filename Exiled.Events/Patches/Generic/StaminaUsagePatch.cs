// -----------------------------------------------------------------------
// <copyright file="StaminaUsagePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;

#pragma warning disable SA1313
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Stamina.ProcessStamina()"/>.
    /// </summary>
    [HarmonyPatch(typeof(Stamina), nameof(Stamina.ProcessStamina))]
    internal class StaminaUsagePatch
    {
        private static bool Prefix(Stamina __instance) => Player.Get(__instance._hub)?.IsUsingStamina ?? true;
    }
}