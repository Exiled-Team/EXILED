// -----------------------------------------------------------------------
// <copyright file="Consuming.cs" company="Exiled Team">
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

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see>
    ///         <cref>RagdollAbilityBase{T}.ServerProcessCmd</cref>
    ///     </see>
    ///     .
    ///     Adds the <see cref="Handlers.Scp049.ConsumingCorpse" /> event.
    /// </summary>
    // TODO: REWORK TRANSPILER
    [HarmonyPatch]
    public class Consuming
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.ServerProcessCmd))]
        private static IEnumerable<CodeInstruction> OnConsumingCorpse(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int offset = -1;
            int index = newInstructions.FindLastIndex(instrc => instrc.LoadsField(Field(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>._errorCode)))) + offset;
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Call, Method(typeof(Consuming), nameof(Consuming.ServerProcessConsume))),
                new (OpCodes.Br, retLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        /// <summary>
        /// Processes RagDoll ability for the Zombie interaction.
        /// </summary>
        /// <param name="zombieAbility"> <see cref="ZombieRole"/> parameterized <see cref="RagdollAbilityBase{T}"/>. </param>
        private static void ServerProcessConsume(RagdollAbilityBase<ZombieRole> zombieAbility)
        {
            Transform transform = zombieAbility._ragdollTransform;

            Ragdoll currentRagDoll = Ragdoll.Get(zombieAbility.CurRagdoll);

            Player zombiePlayer = Player.Get(zombieAbility.Owner);

            ZombieConsumeAbility.ConsumeError errorCode = (ZombieConsumeAbility.ConsumeError)zombieAbility._errorCode;

            if (zombiePlayer.Role.Type != RoleTypeId.Scp049)
            {
                ConsumingCorpseEventArgs ev = new(zombiePlayer, currentRagDoll, errorCode);
                Handlers.Scp049.OnConsumingCorpse(ev);

                if (!ev.IsAllowed)
                    return;

                currentRagDoll = ev.Ragdoll;
                errorCode = ev.ErrorCode;
            }

            bool errorCodeFlag = errorCode != ZombieConsumeAbility.ConsumeError.None;

            if (errorCodeFlag)
            {
                zombieAbility._ragdollTransform = transform;
                zombieAbility.CurRagdoll = currentRagDoll.Base;
                zombieAbility.ServerSendRpc(true);
                return;
            }

            zombieAbility.IsInProgress = true;
        }
    }
}