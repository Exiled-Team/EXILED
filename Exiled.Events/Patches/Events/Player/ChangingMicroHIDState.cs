// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="Exiled Team">
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

    using InventorySystem.Items.MicroHID;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MicroHIDItem.ServerSendStatus(HidStatusMessageType, byte)"/>.
    /// Adds the <see cref="Handlers.Player.OnChangingMicroHIDState"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class ChangingMicroHIDState
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label skipLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMicroHIDStateEventArgs));

            List<CodeInstruction> instructionsToAdd = new()
            {
                // Player.Get(this.Owner);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(MicroHIDItem), nameof(MicroHIDItem.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // state
                new CodeInstruction(OpCodes.Ldloc_0),

                // this.State
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // ChangingMicroHIDStateEventArgs ev = new(Player, MicroHIDItem, HidState, HidState, bool)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMicroHIDStateEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingMicroHIDState(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingMicroHIDState))),

                // if (!ev.IsAllowed)
                //    goto skipLabel;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, skipLabel),

                // this.State = ev.NewState
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.NewState))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),
                new CodeInstruction(OpCodes.Br, continueLabel),

                // this.State = state;
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(skipLabel),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            };

            int offset = 1;

            foreach (CodeInstruction instruction in newInstructions.FindAll(i => i.StoresField(Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State)))))
                newInstructions.InsertRange(newInstructions.IndexOf(instruction) + offset, instructionsToAdd);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}