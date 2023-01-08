// -----------------------------------------------------------------------
// <copyright file="StartingByServer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using API.Features;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    

    using static HarmonyLib.AccessTools;

    using Warhead = Handlers.Warhead;

    /// <summary>
    ///     Patch the <see cref="AlphaWarheadController.Update" />.
    ///     Adds the <see cref="Warhead.Starting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.ServerUpdateAutonuke))]
    internal static class StartingByServer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -4;
            int index = newInstructions.FindLastIndex(
                            instruction => instruction.Calls(Method(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation)))) + offset;

            Label returnLabel = generator.DefineLabel();

            // StartingEventArgs ev = new(Server.Host, true);
            //
            // Handlers.Warhead.OnStarting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Server.Host
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.Host))).MoveLabelsFrom(newInstructions[index]),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StartingEventArgs ev = new(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Warhead.OnStarting(ev);
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStarting))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}