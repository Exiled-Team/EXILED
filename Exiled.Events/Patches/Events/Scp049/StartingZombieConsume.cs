// -----------------------------------------------------------------------
// <copyright file="StartingZombieConsume.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp049;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp049ResurrectAbility.ServerComplete" />.
    ///     Adds the <see cref="Handlers.Scp049.FinishingRecall" /> event.
    /// </summary>
    [HarmonyPatch]
    public class StartingZombieConsume
    {
        [HarmonyPatch(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.ServerProcessCmd))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>._errorCode)))) + offset;
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // RagdollAbilityBase<ZombieRole>
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(StartingZombieConsume), nameof(ServerProcessCmdRewrite))),
                    new(OpCodes.Br, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Processes Ragdoll ability for ZombieRole/049 interaction.
        /// </summary>
        /// <param name="zombieAbilityBase"> <see cref="ZombieRole"/> parameterized <see cref="RagdollAbilityBase{T}"/>. </param>
        private static void ServerProcessCmdRewrite(RagdollAbilityBase<ZombieRole> zombieAbilityBase)
        {
            Transform ragdollTransform = zombieAbilityBase._ragdollTransform;
            BasicRagdoll curRagdoll = zombieAbilityBase.CurRagdoll;

            API.Features.Player currentPlayer = API.Features.Player.Get(zombieAbilityBase.Owner);
            if (currentPlayer.Role.Type is not RoleTypeId.Scp049)
            {
                ConsumingBodyEventArgs ev = new ConsumingBodyEventArgs(currentPlayer, curRagdoll, zombieAbilityBase._errorCode);
                Handlers.Scp049.OnStartingConsume(ev);

                if (!ev.IsAllowed)
                {
                    return;
                }

                curRagdoll = ev.TargetRagdoll;
                zombieAbilityBase._errorCode = ev.ErrorCode;
            }

            bool flag = zombieAbilityBase._errorCode > 0;
            if (flag)
            {
                zombieAbilityBase._ragdollTransform = ragdollTransform;
                zombieAbilityBase.CurRagdoll = curRagdoll;
                zombieAbilityBase.ServerSendRpc(true);
                return;
            }

            zombieAbilityBase.IsInProgress = true;
        }
    }
}