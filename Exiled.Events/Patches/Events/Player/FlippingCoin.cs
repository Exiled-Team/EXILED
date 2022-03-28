// -----------------------------------------------------------------------
// <copyright file="FlippingCoin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Coin;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CoinNetworkHandler.ServerProcessMessage(NetworkConnection, CoinNetworkHandler.CoinFlipMessage)"/>.
    /// Adds the <see cref="Handlers.Player.FlippingCoin"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CoinNetworkHandler), nameof(CoinNetworkHandler.ServerProcessMessage))]
    internal static class FlippingCoin
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(FlippingCoinEventArgs));

            const int instructionsToRemove = 12;
            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;

            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ReferenceHub)
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // IsTails = Random.value >= 5
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Random), nameof(Random.value))),
                new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                new CodeInstruction(OpCodes.Clt_Un),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),

                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = FlippingCoinEventArgs(Player, IsTails, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FlippingCoinEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // OnFlippingCoin(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFlippingCoin))),

                // if (ev.IsAllowed)
                //   return
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // Item.SerialNumber
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.CurItem))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Items.ItemIdentifier), nameof(InventorySystem.Items.ItemIdentifier.SerialNumber))),

                // ev.IsTails
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsTails))),

                // new CoinFlipMessage(SerialNumber, IsTails)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(CoinNetworkHandler.CoinFlipMessage))[0]),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4_0),

                // NetworkServer.SendToAll<CoinFlipMessage>(CoinFlipMessage, 0, false)
                new CodeInstruction(OpCodes.Call, Method(typeof(NetworkServer), nameof(NetworkServer.SendToAll), null, new[] { typeof(CoinNetworkHandler.CoinFlipMessage) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
