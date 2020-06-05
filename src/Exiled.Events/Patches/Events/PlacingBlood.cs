// -----------------------------------------------------------------------
// <copyright file="PlacingBlood.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.RpcPlaceBlood(Vector3, int, float)"/>.
    /// Adds the <see cref="Map.PlacingBlood"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
    public class PlacingBlood
    {
        /// <summary>
        /// Prefix of <see cref="CharacterClassManager.RpcPlaceBlood(Vector3, int, float)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <param name="pos"><inheritdoc cref="PlacingBloodEventArgs.Position"/></param>
        /// <param name="type"><inheritdoc cref="PlacingBloodEventArgs.Type"/></param>
        /// <param name="f"><inheritdoc cref="PlacingBloodEventArgs.Multiplier"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance, Vector3 pos, int type, float f)
        {
            var ev = new PlacingBloodEventArgs(API.Features.Player.Get(__instance.gameObject), pos, type, f);

            Map.OnPlacingBlood(ev);

            pos = ev.Position;
            type = ev.Type;
            f = ev.Multiplier;

            return ev.IsAllowed && Config.CanSpawnBlood;
        }
    }
}
