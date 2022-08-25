// -----------------------------------------------------------------------
// <copyright file="FailingEscapePocketDimension.cs" company="Exiled Team">
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
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)" />.
    ///     Adds the <see cref="Handlers.Player.EscapingPocketDimension" /> and
    ///     <see cref="Handlers.Player.FailingEscapePocketDimension" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class FailingEscapePocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder exiledPlayerLocal = generator.DeclareLocal(typeof(Player));

            // --------- Player check ---------
            // The check is a check that this is a player, if it isn't a player, then we simply call return
            // if we don't, we'll get a NullReferenceException which is also thrown when we try to call the event.

            // Find the first ReferenceHub.TryGetHubNetID method
            int offset = 3;
            int index = newInstructions.FindIndex(i => (i.opcode == OpCodes.Call) && ((MethodInfo)i.operand == Method(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHubNetID)))) + offset;

            // Get the return label from the instruction at the index.
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, exiledPlayerLocal.LocalIndex),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // ----------- FailingEscapePocketDimension-------------
            offset = 2;
            index = newInstructions.FindLastIndex(
                i => (i.opcode == OpCodes.Ldsfld) && ((FieldInfo)i.operand ==
                                                      Field(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.DebugBool)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S, exiledPlayerLocal.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FailingEscapePocketDimensionEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}