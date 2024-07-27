// -----------------------------------------------------------------------
// <copyright file="CameraList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using API.Features;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;

    /// <summary>
    /// Patches <see cref="Scp079InteractableBase.OnRegistered"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079InteractableBase), nameof(Scp079InteractableBase.OnRegistered))]
    internal class CameraList
    {
        private static void Postfix(Scp079InteractableBase __instance)
        {
            if (__instance is Scp079Camera camera)
                 Room.RoomIdentifierToRoom[__instance.Room].CamerasValue.Add(Camera.Get(camera));
        }
    }

    /// <summary>
    /// Patches <see cref="Scp079InteractableBase.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079InteractableBase), nameof(Scp079InteractableBase.OnDestroy))]
    internal class CameraListRemove
    {
        private static void Postfix(Scp079InteractableBase __instance)
        {
            if (__instance is Scp079Camera cameraBase)
                Camera.Camera079ToCamera.Remove(cameraBase);
        }
    }
}