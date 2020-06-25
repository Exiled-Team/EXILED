// -----------------------------------------------------------------------
// <copyright file="PlacingBloodAndDecal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="WeaponManager.RpcPlaceDecal(bool, int, Vector3, Quaternion)"/>.
    /// Adds the <see cref="Map.PlacingDecal"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.RpcPlaceDecal))]
    public class PlacingBloodAndDecal
    {
        /// <summary>
        /// Prefix of <see cref="WeaponManager.RpcPlaceDecal(bool, int, Vector3, Quaternion)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="WeaponManager"/> instance.</param>
        /// <param name="isBlood">Indicates whether it's blood or not.</param>
        /// <param name="type"><inheritdoc cref="PlacingDecalEventArgs.Type"/></param>
        /// <param name="pos"><inheritdoc cref="PlacingDecalEventArgs.Position"/></param>
        /// <param name="rot"><inheritdoc cref="PlacingDecalEventArgs.Rotation"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(WeaponManager __instance, bool isBlood, int type, Vector3 pos, Quaternion rot)
        {
            if (isBlood)
            {
                var ev = new PlacingBloodEventArgs(
                    API.Features.Player.Get(__instance.gameObject),
                    pos,
                    __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass).bloodType,
                    1);

                pos = ev.Position;
                __instance._hub.characterClassManager.Classes.SafeGet(__instance._hub.characterClassManager.CurClass).bloodType = ev.Type;

                Map.OnPlacingBlood(ev);

                return ev.IsAllowed && Config.CanSpawnBlood;
            }
            else
            {
                var ev = new PlacingDecalEventArgs(API.Features.Player.Get(__instance.gameObject), pos, rot, type);

                Map.OnPlacingDecal(ev);

                pos = ev.Position;
                rot = ev.Rotation;
                type = ev.Type;

                return ev.IsAllowed;
            }
        }
    }
}
