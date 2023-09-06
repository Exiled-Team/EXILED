// -----------------------------------------------------------------------
// <copyright file="ReservedSlotPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ReservedSlot.HasReservedSlot(string, out bool)" />.
    ///     Adds the <see cref="Player.ReservedSlot" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.ReservedSlot))]
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
    internal static class ReservedSlotPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label baseGame = generator.DefineLabel();
            Label conditional = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(ReservedSlotsCheckEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj) + offset;

            newInstructions[index].WithLabels(baseGame);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // ReservedSlotCheckEventArgs ev = new(userid, flag);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReservedSlotsCheckEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnReservedSlot(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReservedSlot))),

                    // if (ev.Result == ReservedSlotEventResult.UseBaseGameSystem)
                    //     goto baseGame;
                    new(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReservedSlotsCheckEventArgs), nameof(ReservedSlotsCheckEventArgs.Result))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Beq_S, baseGame),

                    // if (ev.Result == ReservedSlotEventResult.AllowConnectionUnconditionally)
                    // {
                    //      bypass = true;
                    //      return true;
                    // }
                    new(OpCodes.Ldloc_S, ev),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReservedSlotsCheckEventArgs), nameof(ReservedSlotsCheckEventArgs.Result))),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Bne_Un_S, conditional),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Call, Method(typeof(ReservedSlotPatch), nameof(CallNwEvent))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Stind_I1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ret),

                    // if (ev.Result == ReservedSlotEventResult.CannotUseReservedSlots)
                    // {
                    //     bypass = false;
                    //     return false;
                    // }
                    new CodeInstruction(OpCodes.Ldloc_S, ev).WithLabels(conditional),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReservedSlotsCheckEventArgs), nameof(ReservedSlotsCheckEventArgs.Result))),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Bne_Un_S, continueLabel),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Br_S, skipLabel),

                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(continueLabel),
                    new CodeInstruction(OpCodes.Stloc_0).WithLabels(skipLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void CallNwEvent(string userId, bool hasReservedSlot)
        {
            EventManager.ExecuteEvent(new PlayerCheckReservedSlotEvent(userId, hasReservedSlot));
        }
    }
}