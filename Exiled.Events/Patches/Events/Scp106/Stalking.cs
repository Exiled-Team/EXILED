// -----------------------------------------------------------------------
// <copyright file="Stalking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106StalkAbility.ServerProcessCmd"/>.
    /// To add the <see cref="Handlers.Scp106.Stalking"/> event.
    /// </summary>
    // TODO: REWORK TRANSPILER
    [EventPatch(typeof(Handlers.Scp106), nameof(Handlers.Scp106.Stalking))]
    [HarmonyPatch]
    public class Stalking
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerProcessCmd))]
        private static IEnumerable<CodeInstruction> OnStalking(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Stalking), nameof(ServerProcessStalk))),
                new(OpCodes.Br, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        /// <summary>
        /// Process the stalk ability and call the event.
        /// </summary>
        /// <param name="stalkAbility">106's <see cref="Scp106StalkAbility"/> ability.</param>
        private static void ServerProcessStalk(Scp106StalkAbility stalkAbility)
        {
            if (stalkAbility._sinkhole.IsDuringAnimation || !stalkAbility._sinkhole.Cooldown.IsReady ||
                !stalkAbility.CastRole.FpcModule.IsGrounded)
                return;

            Player scp106 = Player.Get(stalkAbility.Owner);
            StalkingEventArgs ev = new(scp106, stalkAbility);
            Handlers.Scp106.OnStalking(ev);

            bool flag = ev.Vigor < ev.MinimumVigor;

            if (!ev.IsAllowed)
                return;

            if (stalkAbility.IsActive)
            {
                stalkAbility.IsActive = false;
            }
            else if (flag)
            {
                if (stalkAbility.Role.IsLocalPlayer)
                    Scp106Hud.PlayFlash(true);
                stalkAbility.ServerSendRpc(false);
            }
            else
            {
                stalkAbility.IsActive = true;
            }
        }
    }
}