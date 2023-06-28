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
    ///     Patch the <see cref="PlayerInteract.UserCode_CmdDetonateWarhead" />.
    ///     Adds the <see cref="Handlers.Warhead.Starting" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdDetonateWarhead))]
    internal static class Starting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            Label returnLabel = generator.DefineLabel();

            // if (!Warhead.CanBeStarted)
            //   return;
            //
            // StartingEventArgs ev = new(Player.Get(component), true);
            //
            // Handlers.Warhead.OnStarting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (!Warhead.CanBestarted)
                    //    return;
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Warhead), nameof(Warhead.CanBeStarted))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // Player.Get(component)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StartingEventArgs ev = new(Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Warhead.OnStarting(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnStarting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Call, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}