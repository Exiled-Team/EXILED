// -----------------------------------------------------------------------
// <copyright file="TogglingFlashlight.cs" company="Exiled Team">
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

    using HarmonyLib;

    using InventorySystem.Items.ToggleableLights;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashlightNetworkHandler.ServerProcessMessage" />.
    /// Adds the <see cref="Handlers.Player.TogglingFlashlight" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingFlashlight))]
    [HarmonyPatch(typeof(FlashlightNetworkHandler), nameof(FlashlightNetworkHandler.ServerProcessMessage))]
    internal static class TogglingFlashlight
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingFlashlightEventArgs));

            int offset = -8;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),

                    // ToggleableLightItemBase
                    new(OpCodes.Ldloc_1),

                    // msg.NewState
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(FlashlightNetworkHandler.FlashlightMessage), nameof(FlashlightNetworkHandler.FlashlightMessage.NewState))),

                    // TogglingFlashlightEventArgs ev = new(Player, ToggleableLightItemBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingFlashlightEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnTogglingFlashlight(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingFlashlight))),
                });

            // Remove all msg.NewState to inject or logic
            for (int i = 0; i < 2; i++)
            {
                offset = -1;
                index = newInstructions.FindLastIndex(i => i.LoadsField(Field(typeof(FlashlightNetworkHandler.FlashlightMessage), nameof(FlashlightNetworkHandler.FlashlightMessage.NewState)))) + offset;

                newInstructions.RemoveRange(index, 2);
                newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingFlashlightEventArgs), nameof(TogglingFlashlightEventArgs.NewState))),
                });
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
