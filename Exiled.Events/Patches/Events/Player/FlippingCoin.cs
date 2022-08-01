// -----------------------------------------------------------------------
// <copyright file="FlippingCoin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Item;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Coin;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches
    ///     <see cref="CoinNetworkHandler.ServerProcessMessage(NetworkConnection, CoinNetworkHandler.CoinFlipMessage)" />.
    ///     Adds the <see cref="Handlers.Item.FlippingCoin" /> event.
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

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(ReferenceHub)
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // IsTails = Random.value >= 5
                new(OpCodes.Call, PropertyGetter(typeof(Random), nameof(Random.value))),
                new(OpCodes.Ldc_R4, 0.5f),
                new(OpCodes.Clt_Un),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ceq),

                new(OpCodes.Ldc_I4_1),

                // var ev = FlippingCoinEventArgs(Player, IsTails, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FlippingCoinEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // OnFlippingCoin(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnFlippingCoin))),

                // if (ev.IsAllowed)
                //   return
                new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // Item.SerialNumber
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))),
                new(OpCodes.Ldflda, Field(typeof(Inventory), nameof(Inventory.CurItem))),
                new(OpCodes.Ldfld, Field(typeof(ItemIdentifier), nameof(ItemIdentifier.SerialNumber))),

                // ev.IsTails
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsTails))),

                // new CoinFlipMessage(SerialNumber, IsTails)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CoinNetworkHandler.CoinFlipMessage))[0]),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldc_I4_0),

                // NetworkServer.SendToAll<CoinFlipMessage>(CoinFlipMessage, 0, false)
                new(OpCodes.Call, Method(typeof(NetworkServer), nameof(NetworkServer.SendToAll), null, new[] { typeof(CoinNetworkHandler.CoinFlipMessage) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
