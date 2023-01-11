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

    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;
    using GameCore;
    using HarmonyLib;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp106StalkAbility.ServerProcessCmd"/>.
    /// </summary>
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
            ServerChangingStalkEventArgs playerChangingStalkEventArgsStatus = new ServerChangingStalkEventArgs(currentPlayer, instance, instance._sinkhole.Cooldown, instance._isActive);
            Scp106.OnServerChangingStalk(playerChangingStalkEventArgsStatus);

            // FYI, if you get to 0 vigor, and keep denying event, it will keep calling this function, you'll need to actually change time/value
            // to slow down spam of 60 per minute
            if (!playerChangingStalkEventArgsStatus.IsAllowed)
            {
                // If NW handler is true, continue, otherwise, instant return
                return playerChangingStalkEventArgsStatus.AllowNwEventHandler;
            }

            if (playerChangingStalkEventArgsStatus.BypassChecks)
            {
                return false;
            }

            instance._isActive = isActiveValue;
            instance._valueDirty = true;
            instance.Owner.interCoordinator.AddBlocker(instance);
            if (isActiveValue)
            {
                instance.ScpRole.Sinkhole.TargetDuration = playerChangingStalkEventArgsStatus.TargetDuration;
                return false;
            }

            if (NetworkServer.active)
            {
                instance._sinkhole.Cooldown.Trigger(playerChangingStalkEventArgsStatus.Cooldown);
            }

            return false;
        }

        private static bool Scp106Stalking(Scp106StalkAbility instance, NetworkReader reader)
        {
            API.Features.Player currentPlayer = API.Features.Player.Get(instance.Owner);
            if (!instance.IsActive)
            {
                RequestingStalkEventArgs requestingStalkEventArgs = new RequestingStalkEventArgs(currentPlayer, instance, instance._sinkhole.Cooldown);
                Handlers.Scp106.OnStalking(requestingStalkEventArgs);
                return Scp106OnStalkingHandler(instance, requestingStalkEventArgs);
            }

            RequestingEndStalkEventArgs requestingEndStalkEventArgs = new RequestingEndStalkEventArgs(currentPlayer, instance, instance._sinkhole.Cooldown);
            Handlers.Scp106.OnLeavingStalk(requestingEndStalkEventArgs);
            return Scp106OnLeavingStalkingHandler(instance, requestingEndStalkEventArgs);
        }

        private static bool Scp106OnStalkingHandler(Scp106StalkAbility instance, RequestingStalkEventArgs requestingStalkEventArgs)
        {
            if (!requestingStalkEventArgs.IsAllowed)
            {
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlash(true);
                }

                instance.ServerSendRpc(false);
                instance.IsActive = false;
                return false;
            }

            // I would advise for this to exist due to how annoying it is to get property protected field.
            // Also, I would suggest diving by 120 because of the multiple by 120. int num = Mathf.FloorToInt(this.VigorAmount * 120f); lmao
            instance.Vigor.VigorAmount = Mathf.FloorToInt(requestingStalkEventArgs.Vigor * 120f);

            if (requestingStalkEventArgs.BypassChecks)
            {
                instance.IsActive = true;
                return false;
            }

            if (requestingStalkEventArgs.ValidateNewVigor && requestingStalkEventArgs.MinimumVigor > instance.Vigor.VigorAmount)
            {
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlash(true);
                }

                instance.ServerSendRpc(false);
                instance.IsActive = false;
                return false;
            }

            return true;
        }

        private static bool Scp106OnLeavingStalkingHandler(Scp106StalkAbility instance, RequestingEndStalkEventArgs requestingEndStalkEventArgs)
        {
            if (!requestingEndStalkEventArgs.IsAllowed || (requestingEndStalkEventArgs.MustUseAllVigor && instance.Vigor.VigorAmount > 0.0f))
            {
                // Force sinkhole/submerged
                instance.IsActive = true;
                if (instance.Role.IsLocalPlayer)
                {
                    Scp106Hud.PlayFlash(true);
                }

                instance.ServerSendRpc(false);
                return false;
            }

            // I would advise for this to exist due to how annoying it is to get property protected field.
            // Also, I would suggest diving by 120 because of the multiple by 120. int num = Mathf.FloorToInt(this.VigorAmount * 120f); lmao
            instance.Vigor.VigorAmount = Mathf.FloorToInt(requestingEndStalkEventArgs.Vigor * 120f);

            if (requestingEndStalkEventArgs.BypassChecks)
            {
                instance.IsActive = false;
                return false;
            }

            return true;
        }
    }
}