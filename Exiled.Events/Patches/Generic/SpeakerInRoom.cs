// -----------------------------------------------------------------------
// <copyright file="SpeakerInRoom.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using API.Features;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    /// <summary>
    /// Patches <see cref="Scp079Speaker.OnRegistered"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Speaker), nameof(Scp079Speaker.OnRegistered))]
    internal class SpeakerInRoom
    {
        private static void Postfix(Scp079Speaker __instance) => Room.RoomIdentifierToRoom[__instance.Room].SpeakersValue.Add(__instance);
    }
}