// -----------------------------------------------------------------------
// <copyright file="MuteHandlerClear.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="MuteHandler.Reload"/>.
    /// </summary>
    [HarmonyPatch(typeof(MuteHandler), nameof(MuteHandler.Reload))]
    public class MuteHandlerClear
    {
        /// <summary>
        /// Fix the <see cref="MuteHandler.Reload"/> method.
        /// </summary>
        public static void Prefix() => MuteHandler.Mutes?.Clear();
    }
}