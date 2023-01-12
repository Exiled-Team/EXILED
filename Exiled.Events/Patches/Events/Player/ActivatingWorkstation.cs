// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstation.cs" company="Exiled Team">
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

    using InventorySystem.Items.Firearms.Attachments;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="WorkstationController.ServerInteract" />.
    ///     Adds the <see cref="Handlers.Player.ActivatingWorkstation" /> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.ServerInteract))]
    internal static class ActivatingWorkstation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ActivatingWorkstationEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            // Remove "this.NetworkStatus = 1", to inject our custom logic
            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(ply)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ActivatingWorkstationEventArgs ev = new(Player, WorkstationController, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWorkstationEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnActivatingWorkstation(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWorkstation))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWorkstationEventArgs), nameof(ActivatingWorkstationEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // this.NetworkStatus = ev.NewStatus
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(ActivatingWorkstationEventArgs), nameof(ActivatingWorkstationEventArgs.NewStatus))),
                    new(OpCodes.Call, PropertySetter(typeof(WorkstationController), nameof(WorkstationController.NetworkStatus))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}