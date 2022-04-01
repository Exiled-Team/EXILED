// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="PlayerStats.DealDamage(DamageHandlerBase)"/>.
    /// Adds the <see cref="Handlers.Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
    internal static class Hurting {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder hurtingEv = generator.DeclareLocal(typeof(HurtingEventArgs));

            Label notRecontainment = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player = Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerStats), nameof(PlayerStats._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Stloc, player.LocalIndex),

                // if (handler is RecontainmentDamageHandler)
                // {
                //    if (player.Role == RoleType.Scp079)
                //    {
                //        Handlers.Scp079.OnRecontained(new RecontainedEventArgs(player));
                //        return;
                //    }
                // }
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Isinst, typeof(RecontainmentDamageHandler)),
                new CodeInstruction(OpCodes.Brfalse, notRecontainment),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new CodeInstruction(OpCodes.Ldc_I4_7),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, notRecontainment),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RecontainedEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnRecontained))),

                // var ev = new HurtingEventArgs(player, handler)
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex).WithLabels(notRecontainment),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(HurtingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, hurtingEv.LocalIndex),

                // Handlers.Player.OnHurting(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHurting))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
