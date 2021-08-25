// -----------------------------------------------------------------------
// <copyright file="RoundRestartFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using HarmonyLib;

    /// <summary>
    /// Patches server round end being broken.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.RoundInProgress))]
    internal class RoundRestartFix
    {
        private static bool Prefix(ref bool __result)
        {
            if (ReferenceHub.LocalHub == null || RoundSummary.singleton == null)
            {
                __result = false;

                return false;
            }

            return true;
        }
    }
}
