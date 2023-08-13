// -----------------------------------------------------------------------
// <copyright file="RoomBlackout.cs" company="Exiled Team">
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
    ///     Patches <see cref="Scp079BlackoutRoomAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp079.RoomBlackout" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.Pinging))]
    [HarmonyPatch(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility.ServerProcessCmd))]
    internal static class RoomBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(RoomBlackoutEventArgs));

            int offset = -1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(PropertyGetter(typeof(Scp079AbilityBase), nameof(Scp079AbilityBase.LostSignalHandler)))) + offset;
            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(RoomBlackout), nameof(ProcessRoomBlackout))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ProcessRoomBlackout(Scp079BlackoutRoomAbility instance)
        {
            RoomBlackoutEventArgs ev = new(instance.Owner, instance._roomController.Room, instance._cost, instance._blackoutDuration, instance._cooldown, !instance.LostSignalHandler.Lost);

            Handlers.Scp079.OnRoomBlackout(ev);

            if (ev.IsAllowed)
            {
                instance._blackoutDuration = ev.BlackoutDuration;
                instance._cooldown = (float)ev.Cooldown;

                // Gets casted to float above, even though it is an int, joy.
                instance._cost = (int)ev.AuxiliaryPowerCost;
            }

            return ev.IsAllowed;
        }
    }
}