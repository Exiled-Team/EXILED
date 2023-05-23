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
    using Exiled.Events.EventArgs.Player;
    using Footprinting;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="BanPlayer.BanUser(Footprint, ICommandSender, string, long)" />.
    ///     Adds the <see cref="Handlers.Player.Banning" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(Footprint), typeof(ICommandSender), typeof(string), typeof(long))]
    internal static class Banning
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -4;
            int index = newInstructions.FindIndex(x => x.Calls(Method(typeof(BanPlayer), nameof(BanPlayer.ApplyIpBan), new[] { typeof(Footprint), typeof(ICommandSender), typeof(string), typeof(long) }))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(BanningEventArgs));

            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(issuer);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(Footprint) })),

                    // Player target = Player.Get
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ICommandSender) })),

                    // duration
                    new(OpCodes.Ldarg_3),

                    // reason
                    new(OpCodes.Ldarg_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // BanningEventArgs ev = new(player, target, reason, duration, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BanningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnBanning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnBanning))),

                    // if (!ev.IsAllowed)
                    //      return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // loading ev 4 times
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // duration = ev.Duration;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Duration))),
                    new(OpCodes.Starg_S, 3),

                    // reason = ev.Reason;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Reason))),
                    new(OpCodes.Starg_S, 2),

                    // target = ev.Target.Footprint;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Target))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Footprint))),
                    new(OpCodes.Starg_S, 0),

                    // issuer = ev.Player.Sender;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Player))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Sender))),
                    new(OpCodes.Starg_S, 1),
                });

            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldstr);

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.FullMessage
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.FullMessage))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}