// -----------------------------------------------------------------------
// <copyright file="ChargingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1118
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp096.ChargePlayer"/>.
    /// Adds the <see cref="Handlers.Scp096.ChargingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ChargePlayer))]
    internal static class ChargingPlayer
    {
        /// <summary>
        /// The hashset of already charged players.
        /// Prevents double calling on the same player.
        /// </summary>
        public static readonly HashSet<ReferenceHub> ChargedPlayers = new HashSet<ReferenceHub>();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            // Search for "Stloc_0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            // Get the return label from the last instruction.
            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Create the damage labels.
            Label targetDamageLabel = generator.DefineLabel();
            Label nonTargetDamageLabel = generator.DefineLabel();

            // if (!ChargedPlayers.Add(ReferenceHub)
            //   return;
            //
            // var ev = new ChargingPlayerEventArgs(Scp096, Player player, Player victim, isTarget, isTarget ? 9696f, 35f, bool endCharge, true);
            //
            // Handlers.Scp096.OnChargingPlayer(ev);
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ChargingPlayer), nameof(ChargedPlayers))).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(ChargedPlayers.Add))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScps.PlayableScp), nameof(PlayableScps.PlayableScp.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Brtrue_S, targetDamageLabel),
                new CodeInstruction(OpCodes.Ldc_R4, 35f),
                new CodeInstruction(OpCodes.Br_S, nonTargetDamageLabel),
                new CodeInstruction(OpCodes.Ldc_R4, 9696f).WithLabels(targetDamageLabel),
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(nonTargetDamageLabel),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChargingPlayerEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnChargingPlayer))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChargingPlayerEventArgs), nameof(ChargingPlayerEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp096.EndCharge"/>.
    /// Serves to clear the ChargedPlayers.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.EndCharge))]
    internal static class ChargeEnded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // ChargePlayers.Clear();
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ChargingPlayer), nameof(ChargingPlayer.ChargedPlayers))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(ChargingPlayer.ChargedPlayers.Clear))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
