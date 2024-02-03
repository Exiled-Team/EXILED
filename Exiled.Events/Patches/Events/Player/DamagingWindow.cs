// -----------------------------------------------------------------------
// <copyright file="DamagingWindow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.DamageHandlers;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="BreakableWindow.Damage(float, PlayerStatsSystem.DamageHandlerBase, Vector3)" />.
    /// Adds the <see cref="Player.PlayerDamageWindow" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.PlayerDamageWindow))]
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.Damage))]
    internal static class DamagingWindow
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(DamagingWindowEventArgs));

            Label ret = generator.DefineLabel();

            int offset = -5;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // damage
                    new(OpCodes.Ldarg_1),

                    // handler
                    new(OpCodes.Ldarg_2),

                    // DamagingWindowEventArgs ev = new(player, this, damage, handler);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingWindowEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnPlayerDamageWindow(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnPlayerDamageWindow))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingWindowEventArgs), nameof(DamagingWindowEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, ret),

                    // damage = ev.Handler.Damage;
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingWindowEventArgs), nameof(DamagingWindowEventArgs.Handler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CustomDamageHandler), nameof(CustomDamageHandler.Damage))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}