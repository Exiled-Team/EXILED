// -----------------------------------------------------------------------
// <copyright file="TogglingFlashlight.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Flashlight;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashlightNetworkHandler.ServerProcessMessage"/>.
    /// Adds the <see cref="Player.TogglingFlashlight"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FlashlightNetworkHandler), nameof(FlashlightNetworkHandler.ServerProcessMessage))]
    internal static class TogglingFlashlight
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            List<CodeInstruction> safeInstructions = new List<CodeInstruction>();

            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingFlashlightEventArgs));

            Label retLabel = generator.DefineLabel();
            Label jmpLabel = generator.DefineLabel();

            safeInstructions.AddRange(newInstructions);

            newInstructions.InsertRange(0, safeInstructions);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Round), nameof(API.Features.Round.IsStarted))),
                new CodeInstruction(OpCodes.Brtrue_S, jmpLabel),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(jmpLabel),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FlashlightNetworkHandler.FlashlightMessage), nameof(FlashlightNetworkHandler.FlashlightMessage.NewState))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingFlashlightEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingFlashlight))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingFlashlightEventArgs), nameof(TogglingFlashlightEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, retLabel),
            });

            offset = -6;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions.RemoveRange(index, 2);

            offset = -4;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingFlashlightEventArgs), nameof(TogglingFlashlightEventArgs.NewState))),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions.RemoveRange(index, 2);

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TogglingFlashlightEventArgs), nameof(TogglingFlashlightEventArgs.NewState))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
