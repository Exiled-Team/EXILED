// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using GameCore;
using Mirror;
using UnityEngine;

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;


    [HarmonyPatch]
    internal static class Stalking
    {

        /// <summary>
        ///     Patches <see cref="Scp106HuntersAtlasAbility.ServerProcessCmd" />.
        ///     Adds the <see cref="Teleporting" /> event.
        /// </summary>
        [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.ServerProcessCmd))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> PatchServerProcessCmd(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // SCp106StalkAbility, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns DoctorSenseEventArgs
                    new(OpCodes.Call, Method(typeof(Stalking), nameof(Scp106Stalking))),
                    // If !ev.IsAllowed, return
                    new(OpCodes.Brfalse_S, returnLabel),

                });



            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }


        /// <summary>
        ///     Patches <see cref="Scp106HuntersAtlasAbility.ServerProcessCmd" />.
        ///     Adds the <see cref="Teleporting" /> event.
        /// </summary>
        [HarmonyPatch(typeof(Scp106StalkAbility), nameof(Scp106StalkAbility.IsActive), MethodType.Setter)]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> PatchIsActive(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // SCp106StalkAbility, NetworkReader
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns DoctorSenseEventArgs
                    new(OpCodes.Call, Method(typeof(Stalking), nameof(Scp106ChangingIsActive))),
                    // If !ev.IsAllowed, return
                    new(OpCodes.Brfalse_S, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool Scp106ChangingIsActive(Scp106StalkAbility instance, bool isActiveValue)
        {
            Player currentPlayer = Player.Get(instance.Owner);
            ServerChangingStalk playerChangingStalkStatus = new ServerChangingStalk(currentPlayer, instance, instance._sinkhole.Cooldown, instance._isActive);
            Scp106.OnServerChangingStalk(playerChangingStalkStatus);

            // FYI, if you get to 0 vigor, and keep denying event, it will keep calling this function, you'll need to actually change time/value
            // to slow down spam of 60 per minute
            if (!playerChangingStalkStatus.IsAllowed)
            {
                //If NW handler is true, continue, otherwise, instant return
                return playerChangingStalkStatus.AllowNwEventHandler;
            }

            if (playerChangingStalkStatus.BypassChecks)
            {
                return false;
            }

            instance._isActive = isActiveValue;
            instance._valueDirty = true;
            instance.Owner.interCoordinator.AddBlocker(instance);
            if (isActiveValue)
            {
                instance.ScpRole.Sinkhole.TargetDuration = playerChangingStalkStatus.TargetDuration;
                return false;
            }
            if (NetworkServer.active)
            {
                instance._sinkhole.Cooldown.Trigger(playerChangingStalkStatus.Cooldown);
            }

            return false;
        }

        private static bool Scp106Stalking(Scp106StalkAbility instance, NetworkReader reader)
        {

            API.Features.Player currentPlayer = API.Features.Player.Get(instance.Owner);
            if (!instance.IsActive)
            {
                PlayerTryEnterStalkEventArgs playerTryEnterStalkEventArgs = new PlayerTryEnterStalkEventArgs(currentPlayer, instance, instance._sinkhole.Cooldown);
                Handlers.Scp106.OnStalking(playerTryEnterStalkEventArgs);
                return Scp106OnStalkingHandler(instance, playerTryEnterStalkEventArgs);
            }

            PlayerTryLeaveStalkEventArgs playerTryLeaveStalkEventArgs = new PlayerTryLeaveStalkEventArgs(currentPlayer, instance, instance._sinkhole.Cooldown);
            Handlers.Scp106.OnLeavingStalk(playerTryLeaveStalkEventArgs);
            return Scp106OnLeavingStalkingHandler(instance, playerTryLeaveStalkEventArgs);
        }

        private static bool Scp106OnStalkingHandler(Scp106StalkAbility instance, PlayerTryEnterStalkEventArgs playerTryEnterStalkEventArgs)
        {
            if (!playerTryEnterStalkEventArgs.IsAllowed)
            {
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlashAnimation();
                }
                instance.ServerSendRpc(false);
                instance.IsActive = false;
                return false;
            }

            // I would advise for this to exist due to how annoying it is to get property protected field.
            // Also, I would suggest diving by 120 because of the multiple by 120. int num = Mathf.FloorToInt(this.VigorAmount * 120f); lmao
            instance.Vigor.VigorAmount = Mathf.FloorToInt(playerTryEnterStalkEventArgs.Vigor * 120f);

            if (playerTryEnterStalkEventArgs.BypassChecks)
            {
                instance.IsActive = true;
                return false;
            }

            if (playerTryEnterStalkEventArgs.ValidateNewVigor && playerTryEnterStalkEventArgs.MinimumVigor > instance.Vigor.VigorAmount)
            {
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlashAnimation();
                }
                instance.ServerSendRpc(false);
                instance.IsActive = false;
                return false;
            }
            return true;
        }

        private static bool Scp106OnLeavingStalkingHandler(Scp106StalkAbility instance, PlayerTryLeaveStalkEventArgs playerTryLeaveStalkEventArgs)
        {
            if ( !playerTryLeaveStalkEventArgs.IsAllowed || (playerTryLeaveStalkEventArgs.MustUseAllVigor && instance.Vigor.VigorAmount > 0.0f))
            {
                // Force sinkhole/submerged
                instance.IsActive = true;
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlashAnimation();
                }
                instance.ServerSendRpc(false);
                return false;
            }

            // I would advise for this to exist due to how annoying it is to get property protected field.
            // Also, I would suggest diving by 120 because of the multiple by 120. int num = Mathf.FloorToInt(this.VigorAmount * 120f); lmao
            instance.Vigor.VigorAmount = Mathf.FloorToInt(playerTryLeaveStalkEventArgs.Vigor * 120f);

            if (playerTryLeaveStalkEventArgs.BypassChecks)
            {
                instance.IsActive = false;
                return false;
            }
            return true;
        }
    }
}