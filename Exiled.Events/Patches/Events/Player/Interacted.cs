// -----------------------------------------------------------------------
// <copyright file="Interacted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerInteract.OnInteract"/>.
    /// Adds the <see cref="Player.Interacted"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.OnInteract))]
    internal class Interacted
    {
        private static void Prefix(PlayerInteract __instance) => Player.OnInteracted(new InteractedEventArgs(API.Features.Player.Get(__instance.gameObject)));
    }
}
