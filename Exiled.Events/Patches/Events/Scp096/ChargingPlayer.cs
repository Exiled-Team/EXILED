// -----------------------------------------------------------------------
// <copyright file="ChargingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
#pragma warning disable SA1118
#pragma warning disable SA1313
#pragma warning disable SA1402
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using PlayableScps;
    using PlayableScps.Messages;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp096.ChargePlayer"/>.
    /// Adds the <see cref="Handlers.Scp096.ChargingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.ChargePlayer))]
    internal static class ChargingPlayer
    {
        /// <summary>
        /// The hashset of already charged players.
        /// Prevents double calling on the same player.
        /// </summary>
        public static readonly HashSet<ReferenceHub> ChargedPlayers = new HashSet<ReferenceHub>();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            // Search for last "lfsfld".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            // Get the count to find the previous index
            var oldCount = newInstructions.Count;

            // Get the return label from the last instruction.
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Create the damage labels.
            var damageLabel1 = generator.DefineLabel();
            var damageLabel2 = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ChargingPlayer), nameof(ChargedPlayers))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(ChargedPlayers.Add))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScp), nameof(PlayableScp.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Brtrue_S, damageLabel1),
                new CodeInstruction(OpCodes.Ldc_R4, 35f),
                new CodeInstruction(OpCodes.Br_S, damageLabel2),
                new CodeInstruction(OpCodes.Ldc_R4, 9696f).WithLabels(damageLabel1),
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(damageLabel2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChargingPlayerEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnChargingPlayer))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChargingPlayerEventArgs), nameof(ChargingPlayerEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp096.EndCharge"/>.
    /// Serves to clear the ChargedPlayers.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndCharge))]
    internal static class ChargeEnded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

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
