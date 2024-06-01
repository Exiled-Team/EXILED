// -----------------------------------------------------------------------
// <copyright file="Banning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using CommandSystem;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Footprinting;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="BanPlayer.BanUser(Footprint, ICommandSender, string, long)" />.
    /// Adds the <see cref="Handlers.Player.Banning" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Banning))]
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(Footprint), typeof(ICommandSender), typeof(string), typeof(long))]
    internal static class Banning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label notEmpty = generator.DefineLabel();
            Label empty = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(BanningEventArgs));
            LocalBuilder msg = generator.DeclareLocal(typeof(string));

            int offset = 1;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Starg_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(Footprint) })),

                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ICommandSender) })),

                    new(OpCodes.Ldarg_3),

                    new(OpCodes.Ldarg_2),

                    new(OpCodes.Ldstr, "You have been banned. "),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                    new(OpCodes.Brfalse_S, notEmpty),
                    new(OpCodes.Ldsfld, Field(typeof(string), nameof(string.Empty))),
                    new(OpCodes.Br_S, empty),
                    new CodeInstruction(OpCodes.Ldstr, "Reason: ").WithLabels(notEmpty),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                    new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })).WithLabels(empty),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BanningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnBanning))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Reason))),
                    new(OpCodes.Starg_S, 2),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Duration))),
                    new(OpCodes.Starg_S, 3),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Player))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Sender))),
                    new(OpCodes.Starg_S, 1),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Target))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Footprint))),
                    new(OpCodes.Starg_S, 0),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.FullMessage))),
                    new(OpCodes.Stloc_S, msg.LocalIndex),
                });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldstr);

            newInstructions.RemoveRange(index, 3);

            newInstructions.Insert(index, new(OpCodes.Ldloc_S, msg.LocalIndex));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}