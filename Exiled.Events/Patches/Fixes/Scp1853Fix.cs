// -----------------------------------------------------------------------
// <copyright file="Scp1853Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;

    using CustomPlayerEffects;

    using HarmonyLib;
    #pragma warning disable SA1313

    /// <summary>
    /// Patch the <see cref="Scp1853.OnUpdate"/>.
    /// Fix Spamming EnableEffect.
    /// </summary>
    [HarmonyPatch(typeof(Scp1853), nameof(Scp1853.OnUpdate))]
    internal static class Scp1853Fix
    {
        private static bool Prefix(Scp1853 __instance)
        {
            try
            {
                if (__instance.IsEnabled && __instance._scp207Reference.IsEnabled && !__instance.Hub.playerEffectsController.GetEffect<Poisoned>().IsEnabled)
                {
                    __instance.Hub.playerEffectsController.EnableEffect<Poisoned>(0f, false);
                }

                return false;
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Generic.Scp1853Fix: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
