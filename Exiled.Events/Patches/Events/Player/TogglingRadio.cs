// -----------------------------------------------------------------------
// <copyright file="TogglingRadio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Radio;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RadioItem.ServerProcessCmd" />.
    /// Adds the <see cref="Handlers.Player.TogglingRadio" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingRadio))]
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class TogglingRadio
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            Predicate<CodeInstruction> match = instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerRadioToggleEvent))[0];
            int offset = -4;
            int index = newInstructions.FindIndex(match) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, PropertyGetter(typeof(RadioItem), nameof(RadioItem.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // TogglingRadioEventArgs ev = new TogglingRadioEventArgs(Player, RadioItem, bool, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingRadioEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Player.OnTogglingRadio(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingRadio))),

                // if(!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingRadioEventArgs), nameof(TogglingRadioEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            index = newInstructions.FindLastIndex(match) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, PropertyGetter(typeof(RadioItem), nameof(RadioItem.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // false
                new(OpCodes.Ldc_I4_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // TogglingRadioEventArgs ev = new TogglingRadioEventArgs(Player, Radio, bool);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingRadioEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Player.OnTogglingRadio(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingRadio))),

                // if(!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingRadioEventArgs), nameof(TogglingRadioEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
