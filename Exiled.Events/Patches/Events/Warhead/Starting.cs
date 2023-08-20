// -----------------------------------------------------------------------
// <copyright file="Starting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="AlphaWarheadController.StartDetonation" />.
    ///     Adds the <see cref="Handlers.Warhead.Starting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    internal static class Starting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(StartingEventArgs));

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.StoresField(Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController._isAutomatic)))) + offset;

            // StartingEventArgs ev = new(Player.Get(hub), isAutomatic, true);
            //
            // Handlers.Warhead.OnStarting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // IsAutomatic = ev.IsAuto;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // isAutomatic
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StartingEventArgs ev = new(Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Warhead.OnStarting(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnStarting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Call, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // IsAutomatic = ev.IsAuto;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAuto))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}