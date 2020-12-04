// -----------------------------------------------------------------------
// <copyright file="ChangingCamera.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.CallCmdSwitchCamera(ushort, bool)"/>.
    /// Adds the <see cref="Scp079.ChangingCamera"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdSwitchCamera))]
    internal static class ChangingCamera
    {
        private static bool Prefix(Scp079PlayerScript __instance, ushort cameraId, bool lookatRotation)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true))
                {
                    return false;
                }

                if (!__instance.iAm079)
                {
                    return false;
                }

                Camera079 camera = null;
                foreach (Camera079 camera2 in Scp079PlayerScript.allCameras)
                {
                    if (camera2.cameraId == cameraId)
                    {
                        camera = camera2;
                    }
                }

                if (camera == null)
                {
                    return false;
                }

                float num = __instance.CalculateCameraSwitchCost(camera.transform.position);
                bool isAllowed = num <= __instance.curMana;
                ChangingCameraEventArgs ev = new ChangingCameraEventArgs(API.Features.Player.Get(__instance.gameObject), camera, num, isAllowed);
                Scp079.OnChangingCamera(ev);
                if (ev.IsAllowed)
                {
                    __instance.RpcSwitchCamera(ev.Camera.cameraId, lookatRotation);
                    __instance.Mana -= ev.APCost;
                    __instance.currentCamera = ev.Camera;
                }
                else if (ev.APCost > __instance.curMana)
                {
                    __instance.RpcNotEnoughMana(ev.APCost, __instance.curMana);
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"{typeof(ChangingCamera).FullName}.{nameof(Prefix)}:\n{e}");

                return true;
            }
        }
    }
}
