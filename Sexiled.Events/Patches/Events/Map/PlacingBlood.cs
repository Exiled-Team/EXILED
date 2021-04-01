// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Sexiled.Events;
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

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
            var ev = new PlacingBloodEventArgs(API.Features.Player.Get(__instance.gameObject), pos, type, f);

            Handlers.Map.OnPlacingBlood(ev);

            pos = ev.Position;
            type = ev.Type;
            f = ev.Multiplier;

            return ev.IsAllowed && Sexiled.Events.Events.Instance.Config.CanSpawnBlood;
        }
    }
}
