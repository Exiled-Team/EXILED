using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Scp049;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.PlayableScps.Subroutines;
using PlayerStatsSystem;
using PluginAPI.Core;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace Exiled.Events.Patches.Events.Scp049
{
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
            //Find IsCorpse, then going backwards, find IsInProgress
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.IsCorpseNearby))));
            index = newInstructions.FindLastIndex(index, instruction => instruction.Calls(PropertyGetter(typeof(RagdollAbilityBase<ZombieRole>), nameof(RagdollAbilityBase<ZombieRole>.IsInProgress)))) + offset;
            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Scp049SenseAbility
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(StartingZombieConsume), nameof(ServerProcessCmdRewrite))),
                    new(OpCodes.Br, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Basically rewrites the ServerProcessCmd - It really is not worth it to not do zombieAbilityBase
        /// </summary>
        /// <param name="senseAbility"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static void ServerProcessCmdRewrite(RagdollAbilityBase<ZombieRole> zombieAbilityBase)
        {

            if (zombieAbilityBase.IsInProgress)
            {
                return;
            }
            Transform transform;
            Vector3 position = zombieAbilityBase.ScpRole.FpcModule.Position;
            if (!zombieAbilityBase.IsCorpseNearby(position, zombieAbilityBase._syncRagdoll, out transform))
            {
                return;
            }

            Transform ragdollTransform = zombieAbilityBase._ragdollTransform;
            BasicRagdoll curRagdoll = zombieAbilityBase.CurRagdoll;
            zombieAbilityBase._ragdollTransform = transform;
            zombieAbilityBase.CurRagdoll = zombieAbilityBase._syncRagdoll;
            zombieAbilityBase._errorCode = zombieAbilityBase.ServerValidateBegin(zombieAbilityBase._syncRagdoll);

            API.Features.Player currentPlayer = API.Features.Player.Get(zombieAbilityBase.Owner);
            if (currentPlayer.Role.Type is not RoleTypeId.Scp049)
            {
                ZombieConsumeEventArgs zombieConsumeEvent = new ZombieConsumeEventArgs(currentPlayer, curRagdoll, ZombieConsumeAbility.ConsumedRagdolls, zombieAbilityBase._errorCode);
                Handlers.Scp049.OnStartingConsume(zombieConsumeEvent);

                if (!zombieConsumeEvent.IsAllowed)
                {
                    return;
                }

                curRagdoll = zombieConsumeEvent.TargetRagdoll;
                zombieAbilityBase._errorCode = zombieConsumeEvent.Errorcode;
            }

            bool flag = zombieAbilityBase._errorCode > 0;
            if (flag)
            {
                zombieAbilityBase._ragdollTransform = ragdollTransform;
                zombieAbilityBase.CurRagdoll = curRagdoll;
                if (flag)
                {
                    zombieAbilityBase.ServerSendRpc(true);
                }
                return;
            }
            zombieAbilityBase.IsInProgress = true;

        }

    }
}