// -----------------------------------------------------------------------
// <copyright file="Kicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="BanPlayer.KickUser(ReferenceHub, ICommandSender , string)" />.
    ///     Adds the <see cref="Handlers.Player.Kicking" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Kicking))]
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ICommandSender), typeof(string))]
    internal static class Kicking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(KickingEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(ICommandSender);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ICommandSender) })),

                // Player.Get(ReferenceHub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // reason
                new(OpCodes.Ldarg_3),

                // kick start message
                new(OpCodes.Ldstr, newInstructions.Find(x => x.opcode == OpCodes.Ldstr).operand),

                // true
                new(OpCodes.Ldc_I4_1),

                // KickingEventArgs ev = new(Player, Player, string, string, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KickingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player.OnKicking(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnKicking))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(KickingEventArgs), nameof(KickingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Reason))),
                new(OpCodes.Starg_S, 3),

                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BanningEventArgs), nameof(BanningEventArgs.Target))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                new(OpCodes.Starg_S, 0),
            });

            newInstructions[newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}