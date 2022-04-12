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

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Patches <see cref="Scp096.ChargePlayer"/>.
    /// Adds the <see cref="Handlers.Scp096.ChargingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(PlayableScps.Scp096.ChargePlayer))]
    internal static class ChargingPlayer
    {
        /// <summary>
        /// The hashset of already charged players.
        /// Prevents double calling on the same player.
        /// </summary>
        public static readonly HashSet<ReferenceHub> ChargedPlayers = new();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            // Search for "Stloc_0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            // Get the return label from the last instruction.
            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Declare a local variable of type ChargingPlayerEventArgs.
            LocalBuilder ev = generator.DeclareLocal(typeof(ChargingPlayerEventArgs));
            LocalBuilder contains = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(index, new[]
            {
                // if (!ChargedPlayers.Add(target))
                //    return;
                new(OpCodes.Stloc, contains.LocalIndex),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(ChargingPlayer), nameof(ChargedPlayers))).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(ChargedPlayers.Add))),
                new(OpCodes.Brfalse_S, returnLabel),

                // var ev = new ChargingPlayerEventArgs(this, Player.Get(this.Hub), target, this._targets.Contains(target), damage)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.PlayableScp), nameof(PlayableScps.PlayableScp.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096._targets))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Contains))),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChargingPlayerEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp096.OnChargingPlayer(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnChargingPlayer))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChargingPlayerEventArgs), nameof(ChargingPlayerEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // damage = ev.Damage
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChargingPlayerEventArgs), nameof(ChargingPlayerEventArgs.Damage))),
                new(OpCodes.Stloc_0),
                new(OpCodes.Ldloc, contains.LocalIndex),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp096.EndCharge"/>.
    /// Serves to clear the ChargedPlayers.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(PlayableScps.Scp096.EndCharge))]
    internal static class ChargeEnded
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // ChargePlayers.Clear();
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, Field(typeof(ChargingPlayer), nameof(ChargingPlayer.ChargedPlayers))),
                new(OpCodes.Callvirt, Method(typeof(HashSet<ReferenceHub>), nameof(ChargingPlayer.ChargedPlayers.Clear))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
