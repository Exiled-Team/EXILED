// -----------------------------------------------------------------------
// <copyright file="ChangingCamera.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;
    using PlayerRoles.PlayableScps.Scp079.Cameras;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp079CurrentCameraSync.ServerProcessCmd(NetworkReader)" />.
    ///     Adds the <see cref="ChangingCamera" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079CurrentCameraSync), nameof(Scp079CurrentCameraSync.ServerProcessCmd))]
    internal static class ChangingCamera
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Conv_R4) + offset;

            // Define the first label of the last "ret" and retrieve it.
            Label returnLabel = newInstructions[newInstructions.Count - 1].WithLabels(generator.DefineLabel()).labels[0];

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingCameraEventArgs));

            // ChangingCameraEventArgs ev = new(Player.Get(this.gameObject), camera, num)
            //
            // Handlers.Scp079.OnChangingCamera(ev)
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.AuxiliaryPowerCost
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // num (auxiliary power cost)
                    new(OpCodes.Ldloca_S, 0),

                    new(OpCodes.Call, Method(typeof(ChangingCamera), nameof(ChangingCamera.ChangingCameraEvent))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool ChangingCameraEvent(Scp079CurrentCameraSync instance, ref float cost)
        {
            ChangingCameraEventArgs ev = new ChangingCameraEventArgs(Player.Get(instance.Owner), instance._switchTarget, cost);

            Scp079.OnChangingCamera(ev);

            instance.ServerSendRpc(true);

            if (ev.IsAllowed)
                cost = ev.AuxiliaryPowerCost;

            return ev.IsAllowed;
        }
    }
}