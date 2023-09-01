// -----------------------------------------------------------------------
// <copyright file="Pinging.cs" company="Exiled Team">
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
    using Mirror;
    using PlayerRoles.PlayableScps.Scp079.Pinging;
    using RelativePositioning;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079PingAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp079.Pinging" /> event for  SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.Pinging))]
    [HarmonyPatch(typeof(Scp079PingAbility), nameof(Scp079PingAbility.ServerProcessCmd))]
    internal static class Pinging
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int offset = -2;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(RelativePositionSerialization), nameof(RelativePositionSerialization.ReadRelativePosition)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Load Scp079PingAbility , NetworkReader into ProcessPinging
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Pinging), nameof(Pinging.ProcessPinging))),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void ProcessPinging(Scp079PingAbility instance, NetworkReader reader)
        {
            RelativePosition curRelativePos = reader.ReadRelativePosition();
            Vector3 syncNormal = reader.ReadVector3();
            PingingEventArgs ev = new(instance.Owner, curRelativePos, instance._cost, instance._syncProcessorIndex, syncNormal);

            Handlers.Scp079.OnPinging(ev);

            if (ev.IsAllowed)
            {
                instance._syncNormal = ev.SyncNormal;
                instance._syncPos = curRelativePos;
                instance.ServerSendRpc(hub => instance.ServerCheckReceiver(hub, ev.Position, (int)ev.Type));
                instance.AuxManager.CurrentAux -= ev.AuxiliaryPowerCost;
                instance._rateLimiter.RegisterInput();
            }
        }
    }
}