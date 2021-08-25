// -----------------------------------------------------------------------
// <copyright file="PlayableScpControllerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;

    /// <summary>
    /// Patches another round lock bug.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScpsController), nameof(PlayableScpsController.Update))]
    internal class PlayableScpControllerPatch
    {
        private static bool Prefix()
        {
            if (ReferenceHub.LocalHub == null)
                return false;
            return true;
        }
    }
}
