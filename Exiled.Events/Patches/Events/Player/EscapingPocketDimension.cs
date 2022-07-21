// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="PocketDimensionTeleport.SuccessEscape"/> method.
    /// Adds the <see cref="Exiled.Events.Handlers.Player.EscapingPocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.SuccessEscape))]
    internal static class EscapingPocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingPocketDimensionEventArgs));
            Label retLabel = generator.DefineLabel();
            Label effectLabel = generator.DefineLabel();
            int offset = 2;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(Vector3), nameof(Vector3.Distance))) + offset;

            newInstructions.RemoveRange(index, 28);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Ldloc_S, 4),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingPocketDimensionEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscapingPocketDimension))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.TeleportPosition))),
                new(OpCodes.Callvirt, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ForcePosition), new[] { typeof(Vector3) })),
                new(OpCodes.Br_S, effectLabel),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))) + offset;

            newInstructions.RemoveRange(index, 15);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_S, 11),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingPocketDimensionEventArgs))[1]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscapingPocketDimension))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.TeleportPosition))),
                new(OpCodes.Callvirt, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ForcePosition), new[] { typeof(Vector3) })),
                new(OpCodes.Br_S, effectLabel),
            });

            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))) + offset;
            newInstructions[index].labels.Add(effectLabel);
            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
