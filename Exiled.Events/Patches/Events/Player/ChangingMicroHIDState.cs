// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="Exiled Team">
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
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.MicroHID;

    using NorthwoodLib.Pools;

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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label skipLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingMicroHIDStateEventArgs));
            List<CodeInstruction> instructionsToAdd = new()
            {
                // Player.Get(this.Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MicroHIDItem), nameof(MicroHIDItem.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // state
                new(OpCodes.Ldloc_0),

                // this.State
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new ChangingMicroHIDStateEventArgs(Player, MicroHIDItem, HidState, HidState, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingMicroHIDStateEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingMicroHIDState(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingMicroHIDState))),

                // if (!ev.IsAllowed)
                // goto RETURN_LABEL
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, skipLabel),

                // this.State = ev.NewState
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingMicroHIDStateEventArgs), nameof(ChangingMicroHIDStateEventArgs.NewState))),
                new(OpCodes.Stfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),
                new(OpCodes.Br, continueLabel),

                // this.State = state;
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Stfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            };
            int offset = 1;

            foreach (CodeInstruction instruction in newInstructions.FindAll(i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))))
                newInstructions.InsertRange(newInstructions.IndexOf(instruction) + offset, instructionsToAdd);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
