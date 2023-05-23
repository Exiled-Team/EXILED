// -----------------------------------------------------------------------
// <copyright file="SendingCall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049CallAbility.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp049.SendingCall" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049CallAbility), nameof(Scp049CallAbility.ServerProcessCmd))]
    public class SendingCall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label evLabel = generator.DefineLabel();

            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));
            LocalBuilder ev = generator.DeclareLocal(typeof(SendingCallEventArgs));

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp049CallAbility), nameof(Scp049CallAbility._serverTriggered))),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, isAllowed.LocalIndex),
                    new(OpCodes.Brtrue_S, evLabel),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp049CallAbility), nameof(Scp049CallAbility.Cooldown))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AbilityCooldown), nameof(AbilityCooldown.IsReady))),
                    new(OpCodes.Stloc_S, isAllowed.LocalIndex),

                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(evLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp049CallAbility), nameof(Scp049CallAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldc_R4, 20f),

                    new(OpCodes.Ldloc_S, isAllowed.LocalIndex),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SendingCallEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S),

                    new(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnSendingCall))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCallEventArgs), nameof(SendingCallEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, returnLabel),

                    new(OpCodes.Ret),
                });

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldc_R4);

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SendingCallEventArgs), nameof(SendingCallEventArgs.Duration))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        /// <summary>
        /// Process's Scp049 call ability.
        /// </summary>
        /// <param name="callAbility"> <see cref="Scp049CallAbility"/>. </param>
        private static void ProcessCall(Scp049CallAbility callAbility)
        {
            Player currentScp = Player.Get(callAbility.Owner);
            float duration = Scp049CallAbility.EffectDuration;

            var ev = new SendingCallEventArgs(currentScp, duration, !callAbility._serverTriggered && callAbility.Cooldown.IsReady);
            Handlers.Scp049.OnSendingCall(ev);

            if (!ev.IsAllowed)
                return;
            callAbility.Duration.Trigger(ev.Duration);
            callAbility._serverTriggered = true;
            callAbility.ServerSendRpc(true);
        }
    }
}
