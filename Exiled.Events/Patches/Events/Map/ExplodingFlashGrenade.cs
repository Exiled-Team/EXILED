// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.ServerFuseEnd()"/>.
    /// Adds the <see cref="Handlers.Map.ExplodingGrenade"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ExplodingGrenade))]
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    internal static class ExplodingFlashGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            int offset = 4;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Keyframe), nameof(Keyframe.time)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // FlashbangGrenade
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // Processes ExplodingGrenadeEventArgs
                    new(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ProcessEvent))),
                    new(OpCodes.Stloc_1),
                    new(OpCodes.Br_S, returnLabel),
                });

            newInstructions[newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ble_S) - 2].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static int ProcessEvent(FlashbangGrenade instance)
        {
            ExplodingGrenadeEventArgs explodingGrenadeEvent = new ExplodingGrenadeEventArgs(Player.Get(instance.PreviousOwner.Hub), instance);

            Handlers.Map.OnExplodingGrenade(explodingGrenadeEvent);

            if (!explodingGrenadeEvent.IsAllowed)
                return 0;

            int size = 0;
            foreach (Player player in explodingGrenadeEvent.TargetsToAffect)
            {
                if (!ExiledEvents.Instance.Config.CanFlashbangsAffectThrower && explodingGrenadeEvent.Player == player)
                    continue;

                if (HitboxIdentity.CheckFriendlyFire(explodingGrenadeEvent.Player.ReferenceHub, player.ReferenceHub))
                {
                    instance.ProcessPlayer(player.ReferenceHub);
                    size++;
                }
            }

            return size;
        }

        private static List<Player> ConvertHubs(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();
    }
}