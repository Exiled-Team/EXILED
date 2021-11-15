// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject, bool, bool)"/>.
    /// Adds the <see cref="Handlers.Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Hurting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(ArtificialHealthManager), nameof(ArtificialHealthManager.GetAhpValue))) + offset;

            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(Player));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(Player));
            LocalBuilder mem_0x03 = generator.DeclareLocal(typeof(float));
            LocalBuilder ev_0x01 = generator.DeclareLocal(typeof(HurtingEventArgs));
            LocalBuilder ev_0x02 = generator.DeclareLocal(typeof(DyingEventArgs));
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(int));
            LocalBuilder cmp_0x02 = generator.DeclareLocal(typeof(int));
            LocalBuilder cmp_0x03 = generator.DeclareLocal(typeof(int));
            LocalBuilder cmp_0x04 = generator.DeclareLocal(typeof(int));

            Label ret = generator.DefineLabel();
            Label cdc = generator.DefineLabel();
            Label cmova = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label into = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.IsPlayer))),
                new CodeInstruction(OpCodes.Brtrue_S, cmova),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerStats), nameof(PlayerStats.gameObject))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldarga_S, 1).WithLabels(cmova),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.RHub))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_2).WithLabels(jmp),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsHost))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.Tool))),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(DamageTypes), nameof(DamageTypes.Recontainment))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, jcc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Role))),
                new CodeInstruction(OpCodes.Ldc_I4_7),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, jcc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RecontainedEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnRecontained))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DiedEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDied))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex).WithLabels(jcc),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsHost))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(HurtingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsHost))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHurting))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.HitInformation))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsGodModeEnabled))),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Amount))),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Ldc_I4_M1),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Health))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ArtificialHealth))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Amount))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Amount))),
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Attacker))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.Target))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HurtingEventArgs), nameof(HurtingEventArgs.HitInformation))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DyingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDying))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.ItemsToDrop))),
                new CodeInstruction(OpCodes.Brfalse_S, into),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.Target))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.ItemsToDrop))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.ResetInventory), new[] { typeof(List<API.Features.Items.Item>) })),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.DropItems))),
                new CodeInstruction(OpCodes.Ldloc_S, ev_0x02.LocalIndex).WithLabels(into),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.Target))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Inventory))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Call, Method(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus))),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(DisarmedPlayers), nameof(DisarmedPlayers.Entries))),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DisarmedPlayersListMessage))[0]),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(NetworkUtils), nameof(NetworkUtils.SendToAuthenticated)).MakeGenericMethod(typeof(DisarmedPlayersListMessage))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
