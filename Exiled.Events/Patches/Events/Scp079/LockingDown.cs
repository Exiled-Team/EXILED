// -----------------------------------------------------------------------
// <copyright file="LockingDown.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp079;
    using PlayerRoles.PlayableScps.Scp079.Cameras;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp079LockdownRoomAbility.ServerProcessCmd(NetworkReader)" />.
    /// Adds the <see cref="Scp079.LockingDown" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Scp079), nameof(Scp079.LockingDown))]
    [HarmonyPatch(typeof(Scp079LockdownRoomAbility), nameof(Scp079LockdownRoomAbility.ServerProcessCmd))]
    internal static class LockingDown
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(LockingDownEventArgs));

            int offset = -6;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            // LockingDownEventArgs ev = new(Player.Get(base.Owner), base.CurrentCamSync.CurrentCamera.Room, (float)this._cost);
            //
            // Handlers.Scp079.OnLockingDown(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(StandardSubroutine<Scp079Role>), nameof(StandardSubroutine<Scp079Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // base.CurrentCamSync.CurrentCamera.Room
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(Scp079AbilityBase), nameof(Scp079AbilityBase.CurrentCamSync))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync.CurrentCamera))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079Camera), nameof(Scp079Camera.Room))),

                    // (float)this._cost
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079LockdownRoomAbility), nameof(Scp079LockdownRoomAbility._cost))),
                    new(OpCodes.Conv_R4),

                    // LockingDownEventArgs ev = new(Player, RoomIdentifier, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LockingDownEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnLockingDown(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnLockingDown))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LockingDownEventArgs), nameof(LockingDownEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            // Replace "(float)this._cost" with "ev.AuxiliaryPowerCost"
            offset = -1;
            index = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(Scp079LockdownRoomAbility), nameof(Scp079LockdownRoomAbility._cost)))) + offset;

            newInstructions.RemoveRange(index, 3);

            newInstructions.InsertRange(
               index,
               new CodeInstruction[]
               {
                    // ev.AuxiliaryPowerCost
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LockingDownEventArgs), nameof(LockingDownEventArgs.AuxiliaryPowerCost))),
               });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}