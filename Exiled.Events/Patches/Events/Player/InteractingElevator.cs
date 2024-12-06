// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingElevator" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingElevator))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.ServerInteract))]
    internal class InteractingElevator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = -1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_0) + offset;

            // InteractingElevatorEventArgs ev = new(Player.Get(referenceHub), elevatorChamber, true);
            //
            // Handlers.Player.OnInteractingElevator(ev);
            //
            // if (!ev.IsAllowed)
            //     continue;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // elevatorChamber
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // InteractingElevatorEventArgs ev = new(Player, ElevatorChamber, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingElevatorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnInteractingElevator(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingElevator))),

                    // if (!ev.IsAllowed)
                    //     continue;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingElevatorEventArgs), nameof(InteractingElevatorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}