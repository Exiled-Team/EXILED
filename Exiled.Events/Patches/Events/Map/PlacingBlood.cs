// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Exiled.Events;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RpcPlaceBlood(Vector3, int, float)"/>.
    /// Adds the <see cref="Map.PlacingBlood"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
    internal static class PlacingBlood
    {
        private static bool Prefix(CharacterClassManager __instance, ref Vector3 pos, ref int type, ref float f)
        {
            PlacingBloodEventArgs ev = new PlacingBloodEventArgs(API.Features.Player.Get(__instance.gameObject), pos, type, f);

            Map.OnPlacingBlood(ev);

            pos = ev.Position;
            type = ev.Type;
            f = ev.Multiplier;

            return ev.IsAllowed && Events.Instance.Config.CanSpawnBlood;
        }
    }
}
