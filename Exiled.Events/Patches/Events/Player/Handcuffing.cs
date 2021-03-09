// -----------------------------------------------------------------------
// <copyright file="Handcuffing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Handcuffs.CallCmdCuffTarget(GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.Handcuffing"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdCuffTarget))]
    internal static class Handcuffing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            var offset = 0;

            // Search for the last "ldloc.2".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_2) + offset;

            // Get the first label of the last "ret".
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // var ev = new HandcuffingEventArgs(Player.Get(this.gameObject), Player.Get(target), true);
            //
            // Handlers.Player.OnHandcuffing(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // Player.Get(target)
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new HandcuffingEventArgs(...);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(HandcuffingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnHandcuffing(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHandcuffing))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HandcuffingEventArgs), nameof(HandcuffingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
