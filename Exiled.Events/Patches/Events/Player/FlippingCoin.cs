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

    using API.Features;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Coin;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches
    ///     <see cref="CoinNetworkHandler.ServerProcessMessage(NetworkConnection, CoinNetworkHandler.CoinFlipMessage)" />.
    ///     Adds the <see cref="Handlers.Player.FlippingCoin" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CoinNetworkHandler), nameof(CoinNetworkHandler.ServerProcessMessage))]
    internal static class FlippingCoin
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(FlippingCoinEventArgs));

            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S) + offset;

            // Extract the old labels, before removing it
            List<Label> oldLabels = newInstructions[index].ExtractLabels();

            // Remove both NW events and logic
            newInstructions.RemoveRange(index, 45);

            // Redirect the old labels to the new starting point
            newInstructions[index].WithLabels(oldLabels);

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(Coin), nameof(Coin.RateLimiter)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ReferenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // FlippingCoinEventArgs ev = new(Player, bool, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FlippingCoinEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // OnFlippingCoin(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFlippingCoin))),

                    // if (!ev.IsAllowed)
                    //   return
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // flag = ev.IsTails
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsTails))),
                    new(OpCodes.Stloc_3),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}