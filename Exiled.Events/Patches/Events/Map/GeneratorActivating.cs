// -----------------------------------------------------------------------
// <copyright file="GeneratorActivating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.Engaged" />.
    /// Adds the <see cref="Map.GeneratorActivating" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.GeneratorActivating))]
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Engaged), MethodType.Setter)]
    internal static class GeneratorActivating
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retModLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(GeneratorActivatingEventArgs));

            // GeneratorActivatedEventArgs ev = new(this, true);
            //
            // Map.OnGeneratorActivated(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // GeneratorActivatedEventArgs ev = new(Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GeneratorActivatingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Map.OnGeneratorActivated(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnGeneratorActivating))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatingEventArgs), nameof(GeneratorActivatingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retModLabel),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (ev.IsAllowed)
                    //    return;
                    new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(retModLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatingEventArgs), nameof(GeneratorActivatingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue, returnLabel),

                    // this._leverStopwatch.Restart
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._leverStopwatch))),
                    new(OpCodes.Callvirt, Method(typeof(Stopwatch), nameof(Stopwatch.Restart))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}