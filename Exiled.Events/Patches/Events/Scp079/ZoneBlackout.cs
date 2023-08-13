// -----------------------------------------------------------------------
// <copyright file="ZoneBlackout.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079BlackoutZoneAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp079.ZoneBlackout" /> event for  SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.Pinging))]
    [HarmonyPatch(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ServerProcessCmd))]
    internal static class ZoneBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 3;
            int index = newInstructions.FindIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Remove NW answer
                    new(OpCodes.Pop),

                    // Pass Scp079BlackoutZoneAbility instance
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(ZoneBlackout), nameof(ProcessZoneBlackout))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ProcessZoneBlackout(Scp079BlackoutZoneAbility instance)
        {
            ZoneBlackoutEventArgs ev = new(instance.Owner, instance._syncZone, instance._cost, instance._duration, instance._cooldown, instance.ErrorCode);

            Handlers.Scp079.OnZoneBlackout(ev);

            if (ev.IsAllowed)
            {
                instance._duration = ev.BlackoutDuration;
                instance._cooldown = ev.Cooldown;

                // Gets casted to float above, even though it is an int, joy.
                instance._cost = (int)ev.AuxiliaryPowerCost;
            }

            return ev.IsAllowed;
        }
    }
}