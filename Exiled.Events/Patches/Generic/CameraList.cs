// -----------------------------------------------------------------------
// <copyright file="CameraList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using MapGeneration;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079InteractableBase.OnRegistered"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079InteractableBase), nameof(Scp079InteractableBase.OnRegistered))]
    internal class CameraList
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder cameraBase = generator.DeclareLocal(typeof(Scp079Camera));
            Label ret = generator.DefineLabel();

            // if (this is Scp079Camera camera)
            //     Room.RoomIdentifierToRoom[Room].CamerasValue.Add(new Camera(camera));
            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Isinst, typeof(Scp079Camera)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, cameraBase.LocalIndex),
                    new(OpCodes.Brfalse_S, ret),
                    new(OpCodes.Ldsfld, Field(typeof(Room), nameof(Room.RoomIdentifierToRoom))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079Camera), nameof(Scp079Camera.Room))),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<RoomIdentifier, Room>), "get_Item")),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Room), nameof(Room.CamerasValue))),
                    new(OpCodes.Ldloc_S, cameraBase.LocalIndex),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Camera))[0]),
                    new(OpCodes.Callvirt, Method(typeof(List<Camera>), nameof(List<Camera>.Add), new[] { typeof(Camera) })),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp079InteractableBase.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp079InteractableBase), nameof(Scp079InteractableBase.OnDestroy))]
    internal class CameraListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            LocalBuilder cameraBase = generator.DeclareLocal(typeof(Scp079Camera));
            Label ret = generator.DefineLabel();

            // if (__instance is Scp079Camera cameraBase)
            //     Camera.Camera079ToCamera.Remove(cameraBase);
            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Isinst, typeof(Scp079Camera)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, cameraBase.LocalIndex),
                    new(OpCodes.Brfalse_S, ret),
                    new(OpCodes.Ldsfld, Field(typeof(Camera), nameof(Camera.Camera079ToCamera))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<Scp079Camera, Camera>), nameof(Dictionary<Scp079Camera, Camera>.Remove), new[] { typeof(Scp079Camera) })),
                    new(OpCodes.Pop),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}