// -----------------------------------------------------------------------
// <copyright file="RoundStarting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using GameCore;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="RoundStart.NetworkTimer" />.
    /// Adds the <see cref="Server.RoundStarting" /> event.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RoundStarting))]
    [HarmonyPatch(typeof(RoundStart), nameof(RoundStart.NetworkTimer), MethodType.Setter)]
    internal static class RoundStarting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();
            Label contlabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(ret);
            LocalBuilder ev = generator.DeclareLocal(typeof(RoundStartingEventArgs));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Getting a old value
                    new CodeInstruction(OpCodes.Ldarg_1),

                    // Getting a new value
                    new CodeInstruction(OpCodes.Ldc_I4, -1),

                    // If the value is not equal, jump
                    new CodeInstruction(OpCodes.Bne_Un, contlabel),

                    // RoundStartingEventArgs ev = new
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoundStartingEventArgs))[0]),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Server.OnRoundStarting(ev)
                    new CodeInstruction(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRoundStarting))),
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),

                    // If isallowed = false
                    new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RoundStartingEventArgs), nameof(RoundStartingEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, ret),

                    // Empty opcode for jump
                    new CodeInstruction(OpCodes.Nop).WithLabels(contlabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
