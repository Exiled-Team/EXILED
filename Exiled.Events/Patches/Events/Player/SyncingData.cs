// -----------------------------------------------------------------------
// <copyright file="SyncingData.cs" company="Exiled Team">
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
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="AnimationController.CallCmdSyncData(byte, Vector2)"/>.
    /// Adds the <see cref="Player.SyncingData"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.CallCmdSyncData))]
    public class SyncingData
    {
        /// <summary>
        /// Prefix of <see cref="AnimationController.CallCmdSyncData(byte, Vector2)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="AnimationController"/> instance.</param>
        /// <param name="state"><inheritdoc cref="SyncingDataEventArgs.CurrentAnimation"/></param>
        /// <param name="v2"><inheritdoc cref="SyncingDataEventArgs.Speed"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(AnimationController __instance, byte state, Vector2 v2)
        {
            if (!__instance._mSyncRateLimit.CanExecute(false))
                return false;

            var ev = new SyncingDataEventArgs(API.Features.Player.Get(__instance.gameObject), v2, state);

            Player.OnSyncingData(ev);

            return ev.IsAllowed;
        }
    }
}
