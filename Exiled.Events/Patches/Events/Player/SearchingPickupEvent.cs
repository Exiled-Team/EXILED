// -----------------------------------------------------------------------
// <copyright file="SearchingPickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="SearchCoordinator.ReceiveRequestUnsafe" />.
    ///     Adds the <see cref="Handlers.Player.SearchingPickup" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SearchingPickup))]
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchingPickupEvent
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stind_Ref) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(SearchingPickupEventArgs));

            Label allowLabel = generator.DefineLabel();

            // remove base-game check and `SearchSession body` setter
            newInstructions.RemoveRange(index, 14);

            // SearchingPickupEventArgs ev = new(Player.Get(Hub), request.Target, request.Body, completor, request.Target.SearchTimeForPlayer(Hub));
            // Handlers.Player.OnSearchPickupRequest(ev);
            // if(!ev.IsAllowed) {
            //     SearchSession = null;
            //     completor = null;
            //     return true;
            // }
            // completor = ev.SearchCompletor;
            // body = ev.SearchSession;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),

                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Body))),

                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldind_Ref),

                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchRequest), nameof(SearchRequest.Target))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchCoordinator), nameof(SearchCoordinator.Hub))),
                    new(OpCodes.Callvirt, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.SearchTimeForPlayer))),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SearchingPickupEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSearchPickupRequest))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, allowLabel),

                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Initobj, typeof(SearchSession)),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Stind_Ref),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ret),

                    new CodeInstruction(OpCodes.Ldarg_2).WithLabels(allowLabel),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchCompletor))),
                    new(OpCodes.Stind_Ref),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchSession))),
                    new(OpCodes.Stloc_1),
                });

            offset = -5;
            index = newInstructions.FindIndex(
                i => (i.opcode == OpCodes.Stloc_S) &&
                     i.operand is LocalBuilder { LocalIndex: 4 }) + offset;

            // remove base-game SearchTime setter
            newInstructions.RemoveRange(index, 5);

            // SearchTime = ev.SearchTime;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SearchingPickupEventArgs), nameof(SearchingPickupEventArgs.SearchTime))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}