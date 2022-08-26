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

    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="BreakableWindow.Damage(float, PlayerStatsSystem.DamageHandlerBase, Vector3)" />.
    ///     Adds the <see cref="Handlers.Player.PlayerDamageWindow" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PlayerDamageWindow))]
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.Damage))]
    internal static class DamagingWindow
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = 0;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_0) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(DamagingWindowEventArgs));
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // damage
                    new(OpCodes.Ldarg_1),

                    // handler
                    new(OpCodes.Ldarg_2),

                    // ev = new(player, this, damage, handler);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingWindowEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnPlayerDamageWindow(ev);
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnPlayerDamageWindow))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingWindowEventArgs), nameof(DamagingWindowEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, retLabel),

                    // damage = ev.Handler.Damage;
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingWindowEventArgs), nameof(DamagingWindowEventArgs.Handler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CustomDamageHandler), nameof(CustomDamageHandler.Damage))),
                    new(OpCodes.Starg, 1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}