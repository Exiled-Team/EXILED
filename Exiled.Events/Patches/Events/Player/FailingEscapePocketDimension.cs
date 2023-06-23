// -----------------------------------------------------------------------
// <copyright file="FailingEscapePocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)" />.
    ///     Adds the <see cref="Handlers.Player.FailingEscapePocketDimension" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class FailingEscapePocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            // --------- Player check ---------
            // The check is a check that this is a player, if it isn't a player, then we simply call return
            // if we don't, we'll get a NullReferenceException which is also thrown when we try to call the event.
            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse, returnLabel),

                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FailingEscapePocketDimensionEventArgs ev = new(Player, PocketDimensionTeleport, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FailingEscapePocketDimensionEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnFailingEscapePocketDimension(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}