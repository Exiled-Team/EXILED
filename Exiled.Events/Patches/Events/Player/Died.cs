// -----------------------------------------------------------------------
// <copyright file="Died.cs" company="Exiled Team">
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

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject, bool, bool)"/>.
    /// Adds the <see cref="Handlers.Player.Died"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Died
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = -2;

            // Find the index of "playerStats.SetHpAmount" method and add the offset.
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
                                                                     (MethodInfo)instruction.operand == Method(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassID))) + offset;

            // Define the return label and add it to the starting instruction.
            Label returnLabel = generator.DefineLabel();
            newInstructions[index].WithLabels(returnLabel);

            // Used to identify the real attacker,
            // if `info.IsPlayer` is true, transfers control to the label `attacterPlayerGetAfterInstance`
            Label attackerInstanceVsHitInfoSource = generator.DefineLabel();
            Label attacterPlayerGetAfterInstance = generator.DefineLabel();

            // Declare the attacker local variable.
            LocalBuilder attacker = generator.DeclareLocal(typeof(Player));

            // Declare the target local variable.
            LocalBuilder target = generator.DeclareLocal(typeof(Player));

            // Player attacker = Player.Get(info.IsPlayer ? info.RHub.gameObject : __instance.gameObject);
            // Player target = Player.Get(go);
            //
            // if (target == null || attacker == null || target.IsGodModeEnabled)
            //  return;
            //
            // var ev = new DiedEventArgs(attacker, target, info, true);
            //
            // Handlers.Player.OnDied(ev);
            //
            // info = ev.HitInformations;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.IsPlayer))),
                new CodeInstruction(OpCodes.Brtrue_S, attackerInstanceVsHitInfoSource),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Br_S, attacterPlayerGetAfterInstance),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(attackerInstanceVsHitInfoSource),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.RHub))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })).WithLabels(attacterPlayerGetAfterInstance),
                new CodeInstruction(OpCodes.Stloc_S, attacker.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, target.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, attacker.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, target.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsGodModeEnabled))),
                new CodeInstruction(OpCodes.Brtrue_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, attacker.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, target.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DiedEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDied))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(DiedEventArgs), nameof(DiedEventArgs.HitInformations))),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
