// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Patches.Generic;
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using ExiledEvents = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="FlashbangGrenade.ServerFuseEnd()"/>.
    /// Adds the <see cref="Handlers.Map.ExplodingGrenade"/> event and <see cref="Config.CanFlashbangsAffectThrower"/>.
    /// </summary>
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    internal static class ExplodingFlashGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ignoreLabel = generator.DefineLabel();

            int offset = 4;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Keyframe), nameof(Keyframe.time)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // ExplodingFlashGrenade.ProcessEvent(this, num)
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(ExplodingFlashGrenade), nameof(ProcessEvent))),

                    // ignore the foreach since exiled overwrite it
                    new(OpCodes.Br_S, ignoreLabel),
                });

            newInstructions[newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ble_S) - 3].WithLabels(ignoreLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ProcessEvent(FlashbangGrenade instance, float distance)
        {
            List<Player> targetToAffect = ListPool<Player>.Pool.Get();
            foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
            {
                Player player = Player.Get(referenceHub);
                if ((instance.transform.position - referenceHub.transform.position).sqrMagnitude <= distance)
                    continue;
                if (!ExiledEvents.Instance.Config.CanFlashbangsAffectThrower && instance.PreviousOwner.SameLife(new(referenceHub)))
                    continue;
                if (!IndividualFriendlyFire.CheckFriendlyFirePlayer(instance.PreviousOwner, player.ReferenceHub))
                    continue;
                if (!Physics.Linecast(instance.transform.position, player.Position, instance._blindingMask))
                    continue;

                targetToAffect.Add(Player.Get(referenceHub));
            }

            ExplodingGrenadeEventArgs explodingGrenadeEvent = new(Player.Get(instance.PreviousOwner.Hub), instance, targetToAffect);

            ListPool<Player>.Pool.Return(targetToAffect);

            Handlers.Map.OnExplodingGrenade(explodingGrenadeEvent);

            if (!explodingGrenadeEvent.IsAllowed)
                return;

            foreach (Player player in explodingGrenadeEvent.TargetsToAffect)
            {
                instance.ProcessPlayer(player.ReferenceHub);
            }
        }
    }
}