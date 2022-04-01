// -----------------------------------------------------------------------
// <copyright file="MuteHandlerClear.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes {
    using HarmonyLib;

    /// <summary>
    /// Fixes <see cref="MuteHandler.Reload"/> method.
    /// </summary>
    [HarmonyPatch(typeof(MuteHandler), nameof(MuteHandler.Reload))]
    internal static class MuteHandlerClear {
        private static void Prefix() => MuteHandler.Mutes?.Clear();
    }
}
