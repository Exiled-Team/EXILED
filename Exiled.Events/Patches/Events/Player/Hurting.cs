// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using API.Features.Roles;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="PlayerStats.DealDamage(DamageHandlerBase)" />.
    ///     Adds the <see cref="Handlers.Player.Hurting" /> event and <see cref="Handlers.Player.Hurt" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Hurting))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Hurt))]
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.DealDamage))]
    internal static class Hurting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder ev = generator.DeclareLocal(typeof(HurtingEventArgs));

            Label notRecontainment = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(this._hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(PlayerStats), nameof(PlayerStats._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Stloc, player.LocalIndex),

                    // if (handler is RecontainmentDamageHandler)
                    // {
                    //    if (player.Role == RoleTypeId.Scp079)
                    //    {
                    //        Handlers.Scp079.OnRecontained(new RecontainedEventArgs(player));
                    //        return;
                    //    }
                    // }
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Isinst, typeof(RecontainmentDamageHandler)),
                    new(OpCodes.Brfalse, notRecontainment),

                    new(OpCodes.Ldloc, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Role), nameof(Role.Type))),
                    new(OpCodes.Ldc_I4_7),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse, notRecontainment),

                    new(OpCodes.Ldloc, player.LocalIndex),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RecontainedEventArgs))[0]),
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnRecontained))),

                    // HurtingEventArgs ev = new(player, handler)
                    new CodeInstruction(OpCodes.Ldloc, player.LocalIndex).WithLabels(notRecontainment),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(HurtingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnHurting(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHurting))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, ret),
                });

            offset = 2;
            index = newInstructions.FindIndex(instruction => instruction.operand == (object)Method(typeof(DamageHandlerBase), nameof(DamageHandlerBase.ApplyDamage))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // HurtingEventArgs ev = new(player, handler, handleroutput)
                    new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(HurtEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnHurting(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHurt))),

                    // handlerOutput = ev.HandlerOutput;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(HurtEventArgs), nameof(HurtEventArgs.HandlerOutput))),
                    new(OpCodes.Stloc_1),
                });
            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}