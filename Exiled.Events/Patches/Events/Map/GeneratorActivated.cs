// -----------------------------------------------------------------------
// <copyright file="GeneratorActivated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Generator079.CheckFinish"/>.
    /// Adds the <see cref="Map.GeneratorActivated"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.CheckFinish))]
    public class GeneratorActivated
    {
        /// <summary>
        /// Prefix of <see cref="Generator079.CheckFinish"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Generator079"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Generator079 __instance)
        {
            if (__instance.prevFinish || __instance.localTime > 0.0)
                return false;

            var ev = new GeneratorActivatedEventArgs(__instance);

            Map.OnGeneratorActivated(ev);

            __instance.prevFinish = true;
            __instance.epsenRenderer.sharedMaterial = __instance.matLetGreen;
            __instance.epsdisRenderer.sharedMaterial = __instance.matLedBlack;
            __instance.asource.PlayOneShot(__instance.unlockSound);

            return false;
        }
    }
}
