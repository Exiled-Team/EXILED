// -----------------------------------------------------------------------
// <copyright file="InteractingEvents.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp914;

    using global::Scp914;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    using Scp914 = Handlers.Scp914;

    /// <summary>
    /// Patches <see cref="Scp914Controller.ServerInteract" />.
    /// Adds the <see cref="Scp914.Activating" /> event.
    /// </summary>
    [EventPatch(typeof(Scp914), nameof(Scp914.Activating))]
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class InteractingEvents
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingKnobSettingEventArgs));

            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ply)
                new CodeInstruction(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // scp914KnobSetting
                new CodeInstruction(OpCodes.Ldloc_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // ChangingKnobSettingEventArgs ev = new(referenceHub, scp914KnobSetting, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingKnobSettingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Scp914.OnChangingKnobSetting(ev);
                new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnChangingKnobSetting))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),

                // scp914KnobSetting = ev.KnobSetting
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.KnobSetting))),
                new(OpCodes.Stloc_1),
            });

            offset = -3;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ply)
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // ActivatingEventArgs ev2 = new(referenceHub, scp914KnobSetting, isAllowed);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingEventArgs))[0]),
                new(OpCodes.Dup),

                // Scp914.OnActivating(ev2);
                new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnActivating))),

                // if (!ev2.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingEventArgs), nameof(ActivatingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}