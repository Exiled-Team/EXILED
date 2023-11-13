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
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079BlackoutZoneAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp079.ZoneBlackout" /> event for  SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.ZoneBlackout))]
    [HarmonyPatch(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ServerProcessCmd))]
    internal static class ZoneBlackout
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ZoneBlackoutEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Brfalse_S) + offset;

            // remove "this.ErrorCode"
            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this.Owner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScpStandardSubroutine<Scp079Role>), nameof(ScpStandardSubroutine<Scp079Role>.Owner))),

                    // this._syncZone
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone))),

                    // (float)this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cost))),
                    new(OpCodes.Conv_R4),

                    // this._duration
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._duration))),

                    // this._cooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cooldown))),

                    // this.ErrorCode
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility.ErrorCode))),

                    // ZoneBlackoutEventArgs ev = new(ReferenceHub, FacilityZone, float, float, float, Scp079HudTranslation)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ZoneBlackoutEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnZoneBlackout(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnZoneBlackout))),

                    // !ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.IsAllowed))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // if (!ev.IsAllowed) return;
                });

            offset = 1;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this._syncZone = ev.Zone.GetZone()
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.Zone))),
                    new(OpCodes.Call, Method(typeof(RoomExtensions), nameof(RoomExtensions.GetZone), new[] { typeof(API.Enums.ZoneType), })),
                    new(OpCodes.Stfld, Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._syncZone))),
                });

            // replace "roomLightController.ServerFlickerLights(this._duration);"
            // with
            // "roomLightController.ServerFlickerLights(ev.Duration);"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._duration))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.BlackoutDuration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.BlackoutDuration))),
                });

            // replace "this._cooldownTimer.Trigger((double)this._cooldown);"
            // with
            // "this._cooldownTimer.Trigger((double)ev._cooldown);"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cooldown))) + offset;

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.Cooldown
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.Cooldown))),
                });

            // replace "base.AuxManager.CurrentAux -= (float)this._cost;"
            // with
            // "base.AuxManager.CurrentAux -= ev.AuxiliaryPowerCost;"
            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)Field(typeof(Scp079BlackoutZoneAbility), nameof(Scp079BlackoutZoneAbility._cost))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ZoneBlackoutEventArgs), nameof(ZoneBlackoutEventArgs.AuxiliaryPowerCost))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}