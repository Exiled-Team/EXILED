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
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079BlackoutRoomAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp079.RoomBlackout" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.RoomBlackout))]
    [HarmonyPatch(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility.ServerProcessCmd))]
    internal static class RoomBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel;
            Label allowedJump = generator.DefineLabel();
            Label jump = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(RoomBlackoutEventArgs));

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Brfalse);

            returnLabel = (Label)newInstructions[index].operand;

            int offset = -2;
            index += offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),

                    // this._roomController.Room
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._roomController))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomLightController), nameof(RoomLightController.Room))),

                    // (float)this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cost))),
                    new(OpCodes.Conv_R4),

                    // this._duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._blackoutDuration))),

                    // this._cooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cooldown))),

                    // (!this.IsReady || !base.LostSignalHandler.Lost)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility.IsReady))),
                    new(OpCodes.Brfalse_S, allowedJump),

                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility.LostSignalHandler))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Lost))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new(OpCodes.Br, jump),
                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(allowedJump),

                    // RoomBlackoutEventArgs ev = new(ReferenceHub, RoomIdentifier, float, float, float, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RoomBlackoutEventArgs))[0]).WithLabels(jump),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnRoomBlackout(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnRoomBlackout))),

                    // if (ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // replace "base.AuxManager.CurrentAux -= (float)this._cost;"
            // with
            // "base.AuxManager.CurrentAux -= ev.AuxiliaryPowerCost;"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cost))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.AuxiliaryPowerCost))),
                });

            // replace "this._blackoutCooldowns[this._roomController.netId] = NetworkTime.time + (double)this._cooldown;"
            // with
            // "this._blackoutCooldowns[this._roomController.netId] = NetworkTime.time + ev.Cooldown;"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._cooldown))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.Cooldown
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.Cooldown))),
                });

            // replace "this._roomController.ServerFlickerLights(this._blackoutDuration);"
            // with
            // "this._roomController.ServerFlickerLights(ev.BlackoutDuration);"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutRoomAbility), nameof(Scp079BlackoutRoomAbility._blackoutDuration))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.BlackoutDuration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RoomBlackoutEventArgs), nameof(RoomBlackoutEventArgs.BlackoutDuration))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}