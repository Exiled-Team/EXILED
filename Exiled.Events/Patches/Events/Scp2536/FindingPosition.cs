// -----------------------------------------------------------------------
// <copyright file="FindingPosition.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536Controller.CanFindPosition"/>
    /// to add <see cref="Handlers.Scp2536.FindingPosition"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.FindingPosition))]
    [HarmonyPatch(typeof(Scp2536Controller), nameof(Scp2536Controller.CanFindPosition))]
    internal class FindingPosition
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldloc_1);

            IEnumerable<Label> labels = newInstructions[index].labels;

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(target);
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // foundSpot
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldind_Ref),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FindingPositionEventArgs ev = new(Player, Scp2536Spawnpoint, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FindingPositionEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Handlers.Scp2536.OnFindingPosition(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnFindingPosition))),

                    // foundSpot = ev.Spawnpoint;
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FindingPositionEventArgs), nameof(FindingPositionEventArgs.Spawnpoint))),
                    new(OpCodes.Stind_Ref),

                    // return ev.IsAllowed;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FindingPositionEventArgs), nameof(FindingPositionEventArgs.IsAllowed))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}