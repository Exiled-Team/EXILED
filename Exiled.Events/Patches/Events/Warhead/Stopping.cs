// -----------------------------------------------------------------------
// <copyright file="Stopping.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    using Warhead = Handlers.Warhead;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.CancelDetonation(ReferenceHub)" />.
    /// Adds the <see cref="Warhead.Stopping" /> event.
    /// </summary>
    [EventPatch(typeof(Warhead), nameof(Warhead.Stopping))]
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), typeof(ReferenceHub))]
    internal static class Stopping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int oldCount = newInstructions.Count;

            Label returnLabel = generator.DefineLabel();

            // if (!AlphaWarheadController.inProgress)
            //   return;
            //
            // StoppingEventArgs ev = new(Player.Get(disabler), true);
            //
            // Handlers.Warhead.OnStopping(ev);
            //
            // if (!ev.IsAllowed || Warhead.IsWarheadLocked)
            //   return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // if (!AlphaWarheadController.InProgress)
                    //    return;
                    new(OpCodes.Call, PropertyGetter(typeof(AlphaWarheadController), nameof(AlphaWarheadController.InProgress))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Player.Get(disabler)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StoppingEventArgs ev = new(Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Warhead.OnStopping(ev);
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStopping))),

                    // if (!ev.IsAllowed || Warhead.IsWarheadLocked)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingEventArgs), nameof(StoppingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Call, PropertyGetter(typeof(API.Features.Warhead), nameof(API.Features.Warhead.IsLocked))),
                    new(OpCodes.Brtrue_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}