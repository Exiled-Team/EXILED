// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1313

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.ServerHandleBlinkMessage"/>.
    /// Adds the <see cref="Handlers.Scp173.Blinking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerHandleBlinkMessage))]
    internal static class Blinking
    {
       // TODO: this
    }
}
