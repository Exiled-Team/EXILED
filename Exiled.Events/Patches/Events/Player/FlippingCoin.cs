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
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Coin;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches
    /// <see cref="Coin.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Handlers.Player.FlippingCoin" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.FlippingCoin))]
    [HarmonyPatch(typeof(Coin), nameof(Coin.ServerProcessCmd))]
    internal static class FlippingCoin
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(FlippingCoinEventArgs));

            int offset = -5;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ReferenceHub)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // isTails
                    new(OpCodes.Ldloc_1),

                    // FlippingCoinEventArgs ev = new(Player, bool)
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

                    // isTails = ev.IsTails
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FlippingCoinEventArgs), nameof(FlippingCoinEventArgs.IsTails))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
