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
    using Exiled.Events.EventArgs.Scp106;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp106;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp106StalkAbility.ServerProcessCmd"/>.
    /// To add the <see cref="Handlers.Scp106.Stalking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerProcessCmd))]
    public class Stalking
    {
        private static IEnumerable<CodeInstruction> OnStalking(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(StalkingEventArgs));

            int offset = 3;
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Callvirt, PropertyGetter(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.IsGrounded)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldarg_0),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StalkingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Scp106), nameof(Handlers.Scp106.OnStalking))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(StalkingEventArgs), nameof(StalkingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),
                });

            offset = -3;
            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldc_R4) + offset;

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StalkingEventArgs), nameof(StalkingEventArgs.Vigor))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StalkingEventArgs), nameof(StalkingEventArgs.MinimumVigor))),
                });

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
                !stalkAbility.ScpRole.FpcModule.IsGrounded)
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