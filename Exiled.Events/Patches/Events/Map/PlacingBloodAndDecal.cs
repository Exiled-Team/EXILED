// -----------------------------------------------------------------------
// <copyright file="PlacingBloodAndDecal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="GunShoot.SpawnDecal"/>.
    /// Adds the <see cref="Map.PlacingDecal"/> event.
    /// </summary>
    [HarmonyPatch(typeof(GunShoot), nameof(GunShoot.SpawnDecal))]
    internal static class PlacingBloodAndDecal
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_2) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingDecalEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GunShoot), nameof(GunShoot.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingDecalEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnPlacingDecal))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingDecalEventArgs), nameof(PlacingDecalEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlacingDecalEventArgs), nameof(PlacingDecalEventArgs.TypeObject))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingDecalEventArgs), nameof(PlacingDecalEventArgs.Position))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlacingDecalEventArgs), nameof(PlacingDecalEventArgs.Rotation))),
                new CodeInstruction(OpCodes.Call, Method(typeof(GameObject), nameof(GameObject.Instantiate), new[] { typeof(GameObject), typeof(Vector3), typeof(Quaternion) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(PlacingBloodAndDecal), nameof(GetTransform))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Transform), nameof(Transform.SetParent), new[] { typeof(Transform) })),
                new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static Transform GetTransform(RaycastHit hit) => hit.collider.transform;
    }
}
